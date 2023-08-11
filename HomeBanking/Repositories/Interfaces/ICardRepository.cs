using HomeBanking.Models;
using System.Collections.Generic;

namespace HomeBanking.Repositories.Interfaces
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAllCards();
        void Save(Card card);
        Card FindById(long id);
        string GetLastCardNumber();
        string GenerateNextCardNumber();
    }
}
