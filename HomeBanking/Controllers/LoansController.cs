using AutoMapper;
using HomeBanking.DTOs;
using HomeBanking.Models;
using HomeBanking.Models.Enums;
using HomeBanking.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private ITransactionRepository _transactionRepository;

        private readonly IMapper _mapper;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository, ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }
        [HttpGet]

        public IActionResult Get()

        {

            try

            {

                var loans = _loanRepository.GetLoans();

                var loansDTO = _mapper.Map<List<LoanDTO>>(loans);

                return Ok(loansDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }

        [HttpPost]
        public IActionResult Post([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("Acceso no autorizado");
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client is null)
                {
                    return Unauthorized("Acceso no autorizado");
                }

                if (!PerformValidations(loanApplicationDTO))
                {
                    return BadRequest(ModelState);
                }


                ClientLoan clientLoan = new ClientLoan
                {
                    LoanId = loanApplicationDTO.LoanId,
                    ClientId = client.Id,
                    Amount = loanApplicationDTO.Amount + loanApplicationDTO.Amount * 0.2,
                    Payments = loanApplicationDTO.Payments.ToString(),
                };

                _clientLoanRepository.Save(clientLoan);

                string type = _loanRepository.FindById(loanApplicationDTO.LoanId).Name;
                ClientLoanDTO clientLoanDTO = new ClientLoanDTO
                {
                    LoanId = loanApplicationDTO.LoanId,
                    ClientId = client.Id,
                    Name = type,
                    Amount = loanApplicationDTO.Amount + loanApplicationDTO.Amount * 0.2,
                    Payments = loanApplicationDTO.Payments,
                };


                Account account = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);

                Transaction transac = new Transaction
                {
                    Type = TransactionType.CREDIT.ToString(),
                    Amount = loanApplicationDTO.Amount,
                    Description = type,
                    AccountId = account.Id,
                    Date = DateTime.Now,

                };

                _transactionRepository.Save(transac);

                var newTransacDTO = _mapper.Map<TransactionDTO>(transac);

                return Created("", newTransacDTO);
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private bool PerformValidations(LoanApplicationDTO loanApplicationDTO)
        {
            Loan existingLoan = _loanRepository.FindById(loanApplicationDTO.LoanId);
            if (existingLoan is null)
            {
                ModelState.AddModelError("LoanId", "El préstamo especificado no existe.");
                return false;
            }

            double maxAmount = _loanRepository.GetMaxAmmountById(loanApplicationDTO.LoanId);

            if (loanApplicationDTO.Amount <= 0 || loanApplicationDTO.Amount > maxAmount)
            {
                ModelState.AddModelError("Amount", "El monto del préstamo está fuera del rango permitido");
                return false;
            }

            if (!_loanRepository.IsPaymentValid(loanApplicationDTO.LoanId, loanApplicationDTO.Payments))
            {
                ModelState.AddModelError("Payment", "El intervalo de pago no es válido para este préstamo.");
                return false;
            }

            var account = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);
            if (account is null)
            {
                ModelState.AddModelError("ToAccountNumber", "La cuenta especificada no existe.");
                return false;
            }

            var client = _clientRepository.FindByEmail(User.FindFirst("Client").Value);
            if (account.ClientId != client.Id)
            {
                ModelState.AddModelError("ToAccountNumber", "La cuenta especificada no pertenece al mismo usuario.");
                return false;
            }

            return true;
        }



    }
}
