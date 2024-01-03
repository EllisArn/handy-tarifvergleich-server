namespace handy_tarifvergleich_server.Models.Dto
{
    public class OfferDto
    {
        public string Name { get; set; }
        public string Provider { get; set; }
        public string OfferURL { get; set; }
        public double BasePrice { get; set; }
        public bool WorldOffer { get; set; }
        public OfferCostDto Cost { get; set; }
        public OfferDeductionsDto Deductions { get; set; }
        public int ActivationFee { get; set; }
    }
}
