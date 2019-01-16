using System;
using System.Threading.Tasks;
using OrleansBank.Core.Contracts;

namespace OrleansBank.Core
{
    public class CheckingAccountGrain : Orleans.Grain, ICheckingAccount
    {
        public Task<double> Credit(double amount, string description)
        {
            throw new NotImplementedException();
        }

        public Task<double> Debit(double amount, string description)
        {
            throw new NotImplementedException();
        }

        public Task<double> GetBalance()
        {
            return Task.FromResult(new Random().NextDouble());
        }
    }
}
