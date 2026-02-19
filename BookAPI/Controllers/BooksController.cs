using BookAPI.Models;
using BookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;

        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        // Helper method to validate ISBN
        private (bool isValid, string errorMessage) ValidateISBN(string isbn)
        {
            // Check if empty
            if (string.IsNullOrWhiteSpace(isbn))
            {
                return (false, "ISBN is required");
            }

            // Remove hyphens for validation
            var cleanISBN = isbn.Replace("-", "");

            // Check if only digits
            if (!cleanISBN.All(char.IsDigit))
            {
                return (false, "ISBN must contain only numbers and optional hyphens");
            }

            // Check length (10 or 13)
            if (cleanISBN.Length != 10 && cleanISBN.Length != 13)
            {
                return (false, "ISBN must be exactly 10 or 13 digits");
            }

            return (true, "");
        }

        // Helper method to validate Title
        private (bool isValid, string errorMessage) ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return (false, "Title is required");
            }

            var trimmedTitle = title.Trim();

            if (trimmedTitle.Length < 2)
            {
                return (false, "Title must be at least 2 characters");
            }

            if (trimmedTitle.Length > 100)
            {
                return (false, "Title cannot exceed 100 characters");
            }

            return (true, "");
        }

        // Helper method to validate Author
        private (bool isValid, string errorMessage) ValidateAuthor(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
            {
                return (false, "Author is required");
            }

            var trimmedAuthor = author.Trim();

            if (trimmedAuthor.Length < 2)
            {
                return (false, "Author must be at least 2 characters");
            }

            if (trimmedAuthor.Length > 50)
            {
                return (false, "Author cannot exceed 50 characters");
            }

            return (true, "");
        }

        // Helper method to validate Publication Date
        private (bool isValid, string errorMessage) ValidatePublicationDate(DateTime pubDate)
        {
            var today = DateTime.Now;

            // Check if date is in the future
            if (pubDate > today)
            {
                return (false, "Publication date cannot be in the future");
            }

            // Check if date is after 1450
            var minDate = new DateTime(1450, 1, 1);
            if (pubDate < minDate)
            {
                return (false, "Publication date must be after 1450");
            }

            return (true, "");
        }

        // Helper method to check ISBN uniqueness
        private bool IsISBNUnique(string isbn, int? excludeId = null)
        {
            var cleanISBN = isbn.Replace("-", "");
            var books = _bookService.GetAllBooks();

            foreach (var book in books)
            {
                // Skip the current book if we're editing
                if (excludeId.HasValue && book.Id == excludeId)
                    continue;

                var cleanBookISBN = book.ISBN.Replace("-", "");
                if (cleanBookISBN == cleanISBN)
                    return false;
            }

            return true;
        }

        // GET: api/books
        [HttpGet]
        public ActionResult<List<Book>> GetAllBooks(
            [FromQuery] string searchQuery = "",
            [FromQuery] string yearFilter = "all",
            [FromQuery] string sortBy = "title",
            [FromQuery] string sortOrder = "asc",
            [FromQuery] DateTime? minDate = null,
            [FromQuery] DateTime? maxDate = null)
        {
            var books = _bookService.GetAllBooks();

            // Global search - searches across title, author, and ISBN (case-insensitive)
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                books = books.Where(b => 
                    b.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    b.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    b.ISBN.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Filter by date range
            if (minDate.HasValue)
            {
                books = books.Where(b => b.PublicationDate.Date >= minDate.Value.Date).ToList();
            }

            if (maxDate.HasValue)
            {
                books = books.Where(b => b.PublicationDate.Date <= maxDate.Value.Date).ToList();
            }

            // Filter by publication year range (preserving existing logic for backward compatibility if needed)
            if (yearFilter != "all")
            {
                books = yearFilter.ToLower() switch
                {
                    "before2000" => books.Where(b => b.PublicationDate.Year < 2000).ToList(),
                    "2000-2010" => books.Where(b => b.PublicationDate.Year >= 2000 && b.PublicationDate.Year <= 2010).ToList(),
                    "after2010" => books.Where(b => b.PublicationDate.Year > 2010).ToList(),
                    _ => books 
                };
            }

            // Sort by specified column
            books = sortBy.ToLower() switch
            {
                "author" => sortOrder.ToLower() == "desc"
                    ? books.OrderByDescending(b => b.Author).ToList()
                    : books.OrderBy(b => b.Author).ToList(),
                "date" => sortOrder.ToLower() == "desc"
                    ? books.OrderByDescending(b => b.PublicationDate).ToList()
                    : books.OrderBy(b => b.PublicationDate).ToList(),
                "title" => sortOrder.ToLower() == "desc"
                    ? books.OrderByDescending(b => b.Title).ToList()
                    : books.OrderBy(b => b.Title).ToList(),
                _ => books.OrderBy(b => b.Title).ToList()
            };

            return Ok(books);
        }

        // GET: api/books/{id}
        [HttpGet("{id}")]
        public ActionResult<Book> GetBook(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found" });
            }
            return Ok(book);
        }

        // POST: api/books
        [HttpPost]
        public ActionResult<Book> CreateBook([FromBody] Book book)
        {
            Console.WriteLine($"Received create request for: {book.Title}");
            Console.WriteLine($"Image URL Length: {book.ImageUrl?.Length ?? 0}");

            // Validate Title
            var (isValidTitle, titleError) = ValidateTitle(book.Title);
            if (!isValidTitle)
            {
                return BadRequest(new { message = titleError });
            }

            // Validate Author
            var (isValidAuthor, authorError) = ValidateAuthor(book.Author);
            if (!isValidAuthor)
            {
                return BadRequest(new { message = authorError });
            }

            // Validate ISBN
            var (isValidISBN, isbnError) = ValidateISBN(book.ISBN);
            if (!isValidISBN)
            {
                return BadRequest(new { message = isbnError });
            }

            // Check ISBN uniqueness
            if (!IsISBNUnique(book.ISBN))
            {
                return BadRequest(new { message = "This ISBN already exists" });
            }

            // Validate Publication Date
            var (isValidDate, dateError) = ValidatePublicationDate(book.PublicationDate);
            if (!isValidDate)
            {
                return BadRequest(new { message = dateError });
            }

            var createdBook = _bookService.AddBook(book);
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }

        // PUT: api/books/{id}
        [HttpPut("{id}")]
        public ActionResult<Book> UpdateBook(int id, [FromBody] Book book)
        {
            var existingBook = _bookService.GetBookById(id);
            if (existingBook == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found" });
            }

            // Validate Title
            var (isValidTitle, titleError) = ValidateTitle(book.Title);
            if (!isValidTitle)
            {
                return BadRequest(new { message = titleError });
            }

            // Validate Author
            var (isValidAuthor, authorError) = ValidateAuthor(book.Author);
            if (!isValidAuthor)
            {
                return BadRequest(new { message = authorError });
            }

            // Validate ISBN
            var (isValidISBN, isbnError) = ValidateISBN(book.ISBN);
            if (!isValidISBN)
            {
                return BadRequest(new { message = isbnError });
            }

            // Check ISBN uniqueness (excluding current book)
            if (!IsISBNUnique(book.ISBN, id))
            {
                return BadRequest(new { message = "This ISBN already exists" });
            }

            // Validate Publication Date
            var (isValidDate, dateError) = ValidatePublicationDate(book.PublicationDate);
            if (!isValidDate)
            {
                return BadRequest(new { message = dateError });
            }

            var updatedBook = _bookService.UpdateBook(id, book);
            return Ok(updatedBook);
        }

        // DELETE: api/books/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var success = _bookService.DeleteBook(id);
            if (!success)
            {
                return NotFound(new { message = $"Book with ID {id} not found" });
            }
            return NoContent();
        }
    }
}
