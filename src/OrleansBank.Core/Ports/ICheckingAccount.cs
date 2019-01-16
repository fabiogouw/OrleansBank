using System;
using System.Threading.Tasks;
using Orleans;

namespace OrleansBank.Core.Contracts
{
    public interface ICheckingAccount : IGrainWithStringKey
    {
        Task<double> GetBalance();
        Task<double> Credit(double amount, string description);
        Task<double> Debit(double amount, string description);
    }
}
