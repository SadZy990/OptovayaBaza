using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OptovayaBaza.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; }
        public string Comment { get; set; }
        public int MaterialId { get; set; }
        public int? WholesalerId { get; set; }
        public int? AdminId { get; set; }

        public virtual Material Material { get; set; }
        public virtual User Wholesaler { get; set; }
        public virtual User Admin { get; set; }
    }
}