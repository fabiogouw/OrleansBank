using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBank.Domain
{
    public class Transaction
    {
        public Account Account { get; private set; }
        public string Id { get; private set; }
        public double Amount { get; init; }
        public DateTime CreatedAt { get; init; }

        public Transaction(Account account, string id)
        {
            Account = account;
            Id = id;
        }
    }
}
