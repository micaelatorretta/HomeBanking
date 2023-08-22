using AutoMapper;
using HomeBanking.DTOs;
using HomeBanking.Models;

namespace HomeBanking.Automapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Client, ClientDTO>().ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.ClientLoans)).ReverseMap();
    
            CreateMap<Account, AccountDTO>().ReverseMap();

            CreateMap<ClientLoan, ClientLoanDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Loan.Name))
                .ReverseMap();

            CreateMap<Transaction, TransactionDTO>().ReverseMap();

            CreateMap<Card, CardDTO>().ReverseMap();

            CreateMap<Loan, LoanDTO>().ReverseMap();



        }

    }
}
