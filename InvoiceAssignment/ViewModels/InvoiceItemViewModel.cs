using InvoiceAssignment.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceAssignment.ViewModels
{
    public class InvoiceItemViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public decimal Price { get; set; }

        public InvoiceItemViewModel(InvoiceItem item)
        {
            Id = item.Id;
            Text = item.Text;
            Price = item.Price;
        }
    }
}
