using MongoDB.Bson;

namespace handy_tarifvergleich_server.Models
{
    public class Offer
    {
        public ObjectId Id { get; set; }
        public int OfferId { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; }
        public string OfferURL { get; set; }
        public decimal BasePrice { get; set; }
        public bool WorldOffer { get; set; }
        public OfferCost Cost { get; set; }
        public OfferDeductions Deductions { get; set; }
        public decimal ActivationFee { get; set; }
    }
}
