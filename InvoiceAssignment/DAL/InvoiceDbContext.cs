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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Invoice>()
                .Property(e => e.RefNumber)
                .IsRequired();
            builder.Entity<Invoice>()
                .HasOne(e => e.Supplier)
                .WithMany()
                .HasForeignKey("SupplierId");
            builder.Entity<Invoice>()
                .HasOne(e => e.Recipient)
                .WithMany()
                .HasForeignKey("RecipientId");

            builder.Entity<InvoiceItem>()
                .HasOne(e => e.Invoice)
                .WithMany(i => i.InvoiceItems)
                .HasForeignKey("InvoiceId");
        }

        public IEnumerable<Invoice> GetAllInvoices()
        {
            return Invoices
                .Include(i => i.Recipient)
                .Include(i => i.Supplier)
                .Include(i => i.InvoiceItems);
        }

        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            return await Invoices
                .Include(i => i.Recipient)
                .Include(i => i.Supplier)
                .Include(i => i.InvoiceItems)
                .ToListAsync();
        }

        public Invoice GetInvoice(int id)
        {
            return Invoices
                .Include(i => i.Recipient)
                .Include(i => i.Supplier)
                .Include(i => i.InvoiceItems)
                .FirstOrDefault(i => i.Id == id);
        }

        public async Task<Invoice> GetInvoiceAsync(int? id)
        {
            return await Invoices
                .Include(i => i.Recipient)
                .Include(i => i.Supplier)
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}
