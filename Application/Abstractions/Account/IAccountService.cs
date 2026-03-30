using Application.Common.Results;

namespace Application.Abstractions.Account;

public interface IAccountService
{
    Task<AccountResult> DeleteUserAccountAsync(string userId);
}
