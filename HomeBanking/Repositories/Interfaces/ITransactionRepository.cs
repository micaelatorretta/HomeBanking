using HomeBanking.Models;

namespace HomeBanking.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        void Save(Transaction transaction);
        Transaction FindByNumber(long id);
    }
}
