using HomeBanking.Models;
using HomeBanking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HomeBanking.Repositories
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
 
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Account FindById(long id)
        {
            return FindByCondition(account => account.Id == id)
                .Include(account => account.Transactions)
                .FirstOrDefault();
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll()
                .Include(account => account.Transactions)
                .ToList();
        }

        public void Save(Account account)
        {
            Create(account);
            SaveChanges();
        }

        public IEnumerable<Account> GetAccountsByClient(long clientId)
        {
            return FindByCondition(account => account.ClientId == clientId)
                .Include(account => account.Transactions)
                .ToList();
        }

        public string GetLastAccountNumber()
        {
            var lastAccount = FindAll()
                .OrderByDescending(account => account.Id)
                .FirstOrDefault();

            if (lastAccount == null)
            {
                return null; // No hay cuentas en la base de datos
            }

            return lastAccount.Number;
        }

        public string GenerateNextAccountNumber()
        {
            string lastAccountNumber = GetLastAccountNumber();
            if (lastAccountNumber == null)
            {
                return "VIN-100000"; // Comienza desde un valor predeterminado si no hay cuentas
            }

            int lastNumber = int.Parse(lastAccountNumber.Substring(4));
            int nextNumber = lastNumber + 1;

            return "VIN-" + nextNumber.ToString("D6"); // D6 asegura 6 dígitos con ceros iniciales
        }
    }
}
