namespace OrleansBank.Domain
{
    public interface IAccount
    {
        Task<bool> MakeCredit(string uniqueId, double amount);
        Task<bool> MakeDebit(string uniqueId, double amount);
        Task<double> GetBalance();
    }
}