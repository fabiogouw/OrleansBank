using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBank.UseCases.Ports.In
{
    public record TransferRequest
    {
        public string UniqueClientId { get; init; } = string.Empty;
        public string DebitAccountId { get; init; } = string.Empty;
        public string CreditAccountId { get; init; } = string.Empty;
        public double Amount { get; init; }
    }
}
