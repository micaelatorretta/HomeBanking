using HomeBanking.Models;
using System.Collections.Generic;

namespace HomeBanking.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        void Save(Account account);
        Account FindById(long id);
        IEnumerable<Account> GetAccountsByClient(long Clientid);
        string GetLastAccountNumber();
        string GenerateNextAccountNumber();
    }
}
