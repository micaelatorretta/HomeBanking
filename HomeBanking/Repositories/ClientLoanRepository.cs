using HomeBanking.Models;
using HomeBanking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HomeBanking.Repositories
{
    public class ClientLoanRepository : RepositoryBase<ClientLoan>, IClientLoanRepository
    {
        public ClientLoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }
        public ClientLoan FindById(long idClient, long idLoan)
        {
            return FindByCondition(cl => cl.ClientId == idClient && cl.LoanId==idLoan)
                .Include(cl => cl.Client)
                .Include(cl => cl.Loan)
                .FirstOrDefault();
        }
        public void Save(ClientLoan clientLoan)
        {
            Create(clientLoan);
            SaveChanges();
        }
    }
}
