using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriceListEditor1.Data;
using PriceListEditor1.Models;

namespace PriceListEditor1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly PriceListContext _context;

        public ProductsController(PriceListContext context)
        {
            _context = context;
        }

        // GET: Products/Create
        public IActionResult Create(int priceListId)
        {
            var priceList = _context.PriceLists.Include(pl => pl.Columns).FirstOrDefault(pl => pl.Id == priceListId);
            if (priceList == null)
            {
                return NotFound();
            }
            ViewBag.PriceList = priceList;
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int priceListId, Product product, Dictionary<string, string> dynamicColumns)
        {
            var priceList = _context.PriceLists.Include(pl => pl.Columns).FirstOrDefault(pl => pl.Id == priceListId);
            if (priceList == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                product.PriceListId = priceListId;
                product.DynamicColumns = dynamicColumns;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "PriceLists", new { id = priceListId });
            }

            ViewBag.PriceList = priceList;
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.PriceList)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "PriceLists", new { id = product.PriceListId });
        }
    }
}
