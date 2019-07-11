using InvoiceAssignment.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceAssignment.DAL
{
    public class InvoiceDbContext : DbContext
    {
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options)
            : base(options) { }

        public IEnumerable<Invoice> GetAllInvoices()
        {
            return Invoices
                .Include(i => i.Recipient)
                .Include(i => i.Supplier)
                .Include(i => i.InvoiceItems);
        }

        public Invoice GetInvoice(int id)
        {
            return Invoices
                .Include(i => i.Recipient)
                .Include(i => i.Supplier)
                .Include(i => i.InvoiceItems)
                .SingleOrDefault(i => i.Id == id);
        }
    }
}
