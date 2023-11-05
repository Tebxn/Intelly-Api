namespace Intelly_Api.Entities
{
    public class ProductEnt
    {
        public long Product_Id { get; set; }
        public long Product_CompanyId { get; set; }
        public string? Product_Internal_Code { get; set; } = string.Empty;
        public string? Product_Name { get; set; } = string.Empty;
        public float Product_Price { get; set; }
        public string? Product_ImageUrl { get; set; } = string.Empty;
    }
}
