using HomeBanking.Models;

namespace HomeBanking.Repositories.Interfaces
{
    public interface IClientLoanRepository
    {
        ClientLoan FindById(long idClient, long idLoan);
        void Save(ClientLoan clientLoan);
    }
}
