using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HomeBanking.DTOs
{
    public class ClientDTO
    {
      //  [JsonIgnore] //Se comento este atributo porque se usa el Id del cliente para obtener sus tarjetas.
        public long Id { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public ICollection<AccountDTO> Accounts { get; set; }
        public ICollection<ClientLoanDTO> Credits { get; set; }
        public ICollection<CardDTO> Cards { get; set; }
    }
}
