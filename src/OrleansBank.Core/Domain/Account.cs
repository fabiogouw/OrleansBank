using System;
using System.Collections.Generic;

namespace OrleansBank.Core.Domain
{
    public class Account
    {
        private string _id;
        private double _balance = 0;
        private int ClientId { get; set; }
        private List<Transaction> _transactions = new List<Transaction>();

        public double Balance
        {
            get { return _balance; }
        }

        public IEnumerable<Transaction> Transactions
        {
            get { return _transactions; }
        }

        public void AddTransaction(double amount, string description)
        {
            var transaction = new Transaction(_id)
            {
                Amount = amount,
                Description = description
            };
            _transactions.Add(transaction);
            _balance += amount;
        }
    }
}