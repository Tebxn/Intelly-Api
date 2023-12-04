namespace Intelly_Api.Entities
{
    public class ChartEnt
    {
        //For ChartSumTotalByMonthActualYear
        public int? MonthNumber { get; set; }
        public string? MonthName { get; set; } = String.Empty;
        public float? Total { get; set; }

        //For ChartTopCampaignsByTotal
        public string? Sell_MarketingCampaignId { get; set; } = String.Empty;
        public string? MarketingCampaign_Name { get; set; } = String.Empty;
    }
}
