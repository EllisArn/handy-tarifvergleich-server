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
            var jsonOffers = allOffers.Select(offer => offer.ToJson()).ToList();
            return Ok(jsonOffers);
        }

        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddOffer(OfferDto request)
        {
            var offer = new Offer
            {
                Id = ObjectId.GenerateNewId(),
                OfferId = Convert.ToInt32(_offersCollection.CountDocuments(new BsonDocument())) + 1,
                Name = request.Name,
                Provider = request.Provider,
                OfferURL = request.OfferURL,
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
    }
}
