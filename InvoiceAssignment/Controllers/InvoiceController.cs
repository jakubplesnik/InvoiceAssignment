using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InvoiceAssignment.DAL;
using InvoiceAssignment.DAL.Models;
using InvoiceAssignment.ViewModels;

namespace InvoiceAssignment.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly InvoiceDbContext _dbContext;

        public InvoiceController(InvoiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Invoice
        public async Task<IActionResult> Index()
        {
            return View(await _dbContext.GetAllInvoicesAsync());
        }

        // GET: Invoice/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _dbContext.GetInvoiceAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoice/Create
        public IActionResult Create(int id)
        {
            return View(new Invoice()
            {
                Supplier = new Subject()
                {
                    Name = "Test",
                    Address = "Test",
                    Crn = "12345678",
                    Vat = "CZ12345678"
                },
                Recipient = new Subject()
                {
                    Name = "Test",
                    Address = "Test",
                    Crn = "12345678",
                    Vat = "CZ12345678"
                },
                DueDate = DateTime.Now,
                InvoiceItems = new List<InvoiceItem>()
                {
                    new InvoiceItem()
                }
            });
        }

        // POST: Invoice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Id,RefNumber,CreationDate,DueDate,IsPaid,Supplier,Recipient,InvoiceItems")] */Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                invoice.CreationDate = DateTime.UtcNow;
                invoice.RefNumber = DateTime.Now.ToString("yyyyMMddHHmmss");
                invoice.IsPaid = false;

                _dbContext.Add(invoice);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(invoice);
        }

        // GET: Invoice/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _dbContext.GetInvoiceAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            if (invoice.IsPaid)
            {
                return BadRequest("Cannot edit invoice. Invoice is already paid.");
            }

            return View(new CreateEditViewModel()
            {
                Invoice = invoice
                //InvoiceItem =  new InvoiceItem()
                //{
                //    Invoice = invoice
                //}
            });
        }

        // POST: Invoice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Invoice")] CreateEditViewModel model)
        {
            if (id != model.Invoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_dbContext.Entry(model.Invoice.Supplier).Properties.Any(s => s.IsModified))
                    {
                        _dbContext.Update(model.Invoice.Supplier);
                    }

                    if (_dbContext.Entry(model.Invoice.Recipient).Properties.Any(r => r.IsModified))
                    {
                        _dbContext.Update(model.Invoice.Recipient);
                    }

                    _dbContext.Update(model.Invoice);

                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(model.Invoice.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = model.Invoice.Id });
            }
            return View(model.Invoice);
        }

        //public async Task<IActionResult> AddItem(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var invoice = await _dbContext.GetInvoiceAsync(id);
        //    if (invoice == null)
        //    {
        //        return NotFound();
        //    }
        //    if (invoice.IsPaid)
        //    {
        //        return BadRequest("Cannot add item to invoice. Invoice is already paid.");
        //    }

        //    return View(new CreateEditViewModel()
        //    {
        //        Invoice = invoice,
        //        InvoiceItem = new InvoiceItem()
        //        {
        //            Invoice = invoice
        //        }
        //    });
        //}

        [HttpPost, ActionName("AddItem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem([Bind("InvoiceItem")] CreateEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var invoice = await _dbContext.Invoices.FindAsync(model.InvoiceItem.InvoiceId);
            if (invoice == null)
            {
                return NotFound($"Invoice {model.InvoiceItem.InvoiceId} not found.");
            }

            var item = new InvoiceItem()
            {
                Text = model.InvoiceItem.Text,
                Price = model.InvoiceItem.Price,
                Invoice = invoice
            };

            await _dbContext.InvoiceItems.AddAsync(item);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = invoice.Id });
        }

        public async Task<IActionResult> DeleteItem(int? invoiceId, int? itemId)
        {
            if (invoiceId == null || itemId == null)
            {
                return NotFound();
            }

            var invoice = await _dbContext.Invoices.FindAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            var item = await _dbContext.InvoiceItems.FindAsync(itemId);
            if (item == null || item.Invoice != invoice)
            {
                return NotFound();
            }

            _dbContext.InvoiceItems.Remove(item);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = invoiceId });
        }

        private bool InvoiceExists(int id)
        {
            return _dbContext.Invoices.Any(e => e.Id == id);
        }
    }
}
