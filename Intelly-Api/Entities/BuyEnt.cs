namespace Intelly_Api.Entities
{
    public class BuyEnt
    {
        public long BuyId { get; set; }
        public long CustomerId { get; set; }
        public long CompanyId { get; set; }
        public long LocalId { get; set; }
        public DateTime BuyDate { get; set; }
        public long MarketingCampaignId { get; set; }
        public List<ProductEnt> Products { get; set; } //list of products
        

    }
}
