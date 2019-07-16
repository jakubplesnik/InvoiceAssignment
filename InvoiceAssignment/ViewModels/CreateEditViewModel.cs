using InvoiceAssignment.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceAssignment.ViewModels
{
    public class CreateEditViewModel
    {
        public Invoice Invoice { get; set; }
        public InvoiceItem InvoiceItem { get; set; }
    }
}
