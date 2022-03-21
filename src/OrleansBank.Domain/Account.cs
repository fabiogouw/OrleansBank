namespace OrleansBank.Domain
{
    public class Account : IAccount
    {
        private string _id;
        private double _balance = 10;
        private List<Transaction> _transactions = new List<Transaction>();
        private DateTime _updatedAt;

        public string Id
        {
            get { return _id; }
        }

        public double Balance
        {
            get { return _balance; }
        }

        public DateTime UpdatedAt
        {
            get { return _updatedAt; }
        }

        public IEnumerable<Transaction> Transactions
        {
            get { return _transactions; }
        }

        public Account(string id, double balance, List<Transaction> transactions)
        {
            _id = id;
            _balance = balance;
            _transactions = transactions;
        }

        public Task<bool> MakeDebit(string uniqueId, double amount)
        {
            var now = DateTime.Now;
            if (_balance < amount)
            {
                return Task.FromResult(false);
            }
            _balance -= amount;
            _transactions.Add(new Transaction(this, uniqueId)
            {
                Amount = amount,
                CreatedAt = now
            });
            _updatedAt = now;
            return Task.FromResult(true);
        }

        public Task<bool> MakeCredit(string uniqueId, double amount)
        {
            var now = DateTime.Now;
            _balance += amount;
            _transactions.Add(new Transaction(this, uniqueId)
            {
                Amount = amount,
                CreatedAt = now
            });
            _updatedAt = now;
            return Task.FromResult(true);
        }

        public Task<double> GetBalance()
        {
            return Task.FromResult(_balance);
        }

    }
}