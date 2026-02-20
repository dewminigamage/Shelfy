namespace BookAPI.Validators
{
    public static class BookValidator
    {
        public static (bool IsValid, string Error) ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return (false, "Title is required");

            var trimmed = title.Trim();

            if (trimmed.Length < 2)
                return (false, "Title must be at least 2 characters");

            if (trimmed.Length > 100)
                return (false, "Title cannot exceed 100 characters");

            return (true, string.Empty);
        }

        public static (bool IsValid, string Error) ValidateAuthor(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
                return (false, "Author is required");

            var trimmed = author.Trim();

            if (trimmed.Length < 2)
                return (false, "Author must be at least 2 characters");

            if (trimmed.Length > 50)
                return (false, "Author cannot exceed 50 characters");

            return (true, string.Empty);
        }

        public static (bool IsValid, string Error) ValidateISBN(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return (false, "ISBN is required");

            var clean = isbn.Replace("-", "");

            if (!clean.All(char.IsDigit))
                return (false, "ISBN must contain only numbers and optional hyphens");

            if (clean.Length != 10 && clean.Length != 13)
                return (false, "ISBN must be exactly 10 or 13 digits");

            return (true, string.Empty);
        }

        public static (bool IsValid, string Error) ValidatePublicationDate(DateTime pubDate)
        {
            if (pubDate > DateTime.Now)
                return (false, "Publication date cannot be in the future");

            if (pubDate < new DateTime(1450, 1, 1))
                return (false, "Publication date must be after 1450");

            return (true, string.Empty);
        }
    }
}
