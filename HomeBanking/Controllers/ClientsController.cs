using HomeBanking.DTOs;
using HomeBanking.Models;
using HomeBanking.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using HomeBanking.Models.Enums;
using AutoMapper;
using HomeBanking.Utils;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]

    [ApiController]

    public class ClientsController : ControllerBase

    {
        private readonly IMapper _mapper;
        private IClientRepository _clientRepository;
        // private IAccountRepository _accountRepository;
        private AccountsController _accountsController;
        private CardsController _cardsController;
        private IAccountRepository _accountRepository;
        public ClientsController(IMapper mapper, IClientRepository clientRepository, AccountsController accountsController, CardsController cardsController, IAccountRepository accountRepository)

        {
            _clientRepository = clientRepository;
            _accountsController = accountsController;
            _cardsController = cardsController;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }



        [HttpGet]

        public IActionResult Get()

        {

            try

            {

                var clients = _clientRepository.GetAllClients();



                var clientsDTO = _mapper.Map<List<ClientDTO>>(clients);/* new List<ClientDTO>();*/



                //foreach (Client client in clients)

                //{

                //    var newClientDTO = new ClientDTO

                //    {

                //        Id = client.Id,

                //        Email = client.Email,

                //        FirstName = client.FirstName,

                //        LastName = client.LastName,

                //        Accounts = client.Accounts.Select(ac => new AccountDTO

                //        {

                //            Id = ac.Id,

                //            Balance = ac.Balance,

                //            CreationDate = ac.CreationDate,

                //            Number = ac.Number

                //        }).ToList(),
                //        Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                //        {
                //            Id = cl.Id,
                //            LoanId = cl.LoanId,
                //            Name = cl.Loan.Name,
                //            Amount = cl.Amount,
                //            Payments = int.Parse(cl.Payments)
                //        }).ToList(),
                //        Cards = client.Cards.Select(c => new CardDTO
                //        {
                //            Id = c.Id,
                //            CardHolder = c.CardHolder,
                //            Color = c.Color,
                //            Cvv = c.Cvv,
                //            FromDate = c.FromDate,
                //            Number = c.Number,
                //            ThruDate = c.ThruDate,
                //            Type = c.Type
                //        }).ToList()



                //    };


                
                //    clientsDTO.Add(newClientDTO);

                //}





                return Ok(clientsDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }



        [HttpGet("{id}")]

        public IActionResult Get(long id)

        {

            try

            {

                var client = _clientRepository.FindById(id);

                if (client == null)

                {

                    return NotFound(); //404

                }

                ClientDTO clientDTO=_mapper.Map<ClientDTO>(client);

                return Ok(clientDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Unauthorized();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Unauthorized();
                }

                ClientDTO clientDTO = _mapper.Map<ClientDTO>(client);

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] ClientDTO clientDTO)
        {
            try
            {
                if (!ValidationUtils.IsNameValid(clientDTO.FirstName) || !ValidationUtils.IsNameValid(clientDTO.LastName))
                {
                    return StatusCode(400, "datos inválidos");
                }

                if (!ValidationUtils.IsPasswordValid(clientDTO.Password))
                {
                    return StatusCode(400, "datos inválidos");
                }

                if (!ValidationUtils.IsValidEmail(clientDTO.Email))
                {
                    return StatusCode(400, "Email inválido.");
                }

                // Validamos los datos antes de continuar
                if (String.IsNullOrEmpty(clientDTO.Email) || String.IsNullOrEmpty(clientDTO.Password) || String.IsNullOrEmpty(clientDTO.FirstName) || String.IsNullOrEmpty(clientDTO.LastName))
                    return StatusCode(403, "datos inválidos");

                Client user = _clientRepository.FindByEmail(clientDTO.Email);

                if (user != null)
                {
                    return StatusCode(403, "Email está en uso");
                }

                var newClient = _mapper.Map<Client>(clientDTO);
                

                _clientRepository.Save(newClient);

                // Llamamos al método Post del controlador de cuentas para asociar una cuenta al cliente
                _accountsController.Post(newClient.Id);

                return Created("", clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        public IActionResult PostAccounts()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                // Verificamos si el cliente ya tiene tres cuentas
                if (client.Accounts.Count > 2)
                {
                    return StatusCode(403, "El cliente ya tiene 3 cuentas, no puede tener más de 3");
                }

                // Llamamos al método Post del controlador de cuentas para crear una nueva cuenta para el cliente
                var account = _accountsController.Post(client.Id);

                // Si la creación de la cuenta falla, devolvemos un estado 500 junto con el mensaje de error
                if (account == null)
                {
                    return StatusCode(500, "Error al crear la cuenta");
                }

                return Created("", account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/cards")]
        public IActionResult PostCards([FromBody] CardDTO cardDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                {
                    return Unauthorized("Acceso no autorizado");
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Unauthorized("Acceso no autorizado");
                }

                // Realizar validaciones en los datos de la tarjeta
                if (string.IsNullOrWhiteSpace(cardDTO.Type) || string.IsNullOrWhiteSpace(cardDTO.Color))
                {
                    return BadRequest("Campos incompletos");
                }

                // Validar el tipo de la tarjeta
                if (!Enum.IsDefined(typeof(CardType), cardDTO.Type))
                {
                    return BadRequest("Tipo de tarjeta inválido. Debe ser DEBIT o CREDIT.");
                }

                // Validar el color de la tarjeta
                if (!Enum.IsDefined(typeof(CardColor), cardDTO.Color))
                {
                    return BadRequest("Color de tarjeta inválido. Debe ser SILVER, GOLD o TITANIUM.");
                }

                // Obtener el número actual de tarjetas del cliente por tipo y color
                int existingColorCardsCount = client.Cards.Where(c => c.Type == cardDTO.Type && c.Color == cardDTO.Color).Count();

                // Verificar si ya existe una tarjeta del mismo tipo y color
                if (existingColorCardsCount > 0)
                {
                    return BadRequest("El cliente ya tiene una tarjeta de este tipo y color.");
                }

                // Verificar el límite de tarjetas por tipo
                if (cardDTO.Type == CardType.CREDIT.ToString())
                {
                    if(client.Cards.Where(c => c.Type == CardType.CREDIT.ToString()).Count() > 2)
                    {
                        return BadRequest("El cliente ya tiene el máximo número de tarjetas de crédito.");

                    }
    
                }
                else if (cardDTO.Type == CardType.DEBIT.ToString())
                {
                    if (client.Cards.Where(c => c.Type == CardType.DEBIT.ToString()).Count() > 2)
                    {
                        return BadRequest("El cliente ya tiene el máximo número de tarjetas de debito.");

                    }
                }

                

                // Llamamos al método Post del controlador de tarjetas para crear una nueva tarjeta para el cliente
                IActionResult result = _cardsController.Post(new CardDTO
                {
                    ClientId = client.Id,
                    Type = cardDTO.Type,
                    Color = cardDTO.Color,
                    CardHolder = client.FirstName+" "+client.LastName,
                    Cvv = new Random().Next(100, 999),
                    FromDate = DateTime.UtcNow,
                    ThruDate = DateTime.UtcNow.AddYears(4),
                });

                // Si la creación de la tarjeta falla, devolvemos un estado 500 junto con el mensaje de error
                if (!(result is CreatedResult createdResult))
                {
                    return StatusCode(500, "Error al crear la tarjeta");
                }

                return Created(createdResult.Location, createdResult.Value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/accounts")] //para enviar las cuentas al front
        public IActionResult GetAccounts()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                {
                    return Unauthorized("Acceso no autorizado");
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Unauthorized("Acceso no autorizado");
                }

                // Obtén las cuentas asociadas al cliente desde el repositorio de cuentas.
                var accounts = _accountRepository.GetAccountsByClient(client.Id);

                var accountsDTO = _mapper.Map<List<AccountDTO>>(accounts);

                return Ok(accountsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }

        }

    }
    
}
