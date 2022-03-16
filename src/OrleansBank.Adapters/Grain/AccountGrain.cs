using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using OrleansBank.Adapters.Storage;
using OrleansBank.Domain;

namespace OrleansBank.Adapters.Grain
{
    [StorageProvider(ProviderName = "Accounts")]
    public class AccountGrain : Grain<(IdempotencyShield shield, Account account)>, IAccountActor
    {
        public async Task<bool> MakeCredit(string uniqueId, double amount)
        {
            return await ExecuteIdempotently(uniqueId, () => State.account.MakeCredit(uniqueId, amount));
        }

        public async Task<bool> MakeDebit(string uniqueId, double amount)
        {
            return await ExecuteIdempotently(uniqueId, () => State.account.MakeDebit(uniqueId, amount));
        }

        private async Task<bool> ExecuteIdempotently(string idempotentyKey, Func<Task<bool>> func)
        {
            if(State.shield.CheckCommitedAction(idempotentyKey))
            {
                return true;
            }
            var result = await func();
            State.shield.CommitAction(idempotentyKey);
            try
            {
                await WriteStateAsync();
            }
            catch (OrleansException ex) when (ex.InnerException is IdempotencyFailureException)
            {
                // sync the state again, since the storage was the one that realised that the call was duplicated
                await ReadStateAsync();
                return true;
            }
            return result;
        }

        public Task<double> GetBalance()
        {
            return State.account.GetBalance();
        }
    }
}