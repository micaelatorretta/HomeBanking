using AutoMapper;
using HomeBanking.DTOs;
using HomeBanking.Models;
using HomeBanking.Repositories;
using HomeBanking.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private ICardRepository _cardRepository; 
        private readonly IMapper _mapper;

        public CardsController (ICardRepository cardRepository, IMapper mapper)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var cards = _cardRepository.GetAllCards();
                var cardsDTO = _mapper.Map<List<ClientDTO>>(cards); 

                return Ok(cardsDTO);
            }

            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]

        public IActionResult Get(long id)
        {
            try
            {
                var card = _cardRepository.FindById(id);

                if (card is null)
                {
                    return NotFound(); //404
                }
                var newCardDTO = _mapper.Map<CardDTO>(card);

                return Ok(newCardDTO);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post(CardDTO cardDTO)
        {
            try
            {
                string cardNumber = _cardRepository.GenerateNextCardNumber();
                cardDTO.Number = cardNumber;

                var newCard = _mapper.Map<Card>(cardDTO);

                _cardRepository.Save(newCard);

                var newCardDTO = _mapper.Map<CardDTO>(newCard);

                return Created("", newCardDTO);
            }
            catch
            {
                return null;
            }
        }
    }
}
