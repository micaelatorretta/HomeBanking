using HomeBanking.DTOs;
using HomeBanking.Models;
using HomeBanking.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using AutoMapper;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public AccountsController(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }



        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var accounts = _accountRepository.GetAllAccounts();

                var accountsDTO = _mapper.Map<List<AccountDTO>>(accounts);

                return Ok(accountsDTO);
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
                var account = _accountRepository.FindById(id);

                if (account == null)
                {
                    return NotFound(); //404
                }

                var accountDTO = _mapper.Map<AccountDTO>(account);

                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpPost]
        public IActionResult Post(long clientId)
        {
            try
            {
                string accountNumber= _accountRepository.GenerateNextAccountNumber();
                Account newAccount = new Account
                {
                    ClientId = clientId,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    Number = accountNumber

                };
                _accountRepository.Save(newAccount);

                AccountDTO newAccDTO=_mapper.Map<AccountDTO>(newAccount);

                return Created ("", newAccDTO);
            }
            catch
            {
                return null;
            }
        }
    }
}
