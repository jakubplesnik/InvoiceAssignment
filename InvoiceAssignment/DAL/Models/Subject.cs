using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceAssignment.DAL.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        public string Cin { get; set; }
        public string Vat { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        //[InverseProperty("Supplier")]
        //public virtual ICollection<Invoice> SupplierInvoices { get; set; }
        //[InverseProperty("Recipient")]
        //public virtual ICollection<Invoice> RecipientInvoices { get; set; }
    }
}
