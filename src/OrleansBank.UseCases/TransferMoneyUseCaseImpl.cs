using OrleansBank.Domain;
using OrleansBank.UseCases.Ports.In;
using OrleansBank.UseCases.Ports.Out;

namespace OrleansBank.UseCases
{
    public class TransferMoneyUseCaseImpl : ITransferMoneyUseCase
    {
        private readonly IAccountRepository _accountRepository;
        public TransferMoneyUseCaseImpl(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<TransferResponse> Execute(TransferRequest request)
        {
            var debitAccount = _accountRepository.Get(request.DebitAccountId);
            var creditAccount = _accountRepository.Get(request.CreditAccountId);
            var debitAccountBalance = await debitAccount.GetBalance();
            if (debitAccountBalance >= request.Amount)
            {
                await debitAccount.MakeDebit(request.UniqueClientId, request.Amount);
                await creditAccount.MakeCredit(request.UniqueClientId, request.Amount);
                return new TransferResponse()
                {
                    UniqueClientId = request.CreditAccountId,
                    Success = true,
                    Id = Guid.NewGuid().ToString(),
                    RequestedAt = DateTime.UtcNow
                };
            }
            return new TransferResponse()
            {
                UniqueClientId = request.CreditAccountId,
                Success = false,
                Id = Guid.NewGuid().ToString(),
                RequestedAt = DateTime.UtcNow
            };
        }
    }
}