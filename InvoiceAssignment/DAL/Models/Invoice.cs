using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceAssignment.DAL.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        public string RefNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public virtual Subject Recipient { get; set; }
        public virtual Subject Supplier { get; set; }
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}
