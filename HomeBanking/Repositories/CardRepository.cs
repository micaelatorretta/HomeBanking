using HomeBanking.Models;
using HomeBanking.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeBanking.Repositories
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }
        public Card FindById(long id)
        {
            return RepositoryContext.Cards.Where(card => card.Id == id).FirstOrDefault();
        }
        public IEnumerable<Card> GetAllCards()
        {
            return FindAll()
                .ToList();
        }
        public IEnumerable<Card> GetCardsByClient(long clientId)
        {
            return FindByCondition(card => card.ClientId == clientId)
           .ToList();
        }
        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }


        public string GetLastCardNumber()
        {
            var lastCard = FindAll()
                .OrderByDescending(card => card.Id)
                .FirstOrDefault();
            if (lastCard == null)
            {
                return null; // no hay tarjetas en la base de datos
            }
            return lastCard.Number;
        }

        
        public string GenerateNextCardNumber()
        {
            string lastCardNumber = GetLastCardNumber();

            if (lastCardNumber == null)
            {
                return "1000-0000-0000-0000"; // Comienza desde un valor predeterminado si no hay cuentas
            }

            long nextNumber = long.Parse(lastCardNumber.Replace("-", "")) + 1; 
            string formattedNumber = $"{nextNumber:D16}"; // Formatear el número calculado con ceros a la izquierda en un total de 16 dígitos
            string formattedCardNumber = string.Format("{0:0000-0000-0000-0000}", long.Parse(formattedNumber)); // Formatear el número con guiones en grupos de cuatro dígitos cada uno

            return formattedCardNumber;
        }


    }
}
