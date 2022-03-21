namespace OrleansBank.Adapters.Storage
{
    public class IdempotencyMySqlStorageOptions
    {
        public string ConnectionString { get; set; }
        public int MaxIdempotencyKeysPerActiveGrain { get; set; } = 50;
    }
}
