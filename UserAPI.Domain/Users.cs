using System;
using UserAPI.Domain;

namespace UserAPI.Domain
{
    public class Users
    {
        public int Id { get; set; }
        public string Name {get; set;}
        public int Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public TypeUsers TypeUsers {get; set; }
    }
}