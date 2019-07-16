using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceAssignment.DAL.Models
{
    //[Bind("Id,RefNumber,CreationDate,DueDate,IsPaid,Supplier,Recipient,InvoiceItems")]
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Number")]
        [MaxLength(14)]
        public string RefNumber { get; set; }
        [Display(Name = "Created")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime CreationDate { get; set; }
        [Display(Name = "Due")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime DueDate { get; set; }
        [Display(Name = "Paid")]
        public bool IsPaid { get; set; }
        public virtual Subject Recipient { get; set; }
        public virtual Subject Supplier { get; set; }
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}
