using Orleans;
using OrleansBank.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBank.Adapters
{
    public interface IAccountActor : IGrainWithStringKey, IAccount
    {
    }
}
