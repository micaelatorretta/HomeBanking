using AutoMapper;
using HomeBanking.DTOs;
using HomeBanking.Models;

namespace HomeBanking.Automapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Client, ClientDTO>()
                .ReverseMap();


            CreateMap<Account, AccountDTO>()
                .ReverseMap();

            CreateMap<ClientLoan, ClientLoanDTO>()
                .ReverseMap();

            CreateMap<Transaction, TransactionDTO>()
                .ReverseMap();

            CreateMap<Card, CardDTO>()
                .ReverseMap();

        }

    }
}
