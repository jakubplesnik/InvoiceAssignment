using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using InvoiceAssignment.Auth;
using InvoiceAssignment.DAL;
using InvoiceAssignment.DAL.Models;
using InvoiceAssignment.Domain.Communication;
using InvoiceAssignment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InvoiceAssignment.Controllers
{
    [Authorize(Policy = Policies.UsersOnly)]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceDbContext _dbContext;

        public InvoicesController(InvoiceDbContext dbContext, ILogger<InvoicesController> logger)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Returns unpaid invoices
        /// </summary>
        /// <response code="200">The request was processed successfully</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult GetUnpaidInvoices()
        {
            var unpaidInvoices = _dbContext.GetAllInvoices()
                .Where(i => !i.IsPaid);

            var result = unpaidInvoices.Select(i => new InvoiceViewModel(i));

            return Ok(result);
        }

        /// <summary>
        /// Returns the specified invoice
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <response code="200">The request was processed successfully</response>
        /// <response code="404">Invoice was not found</response>
        [HttpGet("{invoiceId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetInvoice(int invoiceId)
        {
            var invoice = _dbContext.GetInvoice(invoiceId);

            if (invoice == null)
            {
                return NotFound(new BaseResponse(404, "Invoice was not found."));
            }

            var result = new InvoiceViewModel(invoice);

            return Ok(result);
        }

        /// <summary>
        /// Adds a new invoice
        /// </summary>
        /// <param name="inv">Invoice body</param>
        /// <response code="200">The request was processed successfully</response>
        /// <response code="404">Invalid model state</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddInvoice([FromBody]Invoice inv)
        {
            inv.Id = 0;
            inv.CreationDate = DateTime.UtcNow;
            inv.RefNumber = DateTime.Now.ToString("yyyyMMddHHmmss");
            inv.IsPaid = false;
            foreach (var item in inv.InvoiceItems)
            {
                item.Invoice = inv;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _dbContext.Invoices.AddAsync(inv);
            await _dbContext.SaveChangesAsync();

            var result = new InvoiceViewModel(inv);

            return Ok(result);
        }

        [HttpPatch("{invoiceId}")]
        public async Task<IActionResult> EditInvoice(int invoiceId, [FromBody]JsonPatchDocument<Invoice> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }
            
            var invoice = _dbContext.GetInvoice(invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            patchDoc.ApplyTo(invoice, ModelState);

            var isValid = TryValidateModel(invoice);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            await _dbContext.SaveChangesAsync();

            var result = new InvoiceViewModel(invoice);

            return Ok(result);
        }

        [HttpPatch("{invoiceId}/pay")]
        public async Task<IActionResult> PayInvoice(int invoiceId)
        {
            var invoice = _dbContext.GetInvoice(invoiceId);

            if (invoice == null)
            {
                return NotFound(new BaseResponse(404, "Invoice was not found."));
            }

            if (invoice.IsPaid)
            {
                return BadRequest(new BaseResponse(400, "Invoice already paid."));
            }

            invoice.IsPaid = true;

            await _dbContext.SaveChangesAsync();

            var result = new InvoiceViewModel(invoice);

            return Ok(result);
        }

        [HttpPost("{invoiceId}/items")]
        public async Task<IActionResult> AddInvoiceItem(int invoiceId, [FromBody]InvoiceItem item)
        {
            var invoice = await _dbContext.Invoices.FindAsync(invoiceId);

            if (invoice == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            item.Invoice = invoice;

            await _dbContext.InvoiceItems.AddAsync(item);
            await _dbContext.SaveChangesAsync();

            var result = new InvoiceItemViewModel(item);

            return Ok(result);
        }

        [HttpDelete("{invoiceId}/items/{itemId}")]
        public async Task<IActionResult> RemoveInvoiceItem(int invoiceId, int itemId)
        {
            var invoice = await _dbContext.Invoices.FindAsync(invoiceId);

            if (invoice == null)
            {
                return NotFound(new BaseResponse(404, "Invoice was not found."));
            }

            var item = await _dbContext.InvoiceItems.FindAsync(itemId);

            if (item == null || item.Invoice != invoice)
            {
                return NotFound(new BaseResponse(404, "Item of specified invoice was not found."));
            }

            _dbContext.InvoiceItems.Remove(item);
            await _dbContext.SaveChangesAsync();

            return Ok(new BaseResponse(200, "Item was successfuly removed."));
        }
    }
}