using System.ComponentModel.DataAnnotations;

namespace UserAPI.WebAPI.Dtos
{
    public class UsersDto
    {
        public string Name {get; set;}
        public int Phone { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string BirthDate { get; set; }
        public int TypeUsers {get; set; }        
    }
}