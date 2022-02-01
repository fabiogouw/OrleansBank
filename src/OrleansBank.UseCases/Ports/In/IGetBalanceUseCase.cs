using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBank.UseCases.Ports.In
{
    public interface IGetBalanceUseCase
    {
        Task<double> Execute(string accountId);
    }
}
