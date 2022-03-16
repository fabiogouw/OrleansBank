using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBank.Adapters.Storage
{
    [Serializable]
    public class IdempotencyFailureException : ApplicationException
    {
        public IdempotencyFailureException(string entity, Exception innerException)
            : base($"Idempotency check failure for entity { entity }.", innerException)
        {
            Entity = entity;
        }

        public string Entity { get; private init; }
    }
}
