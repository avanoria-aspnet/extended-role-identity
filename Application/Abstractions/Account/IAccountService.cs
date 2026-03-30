using Application.Common.Results;
using Application.Dtos;

namespace Application.Abstractions.Account;

public interface IAccountService
{
    Task<AccountResult> GetUserAccountAsync(string userId);
    Task<AccountResult> DeleteUserAccountAsync(string userId);
    Task<AccountResult> UpdateUserAccountDetailsAsync(UpdateAccountDetails details);
}
