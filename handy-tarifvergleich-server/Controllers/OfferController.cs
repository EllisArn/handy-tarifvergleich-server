using handy_tarifvergleich_server.Models;
using handy_tarifvergleich_server.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace handy_tarifvergleich_server.Controllers
{
    [ApiController]
    [Route("offers")]
    public class OfferController : Controller
    {
        private readonly IMongoCollection<BsonDocument> _offersCollection;

        public OfferController()
        {
            var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("IDPA");
            _offersCollection = database.GetCollection<BsonDocument>("offers");
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllOffers()
        {
            var allOffers = _offersCollection.Find(new BsonDocument()).ToList();
            var convertedOffers = allOffers.Select(offer => new Offer
                {
                    OfferId = offer["OfferId"].AsInt32,
                    Name = offer["Name"].AsString,
                    Provider = offer["Provider"].AsString,
                    OfferUrl = offer["OfferUrl"].AsString,
                    WorldOffer = offer["WorldOffer"].AsBoolean,
                    BasePrice = decimal.Parse(offer["BasePrice"].AsString),
                    Cost = new OfferCost
                    {
                        CallPerCallminuteCH = decimal.Parse(offer["Cost"]["CallPerCallminuteCH"].AsString),
                        InternetPerGBCH = decimal.Parse(offer["Cost"]["InternetPerGBCH"].AsString),
                        SMSPerCountCH = decimal.Parse(offer["Cost"]["SMSPerCountCH"].AsString),
                        CallPerCallminuteEurope = decimal.Parse(offer["Cost"]["CallPerCallminuteEurope"].AsString),
                        InternetPerGBEurope = decimal.Parse(offer["Cost"]["InternetPerGBEurope"].AsString),
                        SMSPerCountEurope = decimal.Parse(offer["Cost"]["SMSPerCountEurope"].AsString)
                    },
                    Deductions = new OfferDeductions
                    {
                        FreeGBInternetCH = decimal.Parse(offer["Deductions"]["FreeGBInternetCH"].AsString),
                        FreeCallminutesCH = decimal.Parse(offer["Deductions"]["FreeCallminutesCH"].AsString),
                        FreeSMSCH = decimal.Parse(offer["Deductions"]["FreeSMSCH"].AsString),
                        FreeGBInternetEurope = decimal.Parse(offer["Deductions"]["FreeGBInternetEurope"].AsString),
                        FreeCallminutesEurope = decimal.Parse(offer["Deductions"]["FreeCallminutesEurope"].AsString),
                        FreeSMSEurope = decimal.Parse(offer["Deductions"]["FreeSMSEurope"].AsString)
                    },
                    ActivationFee = decimal.Parse(offer["ActivationFee"].AsString)
                })
                .ToList();

            return Ok(convertedOffers);
        }

        [HttpGet]
        public IActionResult GetOffer(int offerId)
        {
            var offer = _offersCollection.Find(offer => offer["OfferId"] == offerId).FirstOrDefault();
            if (offer == null) return NotFound("Angebot nicht gefunden");

            var offerConverted = new Offer
            {
                OfferId = offer["OfferId"].AsInt32,
                Name = offer["Name"].AsString,
                Provider = offer["Provider"].AsString,
                OfferUrl = offer["OfferUrl"].AsString,
                WorldOffer = offer["WorldOffer"].AsBoolean,
                BasePrice = decimal.Parse(offer["BasePrice"].AsString),
                Cost = new OfferCost
                {
                    CallPerCallminuteCH = decimal.Parse(offer["Cost"]["CallPerCallminuteCH"].AsString),
                    InternetPerGBCH = decimal.Parse(offer["Cost"]["InternetPerGBCH"].AsString),
                    SMSPerCountCH = decimal.Parse(offer["Cost"]["SMSPerCountCH"].AsString),
                    CallPerCallminuteEurope = decimal.Parse(offer["Cost"]["CallPerCallminuteEurope"].AsString),
                    InternetPerGBEurope = decimal.Parse(offer["Cost"]["InternetPerGBEurope"].AsString),
                    SMSPerCountEurope = decimal.Parse(offer["Cost"]["SMSPerCountEurope"].AsString)
                },
                Deductions = new OfferDeductions
                {
                    FreeGBInternetCH = decimal.Parse(offer["Deductions"]["FreeGBInternetCH"].AsString),
                    FreeCallminutesCH = decimal.Parse(offer["Deductions"]["FreeCallminutesCH"].AsString),
                    FreeSMSCH = decimal.Parse(offer["Deductions"]["FreeSMSCH"].AsString),
                    FreeGBInternetEurope = decimal.Parse(offer["Deductions"]["FreeGBInternetEurope"].AsString),
                    FreeCallminutesEurope = decimal.Parse(offer["Deductions"]["FreeCallminutesEurope"].AsString),
                    FreeSMSEurope = decimal.Parse(offer["Deductions"]["FreeSMSEurope"].AsString)
                },
                ActivationFee = decimal.Parse(offer["ActivationFee"].AsString)
            };

            return Ok(offerConverted);
        }

        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddOffer(OfferDto request)
        {
            int offerId = Convert.ToInt32(_offersCollection.Find(new BsonDocument()).ToList().Last()["OfferId"]) + 1;
            var offer = new Offer
            {
                Id = ObjectId.GenerateNewId(),
                OfferId = offerId,
                Name = request.Name,
                Provider = request.Provider,
                OfferUrl = request.OfferUrl,
                BasePrice = request.BasePrice,
                WorldOffer = request.WorldOffer,
                Cost = new OfferCost
                {
                    CallPerCallminuteCH = request.Cost.CallPerCallminuteCH,
                    InternetPerGBCH = request.Cost.InternetPerGBCH,
                    SMSPerCountCH = request.Cost.SMSPerCountCH,
                    CallPerCallminuteEurope = request.Cost.CallPerCallminuteEurope,
                    InternetPerGBEurope = request.Cost.InternetPerGBEurope,
                    SMSPerCountEurope = request.Cost.SMSPerCountEurope,
                },
                Deductions = new OfferDeductions
                {
                    FreeGBInternetCH = request.Deductions.FreeGBInternetCH,
                    FreeCallminutesCH = request.Deductions.FreeCallminutesCH,
                    FreeSMSCH = request.Deductions.FreeSMSCH,
                    FreeGBInternetEurope = request.Deductions.FreeGBInternetEurope,
                    FreeCallminutesEurope = request.Deductions.FreeCallminutesEurope,
                    FreeSMSEurope = request.Deductions.FreeSMSEurope,
                },
                ActivationFee = request.ActivationFee
            };

            _offersCollection.InsertOne(offer.ToBsonDocument());

            return Ok("Angebot erfolgreich hinzugefügt");
        }

        [HttpPut]
        [Route("update")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateOffer(int offerId, OfferDto request)
        {
            var offer = _offersCollection.Find(offer => offer["OfferId"] == offerId).FirstOrDefault();
            if (offer == null) return NotFound("Angebot nicht gefunden");

            var update = Builders<BsonDocument>.Update
                .Set("Name", request.Name)
                .Set("Provider", request.Provider)
                .Set("OfferUrl", request.OfferUrl)
                .Set("BasePrice", request.BasePrice)
                .Set("WorldOffer", request.WorldOffer)
                .Set("Cost.CallPerCallminuteCH", request.Cost.CallPerCallminuteCH)
                .Set("Cost.InternetPerGBCH", request.Cost.InternetPerGBCH)
                .Set("Cost.SMSPerCountCH", request.Cost.SMSPerCountCH)
                .Set("Cost.CallPerCallminuteEurope", request.Cost.CallPerCallminuteEurope)
                .Set("Cost.InternetPerGBEurope", request.Cost.InternetPerGBEurope)
                .Set("Cost.SMSPerCountEurope", request.Cost.SMSPerCountEurope)
                .Set("Deductions.FreeGBInternetCH", request.Deductions.FreeGBInternetCH)
                .Set("Deductions.FreeCallminutesCH", request.Deductions.FreeCallminutesCH)
                .Set("Deductions.FreeSMSCH", request.Deductions.FreeSMSCH)
                .Set("Deductions.FreeGBInternetEurope", request.Deductions.FreeGBInternetEurope)
                .Set("Deductions.FreeCallminutesEurope", request.Deductions.FreeCallminutesEurope)
                .Set("Deductions.FreeSMSEurope", request.Deductions.FreeSMSEurope)
                .Set("ActivationFee", request.ActivationFee);

            _offersCollection.UpdateOne(offer, update);

            return Ok("Angebot erfolgreich aktualisiert");
        }

        [HttpDelete]
        [Route("delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteOffer(int offerId)
        {
            var offer = _offersCollection.Find(offer => offer["OfferId"] == offerId).FirstOrDefault();
            if (offer == null) return NotFound("Angebot nicht gefunden");

            _offersCollection.DeleteOne(offer);

            return Ok("Angebot erfolgreich gelöscht");
        }
    }
}
