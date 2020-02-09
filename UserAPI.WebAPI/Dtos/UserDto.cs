namespace UserAPI.WebAPI.Dtos
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Fullname { get; set; }
        public string BirthDate { get; set; }
        
        public int Phone { get; set; }
    }
}