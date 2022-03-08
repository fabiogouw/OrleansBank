using Orleans;
using OrleansBank.Domain;

namespace OrleansBank.Adapters
{
    public interface IAccountActor : IGrainWithStringKey, IAccount
    {
    }
}
