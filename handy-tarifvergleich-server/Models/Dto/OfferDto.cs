namespace handy_tarifvergleich_server.Models.Dto
{
    public class OfferDto
    {
        public string Name { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string OfferUrl { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public bool WorldOffer { get; set; }
        public OfferCostDto Cost { get; set; } = new OfferCostDto();
        public OfferDeductionsDto Deductions { get; set; } = new OfferDeductionsDto();
        public decimal  ActivationFee { get; set; }
    }
}
