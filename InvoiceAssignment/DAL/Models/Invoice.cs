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
        [MaxLength(14)]
        public string RefNumber { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public virtual Subject Recipient { get; set; }
        public virtual Subject Supplier { get; set; }
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}
