using MySql.Data.MySqlClient;
using Orleans;
using Orleans.Runtime;
using Orleans.Storage;
using OrleansBank.Domain;

namespace OrleansBank.Adapters.Storage
{
    public class IdempotentMySqlAccountStorage : IGrainStorage
    {
        private readonly string _storageName;
        private readonly IdempotencyMySqlStorageOptions _options;

        public IdempotentMySqlAccountStorage(string storageName, IdempotencyMySqlStorageOptions options)
        {
            _storageName = storageName;
            _options = options;
        }

        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            return Task.CompletedTask;
        }

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var connection = new MySqlConnection(_options.ConnectionString);
            connection.Open();
            try
            {
                var (account, etag) = ReadAccountWithEtag(connection, grainReference.GrainIdentity.PrimaryKeyString);
                var idempotencyShield = ReadIdempotencyShield(connection, grainReference.GrainIdentity.PrimaryKeyString);
                grainState.State = (idempotencyShield, account);
                grainState.RecordExists = true;
                grainState.ETag = etag;
            }
            finally
            {
                connection.Close();
            }
            return Task.CompletedTask;
        }

        private (Account, string) ReadAccountWithEtag(MySqlConnection connection, string accountId)
        {
            var cmd = new MySqlCommand(@"SELECT account_id, balance, etag, updated_at FROM tb_account 
                                            WHERE account_id = @account_id", connection);
            cmd.Parameters.AddWithValue("@account_id", accountId);
            using var reader = cmd.ExecuteReader();
            var account = EnumerableFromReader(reader, () => CreateAccountFromReader(reader))
                .Single();  // TODO: but what if there isn't an account in the database?
            return (account, reader.GetString("etag"));
        }

        private IdempotencyShield ReadIdempotencyShield(MySqlConnection connection, string accountId)
        {
            var cmd = new MySqlCommand(@"SELECT idempotency_key
                                            FROM tb_idempotency_keys
                                            WHERE account_id = @account_id 
                                            ORDER BY created_at DESC 
                                            LIMIT @max_idempotency_keys", connection);
            cmd.Parameters.AddWithValue("@account_id", accountId);
            cmd.Parameters.AddWithValue("@max_idempotency_keys", _options.MaxIdempotencyKeysPerActiveGrain);
            using var reader = cmd.ExecuteReader();
            var idempotencyKeys = EnumerableFromReader(reader, () => reader.GetString("idempotency_key"));
            return new IdempotencyShield(accountId, idempotencyKeys, _options.MaxIdempotencyKeysPerActiveGrain);
        }
        private static IEnumerable<T> EnumerableFromReader<T>(MySqlDataReader reader, Func<T> generator)
        {
            while (reader.Read())
            {
                yield return generator();
            }
        }

        private static Account CreateAccountFromReader(MySqlDataReader reader)
        {
            return new Account(reader.GetString("account_id"), reader.GetDouble("balance"), new List<Transaction>());
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var state = ((IdempotencyShield shield, Account account)) grainState.State;
            var connection = new MySqlConnection(_options.ConnectionString);
            string newEtag = CalculareNewEtag(grainState.ETag);

            var commands = new MySqlCommand[] 
                {
                    GetIdempotencyKeyWriteCommand(connection, state.shield),
                    GetAccountWriteCommand(connection, state.account, grainState.ETag, newEtag)
                }
                .Union(GetTransactionWriteCommands(connection, state.account.Transactions))
                ;
            connection.Open();
            try
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    foreach (var command in commands)
                    {
                        command.Transaction = transaction;
                        int rowsAffected = command.ExecuteNonQuery();
                        // TODO: every command should affect at least one row, but is this the right exception type to use?
                        if (rowsAffected == 0)
                        {
                            throw new InconsistentStateException(@$"Version conflict (WriteState): 
                                ProviderName={_storageName} GrainType={grainType} GrainReference={grainReference.ToKeyString()}.");
                        }
                    }
                    transaction.Commit();
                    RecreateState(grainState, state.account, newEtag);
                }
                catch (MySqlException ex)
                {
                    transaction.Rollback();
                    throw CheckIdempotencyException(ex);
                }
            }
            finally
            {
                connection?.Close();
            }
            return Task.CompletedTask;
        }

        private Exception CheckIdempotencyException(MySqlException ex)
        {
            if (ex.Number == (int)MySqlErrorCode.DuplicateKeyEntry && ex.Message.Contains("tb_idempotency_keys.PRIMARY"))
            {
                return new IdempotencyFailureException("account", ex);
            }
            return ex;
        }

        private static void RecreateState(IGrainState grainState, Account account, string newEtag)
        {
            grainState.ETag = newEtag;
            var state = ((IdempotencyShield shield, Account account))grainState.State;
            // after the update, we ignore the saved transactions and recreate the state with the new balance
            state.account = new Account(account.Id, account.Balance, new List<Transaction>());
        }

        private static string CalculareNewEtag(string etag)
        {
            return (int.Parse(etag) + 1).ToString();
        }

        private MySqlCommand GetAccountWriteCommand(MySqlConnection connection, Account account, string currentEtag, string newEtag)
        {
            var cmd = new MySqlCommand(@"UPDATE tb_account
                                            SET balance = @balance,
                                                updated_at = @updated_at,
                                                etag = @newEtag
                                            WHERE account_id = @account_id 
                                            AND etag = @currentEtag", connection);
            cmd.Parameters.AddWithValue("@account_id", account.Id);
            cmd.Parameters.AddWithValue("@balance", account.Balance);
            cmd.Parameters.AddWithValue("@updated_at", account.UpdatedAt);
            cmd.Parameters.AddWithValue("@currentEtag", int.Parse(currentEtag));
            cmd.Parameters.AddWithValue("@newEtag", int.Parse(newEtag));
            return cmd;
        }

        private IEnumerable<MySqlCommand> GetTransactionWriteCommands(MySqlConnection connection, IEnumerable<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                var cmd = new MySqlCommand(@"INSERT INTO tb_transactions
                                            (transaction_id, account_id, amount, created_at)
                                            VALUES (@transaction_id, @account_id, @amount, @created_at)", connection);
                cmd.Parameters.AddWithValue("@transaction_id", transaction.Id);
                cmd.Parameters.AddWithValue("@account_id", transaction.Account.Id);
                cmd.Parameters.AddWithValue("@amount", transaction.Amount);
                cmd.Parameters.AddWithValue("@created_at", transaction.CreatedAt);
                yield return cmd;
            }
        }

        private MySqlCommand GetIdempotencyKeyWriteCommand(MySqlConnection connection, IdempotencyShield idempotencyShield)
        {
            var cmd = new MySqlCommand(@"INSERT INTO tb_idempotency_keys
                                            (idempotency_key, account_id, created_at)
                                            VALUES (@idempotency_key, @account_id, @created_at)", connection);
            cmd.Parameters.AddWithValue("@idempotency_key", idempotencyShield.LastIdempotencyKey);
            cmd.Parameters.AddWithValue("@account_id", idempotencyShield.EntityKey);
            cmd.Parameters.AddWithValue("@created_at", idempotencyShield.UpdatedAt);
            return cmd;
        }
    }
}
