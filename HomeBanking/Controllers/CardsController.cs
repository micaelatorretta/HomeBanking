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

        public CardsController (ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var cards = _cardRepository.GetAllCards();
                var cardsDTO = new List<CardDTO>();

                foreach (Card card in cards)
                {
                    var newCardDTO = new CardDTO
                    {
                        Id = card.Id,
                        CardHolder = card.CardHolder,
                        Type = card.Type,
                        Color = card.Color,
                        Number = card.Number,
                        Cvv = card.Cvv,
                        FromDate = card.FromDate,
                        ThruDate = card.ThruDate,
                    };

                    cardsDTO.Add(newCardDTO);
                    

                }

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

                if (card == null)

                {

                    return NotFound(); //404

                }

                var newCardDTO = new CardDTO
                {
                    Id = card.Id,
                    CardHolder = card.CardHolder,
                    Type = card.Type,
                    Color = card.Color,
                    Number = card.Number,
                    Cvv = card.Cvv,
                    FromDate = card.FromDate,
                    ThruDate = card.ThruDate,
                };

                return Ok(newCardDTO);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post(Card newCard)
        {
            try
            {
                string cardNumber = _cardRepository.GenerateNextCardNumber();
                newCard.Number = cardNumber;

                _cardRepository.Save(newCard);
                CardDTO newCardDTO = new CardDTO
                {
                    Id = newCard.Id,
                    CardHolder = newCard.CardHolder,
                    Type = newCard.Type,
                    Color = newCard.Color,
                    Number = newCard.Number,
                    Cvv = newCard.Cvv,
                    FromDate = newCard.FromDate,
                    ThruDate = newCard.ThruDate,
                };
                return Created("", newCardDTO);
            }
            catch
            {
                return null;
            }
        }
    }
}
