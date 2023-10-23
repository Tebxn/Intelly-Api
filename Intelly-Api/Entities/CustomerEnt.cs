namespace Intelly_Api.Entities
{
    public class CustomerEnt
    {
       public long Customer_Id { get; set; }
        public long? Customer_Company_Id { get; set; }
        public string? Customer_Name { get; set; }
        public string? Customer_FirstLastname { get; set; }
        public string? Customer_SecondLastname { get; set; }
        public string? Customer_Email { get; set; }
        public long? Customer_Membership_Level { get; set; }
        public bool? Customer_Allow_Mails { get; set; }
        public bool? Customer_Status { get; set; }

    }
}
