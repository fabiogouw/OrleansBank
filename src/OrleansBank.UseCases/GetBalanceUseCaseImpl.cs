using OrleansBank.UseCases.Ports.In;
using OrleansBank.UseCases.Ports.Out;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBank.UseCases
{
    public class GetBalanceUseCaseImpl : IGetBalanceUseCase
    {
        private readonly IAccountRepository _accountRepository;

        public GetBalanceUseCaseImpl(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<double> Execute(string accountId)
        {
            var account = _accountRepository.Get(accountId);
            return await account.GetBalance();
        }
    }
}
