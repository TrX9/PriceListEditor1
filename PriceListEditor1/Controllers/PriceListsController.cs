using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriceListEditor1.Data;
using PriceListEditor1.Models;

namespace PriceListEditor1.Controllers
{
    public class PriceListsController : Controller
    {
        private readonly PriceListContext _context;

        public PriceListsController(PriceListContext context)
        {
            _context = context;
        }

        // GET: PriceLists
        public async Task<IActionResult> Index()
        {
            return View(await _context.PriceLists.ToListAsync());
        }

        public IActionResult AddCustomColumn(int id)
        {
            var priceList = _context.PriceLists
                .Include(pl => pl.Columns)
                .FirstOrDefault(pl => pl.Id == id);

            return View(priceList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCustomColumn(int id, string columnName, ColumnType dataType)
        {
            var priceList = await _context.PriceLists
                .Include(pl => pl.Columns)
                .FirstOrDefaultAsync(pl => pl.Id == id);

            if (priceList == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                priceList.AddCustomColumn(columnName, dataType);
                _context.Update(priceList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(priceList);
        }

        // GET: PriceLists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PriceLists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] PriceList priceList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(priceList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(priceList);
        }

        // GET: PriceLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceList = await _context.PriceLists
                .Include(pl => pl.Columns)
                .Include(pl => pl.Products)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceList == null)
            {
                return NotFound();
            }

            return View(priceList);
        }

        // POST: PriceLists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] PriceList priceList)
        {
            if (id != priceList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(priceList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceListExists(priceList.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(priceList);
        }

        // GET: PriceLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceList = await _context.PriceLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceList == null)
            {
                return NotFound();
            }

            return View(priceList);
        }

        // POST: PriceLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var priceList = await _context.PriceLists.FindAsync(id);
            _context.PriceLists.Remove(priceList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PriceListExists(int id)
        {
            return _context.PriceLists.Any(e => e.Id == id);
        }

        // GET: PriceLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceList = await _context.PriceLists
                .Include(pl => pl.Columns)
                .Include(pl => pl.Products)
                .FirstOrDefaultAsync(pl => pl.Id == id);

            if (priceList == null)
            {
                return NotFound();
            }

            foreach (var product in priceList.Products)
            {
                if (product.DynamicColumns == null)
                {
                    product.DynamicColumns = new Dictionary<string, string>();
                }

                foreach (var column in priceList.Columns)
                {
                    if (!product.DynamicColumns.ContainsKey(column.Name))
                    {
                        product.DynamicColumns[column.Name] = "N/A"; // Default value for missing columns
                    }
                }
            }

            return View(priceList);
        }


        // Action to delete a column
        [HttpPost]
        public async Task<IActionResult> DeleteColumn(int columnId)
        {
            var column = await _context.Columns
                                       .Include(c => c.PriceList)
                                       .ThenInclude(pl => pl.Products)
                                       .FirstOrDefaultAsync(c => c.Id == columnId);

            if (column == null)
            {
                return NotFound();
            }

            if (column.HasData(column.PriceList.Products))
            {
                // Optionally, add a message to notify the user that the column cannot be deleted
                TempData["Error"] = "Cannot delete column because it contains data.";
                return RedirectToAction(nameof(Edit), new { id = column.PriceListId });
            }

            _context.Columns.Remove(column);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = column.PriceListId });
        }
    }
}
