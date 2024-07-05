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

        // GET: PriceLists/AddColumn/5
        public IActionResult AddColumn(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceList = _context.PriceLists.Include(p => p.Columns).FirstOrDefault(p => p.Id == id);
            if (priceList == null)
            {
                return NotFound();
            }

            ViewBag.PriceListId = id;
            return View();
        }

        // POST: PriceLists/AddColumn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddColumn(int id, [Bind("Name,DataType")] Column column)
        {
            if (ModelState.IsValid)
            {
                var priceList = _context.PriceLists.Include(p => p.Columns).FirstOrDefault(p => p.Id == id);
                if (priceList == null)
                {
                    return NotFound();
                }

                column.PriceListId = id;
                column.IsCustom = true; 
                priceList.Columns.Add(column);

                _context.Update(priceList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id });
            }

            ViewBag.PriceListId = id;
            return View(column);
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
                                          .FirstOrDefaultAsync(pl => pl.Id == id);
            if (priceList == null)
            {
                return NotFound();
            }
            return View(priceList);
        }

        // POST: PriceLists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Columns")] PriceList priceList)
        {
            if (id != priceList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPriceList = await _context.PriceLists
                                                          .Include(pl => pl.Columns)
                                                          .FirstOrDefaultAsync(pl => pl.Id == id);

                    if (existingPriceList == null)
                    {
                        return NotFound();
                    }

                    existingPriceList.Name = priceList.Name;

                    foreach (var column in existingPriceList.Columns)
                    {
                        var updatedColumn = priceList.Columns.FirstOrDefault(c => c.Id == column.Id);
                        if (updatedColumn != null)
                        {
                            column.Name = updatedColumn.Name;
                        }
                    }

                    _context.Update(existingPriceList);
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

        private bool PriceListExistsById(int id)
        {
            return _context.PriceLists.Any(e => e.Id == id);
        }

        // GET: PriceLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceList = await _context.PriceLists
                .Include(p => p.Columns)
                .Include(p => p.Products)
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
            var priceList = await _context.PriceLists
                .Include(p => p.Columns)
                .Include(p => p.Products)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (priceList != null)
            {
                _context.Columns.RemoveRange(priceList.Columns);
                _context.Products.RemoveRange(priceList.Products);
                _context.PriceLists.Remove(priceList);
                await _context.SaveChangesAsync();
            }

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
                        product.DynamicColumns[column.Name] = "N/A"; 
                    }
                }
            }

            return View(priceList);
        }

        // POST: PriceLists/DeleteColumn/5
        [HttpPost, ActionName("DeleteColumn")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteColumnConfirmed(int id)
        {
            var column = await _context.Columns.FindAsync(id);
            if (column != null)
            {
                var productsWithColumnData = _context.Products.Where(p => p.DynamicColumns.ContainsKey(column.Name)).ToList();
                if (productsWithColumnData.Any())
                {
                    ModelState.AddModelError("", "Cannot delete column with existing data.");
                    return RedirectToAction(nameof(Edit), new { id = column.PriceListId });
                }

                _context.Columns.Remove(column);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), new { id = column.PriceListId });
        }

        // GET: PriceLists/DeleteColumn/5
        public async Task<IActionResult> DeleteColumn(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var column = await _context.Columns.FindAsync(id);
            if (column == null)
            {
                return NotFound();
            }

            var productsWithColumnData = await _context.Products.ToListAsync();
            if (productsWithColumnData.Any(p => p.DynamicColumns.ContainsKey(column.Name)))
            {
                ModelState.AddModelError("", "Cannot delete column with existing data.");
                return RedirectToAction(nameof(Edit), new { id = column.PriceListId });
            }

            _context.Columns.Remove(column);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = column.PriceListId });
        }
    }

}

