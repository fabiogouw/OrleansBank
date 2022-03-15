namespace OrleansBank.Adapters.Storage
{
    public class IdempotencyShield
    {
        private readonly Queue<string> _idempotencyKeys = new Queue<string>();
        private readonly string _entityKey;
        private readonly int _maxSize = 50;
        private DateTime _updatedAt;

        public string EntityKey
        {
            get { return _entityKey; }
        }

        public string LastIdempotencyKey
        {
            get { return _idempotencyKeys.LastOrDefault(); }
        }

        public DateTime UpdatedAt
        {
            get { return _updatedAt; }
        }

        public IdempotencyShield(string entityKey, IEnumerable<string> idempotencyKeys, int maxSize)
        {
            _entityKey = entityKey;
            _idempotencyKeys = new Queue<string>(idempotencyKeys.Take(maxSize));
            _maxSize = maxSize;
            _updatedAt = DateTime.Now;
        }
        public bool CheckCommitedAction(string idempotencyKey)
        {
            return _idempotencyKeys.Contains(idempotencyKey);
        }

        public void CommitAction(string idempotencyKey)
        {
            _idempotencyKeys.Enqueue(idempotencyKey);
            _updatedAt = DateTime.Now;
            if (_idempotencyKeys.Count > _maxSize)
            {
                _idempotencyKeys.Dequeue();
            }
        }
    }
}
