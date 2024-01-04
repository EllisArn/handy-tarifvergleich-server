namespace handy_tarifvergleich_server.Models.Dto
{
    public class OfferDto
    {
        public string Name { get; set; }
        public string Provider { get; set; }
        public string OfferURL { get; set; }
        public decimal BasePrice { get; set; }
        public bool WorldOffer { get; set; }
        public OfferCostDto Cost { get; set; }
        public OfferDeductionsDto Deductions { get; set; }
        public decimal  ActivationFee { get; set; }
    }
}
