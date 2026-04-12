using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OptovayaBaza.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string OrganizationName { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
    }
}