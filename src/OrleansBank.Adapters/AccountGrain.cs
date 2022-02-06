using Orleans;
using Orleans.Providers;
using OrleansBank.Domain;

namespace OrleansBank.Adapters
{
    [StorageProvider(ProviderName = "Accounts")]
    public class AccountGrain : Grain<Account>, IAccountActor
    {
        public async Task<bool> MakeCredit(string uniqueId, double amount)
        {
            var result = await State.MakeCredit(uniqueId, amount);
            await WriteStateAsync();
            return result;
        }

        public async Task<bool> MakeDebit(string uniqueId, double amount)
        {
            var result = await State.MakeDebit(uniqueId, amount);
            await WriteStateAsync();
            return result;
        }

        public Task<double> GetBalance()
        {
            return State.GetBalance();
        }
    }
}