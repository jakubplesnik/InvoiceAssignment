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
        /// Pay the specified invoice
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <response code="200">The request was processed successfully</response>
        /// <response code="400">Invoice was already paid</response>
        /// <response code="404">Invoice was not found</response>
        [HttpPost("{invoiceId}/pay")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PayInvoice(int invoiceId)
        {
            var invoice = _dbContext.GetInvoice(invoiceId);

            if (invoice == null)
            {
                return NotFound(new BaseResponse(404, "Invoice was not found."));
            }

            if (invoice.IsPaid)
            {
                return BadRequest(new BaseResponse(400, "Invoice is already paid."));
            }

            invoice.IsPaid = true;

            await _dbContext.SaveChangesAsync();

            return Ok(new BaseResponse(200, "Invoice was successfuly paid."));
        }

        /// <summary>
        /// Edits an invoice
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="patchDoc">JSON Patch Document</param>
        /// <response code="200">The request was processed successfully</response>
        /// <response code="400">Invalid model state or missing patch document</response>
        /// <response code="404">Invoice was not found</response>
        [HttpPatch("{invoiceId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> EditInvoice(int invoiceId, [FromBody]JsonPatchDocument<Invoice> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(new BaseResponse(400, "Missing JSON Patch document."));
            }
            
            var invoice = _dbContext.GetInvoice(invoiceId);
            if (invoice == null)
            {
                return NotFound(new BaseResponse(404, "Invoice was not found."));
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
    }
}