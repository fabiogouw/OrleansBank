namespace OrleansBank.Domain
{
    public interface IAccount
    {
        Task MakeCredit(string uniqueId, double amount);
        Task MakeDebit(string uniqueId, double amount);
        Task<double> GetBalance();
    }
}