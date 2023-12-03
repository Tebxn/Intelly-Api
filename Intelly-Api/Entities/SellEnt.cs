namespace Intelly_Api.Entities
{
    public class SellEnt
    {
        public long SellId { get; set; }
        public string NumFactura { get; set; } = string.Empty;
        public long? CustomerId { get; set; }
        public long CompanyId { get; set; }
        public long LocalId { get; set; }
        public DateTime SellDate { get; set; }
        public long? MarketingCampaignId { get; set; }
        public float Total { get; set; }
        public List<ProductEnt>? Products { get; set; } //list of products
        

    }
}
