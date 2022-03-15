using Orleans;
using OrleansBank.Domain;

namespace OrleansBank.Adapters.Grain
{
    public interface IAccountActor : IGrainWithStringKey, IAccount
    {
    }
}
