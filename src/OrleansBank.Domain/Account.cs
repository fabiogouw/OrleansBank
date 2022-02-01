namespace OrleansBank.Domain
{
    public class Account : IAccount
    {
        private double _balance = 10;
        private List<Transaction> _transactions = new List<Transaction>();

        public Task MakeDebit(string uniqueId, double amount)
        {
            _balance -= amount;
            _transactions.Add(new Transaction()
            {
                Id = uniqueId,
                Amount = amount,
                DateTime = DateTime.Now
            });
            return Task.CompletedTask;
        }

        public Task MakeCredit(string uniqueId, double amount)
        {
            _balance += amount;
            _transactions.Add(new Transaction()
            {
                Id = uniqueId,
                Amount = amount,
                DateTime = DateTime.Now
            });
            return Task.CompletedTask;
        }

        public Task<double> GetBalance()
        {
            return Task.FromResult(_balance);
        }
    }
}