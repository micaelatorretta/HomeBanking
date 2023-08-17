using HomeBanking.Models;

namespace HomeBanking.Repositories.Interfaces
{
    public interface IClientLoanRepository
    {
        void Save(ClientLoan clientLoan);
    }
}
