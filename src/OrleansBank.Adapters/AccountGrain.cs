using Orleans;
using Orleans.Providers;
using OrleansBank.Domain;

namespace OrleansBank.Adapters
{
    [StorageProvider(ProviderName = "Accounts")]
    public class AccountGrain : Grain<Account>, IAccountActor
    {
        public async Task MakeCredit(string uniqueId, double amount)
        {
            await State.MakeCredit(uniqueId, amount);
            await WriteStateAsync();
        }

        public async Task MakeDebit(string uniqueId, double amount)
        {
            await State.MakeDebit(uniqueId, amount);
            await WriteStateAsync();
        }

        public Task<double> GetBalance()
        {
            return State.GetBalance();
        }
    }
}