using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoiceAssignment.DAL;
using InvoiceAssignment.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InvoiceAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceDbContext _dbContext;
        private readonly ILogger _logger;

        public InvoicesController(InvoiceDbContext dbContext, ILogger<InvoicesController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public IActionResult GetInvoice(int id)
        {
            var invoice = _dbContext.GetInvoice(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }

        [HttpGet]
        public IActionResult GetUnpaidInvoices()
        {
            var result = _dbContext.GetAllInvoices()
                .Where(i => !i.IsPaid);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddInvoice([FromBody]Invoice invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            invoice.Id = 0;
            invoice.CreationDate = DateTime.UtcNow;
            invoice.RefNumber = DateTime.Now.ToString("yyyyMMddHHmmss");
            invoice.IsPaid = false;

            _dbContext.Invoices.Add(invoice);

            //if (invoice.InvoiceItems.Count > 0)
            //{
            //    foreach (var item in invoice.InvoiceItems)
            //    {
            //        item.Invoice = invoice;
            //        _dbContext.InvoiceItems.Add(item);
            //    }
            //}

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("{id}/pay")]
        public IActionResult PayInvoice(int id)
        {
            var invoice = _dbContext.GetInvoice(id);

            if (invoice == null)
            {
                return NotFound();
            }

            if (invoice.IsPaid)
            {
                return BadRequest();
            }

            invoice.IsPaid = true;

            // update and save changes
            _dbContext.Invoices.Update(invoice);
            _dbContext.SaveChanges();

            return Ok(invoice);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _dbContext.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            _dbContext.Invoices.Remove(invoice);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}