using BookAPI.Models;

namespace BookAPI.Services
{
    public class BookService
    {
        private static List<Book> _books = new List<Book>
        {
            new Book 
            { 
                Id = 1, 
                Title = "The Great Gatsby", 
                Author = "F. Scott Fitzgerald", 
                ISBN = "978-0743273565",
                PublicationDate = new DateTime(1925, 4, 10),
                ImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?q=80&w=800&auto=format&fit=crop"
            },
            new Book 
            { 
                Id = 2, 
                Title = "To Kill a Mockingbird", 
                Author = "Harper Lee", 
                ISBN = "978-0061120084",
                PublicationDate = new DateTime(1960, 7, 11),
                ImageUrl = "https://images.unsplash.com/photo-1543002588-bfa74002ed7e?q=80&w=800&auto=format&fit=crop"
            },
            new Book 
            { 
                Id = 3, 
                Title = "1984", 
                Author = "George Orwell", 
                ISBN = "978-0451524935",
                PublicationDate = new DateTime(1949, 6, 8),
                ImageUrl = "https://images.unsplash.com/photo-1534447677768-be436bb09401?q=80&w=800&auto=format&fit=crop"
            },
            new Book 
            { 
                Id = 4, 
                Title = "Pride and Prejudice", 
                Author = "Jane Austen", 
                ISBN = "978-0141439518",
                PublicationDate = new DateTime(1813, 1, 28)
            },
            new Book 
            { 
                Id = 5, 
                Title = "The Catcher in the Rye", 
                Author = "J.D. Salinger", 
                ISBN = "978-0316769174",
                PublicationDate = new DateTime(1951, 7, 16)
            },
            new Book 
            { 
                Id = 6, 
                Title = "Jane Eyre", 
                Author = "Charlotte Brontë", 
                ISBN = "978-0141441146",
                PublicationDate = new DateTime(1847, 10, 16)
            },
            new Book 
            { 
                Id = 7, 
                Title = "The Lord of the Rings", 
                Author = "J.R.R. Tolkien", 
                ISBN = "978-0544003415",
                PublicationDate = new DateTime(1954, 7, 29)
            },
            new Book 
            { 
                Id = 8, 
                Title = "Moby-Dick", 
                Author = "Herman Melville", 
                ISBN = "978-0142437247",
                PublicationDate = new DateTime(1851, 10, 18)
            },
            new Book 
            { 
                Id = 9, 
                Title = "The Hobbit", 
                Author = "J.R.R. Tolkien", 
                ISBN = "978-0547928227",
                PublicationDate = new DateTime(1937, 9, 21)
            },
            new Book 
            { 
                Id = 10, 
                Title = "Wuthering Heights", 
                Author = "Emily Brontë", 
                ISBN = "978-0141439556",
                PublicationDate = new DateTime(1847, 12, 19)
            },
            new Book 
            { 
                Id = 11, 
                Title = "The Picture of Dorian Gray", 
                Author = "Oscar Wilde", 
                ISBN = "978-0141439570",
                PublicationDate = new DateTime(1890, 7, 1)
            },
            new Book 
            { 
                Id = 12, 
                Title = "The Odyssey", 
                Author = "Homer", 
                ISBN = "978-0143109280",
                PublicationDate = new DateTime(1488, 1, 1)
            }
        };

        private static int _nextId = 13;

        public List<Book> GetAllBooks()
        {
            return _books;
        }

        public Book? GetBookById(int id)
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }

        public Book AddBook(Book book)
        {
            book.Id = _nextId++;
            _books.Add(book);
            return book;
        }

        public Book? UpdateBook(int id, Book updatedBook)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return null;

            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.ISBN = updatedBook.ISBN;
            book.PublicationDate = updatedBook.PublicationDate;
            book.ImageUrl = updatedBook.ImageUrl;

            return book;
        }

        public bool DeleteBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return false;

            _books.Remove(book);
            return true;
        }
    }
}
