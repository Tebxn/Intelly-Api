namespace Intelly_Api.Entities
{
    public class UserEnt
    {
        public long User_Id { get; set; }
        public long User_Company_Id { get; set; }
        public string User_Name { get; set; }
        public string User_LastName { get; set; }
        public string User_Email { get;set;}
        public string User_Password { get; set; }
        public int User_Type { get; set;}
        public bool User_State { get; set; }
        public bool User_Password_IsTemp { get; set; }
        public string User_Type_Name { get; set; }
        public string User_Company_Name { get; set; }
    }
}
