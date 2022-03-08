using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBank.Adapters.Storage
{
    public class IdempotencyMySqlStorageOptions
    {
        public string ConnectionString { get; set; }
        public int MaxIdempotencyKeysPerActiveGrain { get; set; } = 50;
    }
}
