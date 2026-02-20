using BookAPI.Models;

namespace BookAPI.Services
{
    public interface IBookService
    {
        List<Book> GetAllBooks();
        List<Book> GetFilteredBooks(
            string searchQuery,
            string yearFilter,
            string sortBy,
            string sortOrder,
            DateTime? minDate,
            DateTime? maxDate);
        Book? GetBookById(int id);
        Book AddBook(Book book);
        Book? UpdateBook(int id, Book updatedBook);
        bool DeleteBook(int id);
        bool IsISBNUnique(string isbn, int? excludeId = null);
    }
}
