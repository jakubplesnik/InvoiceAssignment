using InvoiceAssignment.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceAssignment.ViewModels
{
    public class InvoiceViewModel
    {
        public int Id { get; set; }
        public string RefNumber { get; set; }
        public string CreationDate { get; set; }
        public string DueDate { get; set; }
        public bool IsPaid { get; set; }
        public Subject Recipient { get; set; }
        public Subject Supplier { get; set; }
        public IEnumerable<InvoiceItemViewModel> InvoiceItems { get; set; }

        public InvoiceViewModel(Invoice invoice)
        {
            Id = invoice.Id;
            RefNumber = invoice.RefNumber;
            CreationDate = invoice.CreationDate.ToString("O");
            DueDate = invoice.DueDate.ToString("O");
            IsPaid = invoice.IsPaid;
            Recipient = invoice.Recipient;
            Supplier = invoice.Supplier;
            InvoiceItems = invoice.InvoiceItems.Select(i => new InvoiceItemViewModel(i));
        }
    }
}
