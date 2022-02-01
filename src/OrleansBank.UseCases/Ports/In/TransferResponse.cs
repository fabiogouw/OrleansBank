using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBank.UseCases.Ports.In
{
    public record TransferResponse
    {
        public string UniqueClientId { get; init; } = string.Empty;
        public string Id { get; init; } = string.Empty;
        public DateTime RequestedAt { get; init; }
        public bool Success { get; init; }
        public string Details { get; init; } = string.Empty;
    }
}
