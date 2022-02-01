using OrleansBank.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBank.UseCases.Ports.Out
{
    public interface IAccountRepository
    {
        IAccount Get(string id);
        void Save(IAccount account);
    }
}
