using System;

namespace OrleansBank.Core.Domain
{
    public class Transaction
    {
        public Transaction(string accountId) 
        {
            AccountId = accountId;
        }
        public string AccountId { get; }
        public DateTime Timestamp { get; set; }
        public double Amount {get;set;}
        public string Description {get;set;}
    }
}