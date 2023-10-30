using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scridon_Grigore_Lab2.Data;
using Scridon_Grigore_Lab2.Models;

namespace Scridon_Grigore_Lab2.Controllers
{ 

  
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

      

        // GET: Books
        // Corrected to have only one method signature. 
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString,int? pageNumber)

        {
            // Set up your ViewData parameters for sorting and searching. 
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            // Start with a base query from which to apply sorting and filtering.
            var books = from b in _context.Books
                        select b;

            // If there's a search string, filter the books based on it.
            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString));
            }

            // Apply sorting based on the sortOrder parameter.
            switch (sortOrder)
            {
                case "title_desc":
                    books = books.OrderByDescending(b => b.Title);
                    break;
                case "Price":
                    books = books.OrderBy(b => b.Price); // by Price in ascending order here.
                    break;
                case "price_desc":
                    books = books.OrderByDescending(b => b.Price); // This handles the descending order case.
                    break;
                default:
                    books = books.OrderBy(b => b.Title);
                    break;

                    
            }
            int pageSize = 2;
            // This line was previously inside the switch; it should be here:
            return View(await PaginatedList<Book>.CreateAsync(books.AsNoTracking(), pageNumber ?? 1, pageSize));

            // Execute the query and return the results as a list.
            return View(await books.AsNoTracking().ToListAsync()); // AsNoTracking is used here for read-only operations to improve performance.
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        { 

            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(s => s.Orders)
                .ThenInclude(e => e.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
       
       
        // POST: Books/Create 

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["AuthorID"] = new SelectList(_context.Authors, "ID", "FullName"); // asigurați-vă că "FullName" este o proprietate care combină prenumele și numele de familie
            return View();
        }

        
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("Title,Author,Price")] Book book)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        _context.Add(book);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateException /* ex*/)
                {

                    ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists ");
                }
                return View(book);
            }


            // GET: Books/Edit/5
            public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookToUpdate = await _context.Books.FirstOrDefaultAsync(s => s.ID == id);

            if (bookToUpdate == null)
            {
                return NotFound();
            }

            // This code segment ensures only the specified fields are updated, preventing overposting attacks.
            if (await TryUpdateModelAsync<Book>(
                bookToUpdate,
                "",
                s => s.Title, s => s.Author, s => s.Price)) // specify the properties you want to include in the model binding
            {
                try
                {
                    await _context.SaveChangesAsync(); // no need for _context.Update(book); Entity Framework tracks changes automatically
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    // Log the error (uncomment the ex variable and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                                                "Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(bookToUpdate); // return the updated model to the view
        }


        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (book == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed. Try again, and if the problem persists, see your system administrator.";
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Try to find the book in the database
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                // Book not found - handle according to your application's requirements
                // Option: You can display a not found page or a custom message to the user.
                return NotFound(); // Or return RedirectToAction with a custom message
            }

            try
            {
                // Attempt to remove the book from the database
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                // Redirect to the Index page on successful delete
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex) // 'ex' can be used for logging
            {
                // Log the exception (depending on your logging strategy/framework)
                // Uncomment and modify the following line according to your logging mechanism
                // _logger.LogError(ex, "An error occurred while deleting book with ID {ID}.", id);

                // Redirect to the Delete page and specify that an error occurred
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }


        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
