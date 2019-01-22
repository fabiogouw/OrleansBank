using System;
using System.Threading.Tasks;
using OrleansBank.Contracts;
using Orleans;
using OrleansBank.Core.Domain;
using Orleans.Providers;
using System.Collections.Generic;
using System.Linq;

namespace OrleansBank.Core.Adapters.Actors
{
    [StorageProvider(ProviderName = "OrleansStorage")]
    public class CheckingAccountGrain : Grain<Account>, ICheckingAccount
    {
        public async Task<double> Credit(double amount, string description)
        {
            State.AddTransaction(amount, description);
            await WriteStateAsync();
            return State.Balance;
        }

        public async Task<double> Debit(double amount, string description)
        {
            State.AddTransaction(-1 * amount, description);
            await WriteStateAsync();
            return State.Balance;
        }

        public Task<double> GetBalance()
        {
            return Task.FromResult(State.Balance);
        }

        public Task<List<string>> GetTransactions()
        {
            return Task.FromResult(State.Transactions
                .Select(t => t.ToString())
                .ToList());
        }
    }
}
