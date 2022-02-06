using Orleans;
using OrleansBank.Domain;
using OrleansBank.UseCases.Ports.Out;

namespace OrleansBank.Adapters
{
    public class OrleansAccountRepository : IAccountRepository
    {
        private readonly IGrainFactory _factory;

        public OrleansAccountRepository(IGrainFactory factory)
        {
            _factory = factory;
        }

        public IAccount Get(string id)
        {
            IAccount account = _factory.GetGrain<IAccountActor>(id);
            return account;
        }

        public void Save(IAccount account)
        {
            throw new NotImplementedException();
        }
    }
}
