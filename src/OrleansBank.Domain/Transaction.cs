using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBank.Domain
{
    public record Transaction
    {
        public string Id { get; init; }
        public double Amount { get; init; }
        public DateTime DateTime { get; init; }
    }
}
