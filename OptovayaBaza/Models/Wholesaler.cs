using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OptovayaBaza.Models
{
    public class Wholesaler
    {
        [Key]
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string INN { get; set; }
        public string Phone { get; set; }
        public string LegalAddress { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}