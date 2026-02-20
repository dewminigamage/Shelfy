using BookAPI.Models;
using BookAPI.Services;
using BookAPI.Validators;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        /// <summary>
        /// Validates a book's fields and ISBN uniqueness. Returns an error message or null if valid.
        /// </summary>
        private string? ValidateBook(Book book, int? excludeId = null)
        {
            var (validTitle, titleErr) = BookValidator.ValidateTitle(book.Title);
            if (!validTitle) return titleErr;

            var (validAuthor, authorErr) = BookValidator.ValidateAuthor(book.Author);
            if (!validAuthor) return authorErr;

            var (validIsbn, isbnErr) = BookValidator.ValidateISBN(book.ISBN);
            if (!validIsbn) return isbnErr;

            if (!_bookService.IsISBNUnique(book.ISBN, excludeId))
                return "This ISBN already exists";

            var (validDate, dateErr) = BookValidator.ValidatePublicationDate(book.PublicationDate);
            if (!validDate) return dateErr;

            return null;
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
            var books = _bookService.GetFilteredBooks(searchQuery, yearFilter, sortBy, sortOrder, minDate, maxDate);
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
            _logger.LogInformation("Create book request: {Title} (image length: {Length})", book.Title, book.ImageUrl?.Length ?? 0);

            var error = ValidateBook(book);
            if (error != null)
                return BadRequest(new { message = error });

            var createdBook = _bookService.AddBook(book);
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }

        // PUT: api/books/{id}
        [HttpPut("{id}")]
        public ActionResult<Book> UpdateBook(int id, [FromBody] Book book)
        {
            if (_bookService.GetBookById(id) == null)
                return NotFound(new { message = $"Book with ID {id} not found" });

            var error = ValidateBook(book, excludeId: id);
            if (error != null)
                return BadRequest(new { message = error });

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
