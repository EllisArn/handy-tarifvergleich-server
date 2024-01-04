using MongoDB.Bson;

namespace handy_tarifvergleich_server.Models
{
    public class Offer
    {
        public ObjectId Id { get; set; }
        public int OfferId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string OfferUrl { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public bool WorldOffer { get; set; }
        public OfferCost Cost { get; set; } = new OfferCost();
        public OfferDeductions Deductions { get; set; } = new OfferDeductions();
        public decimal ActivationFee { get; set; }
    }
}
