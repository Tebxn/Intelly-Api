namespace Intelly_Api.Entities
{
    public class MarketingCampaignEnt
    {
        public long MarketingCampaign_Id { get; set; }
        public long MarketingCampaign_CompanyId { get; set; }
        public string? MarketingCampaign_Internal_Code { get; set; } = string.Empty;
        public string? MarketingCampaign_Name { get; set; } = string.Empty;
        public DateTime? MarketingCampaign_Start_Date { get; set; }
        public DateTime? MarketingCampaign_End_Date { get; set; }
        public int? MarketingCampaign_MembershipLevel { get; set; }
        public string? Membership_Name { get; set; } = string.Empty;
        public EmailEnt? Email { get; set; }
    }
}
