using Lemondo.Libary.API.Modules.Authors.Models;
using Lemondo.Libary.API.Modules.Books.Models;

namespace Lemondo.Libary.API.DataBase;

public class SeedData(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task EnsureDataIsSeeded()
    {
        if (!_context.Books.Any())
        {
            var authors = new Author[]
            {
                new Author
                {
                    BornDate = new DateOnly(1890, 9, 15),
                    FirstName = "Agatha",
                    LastName = "Christie"
                },
                new Author
                {
                    BornDate = new DateOnly(1965, 5, 27),
                    FirstName = "Stephen",
                    LastName = "King"
                }
            };

            var books = new Book[]
            {
                new Book
                {
                    Title = "The Big Four",
                    Description = "A secret organization named 'The Big Four'.",
                    IsAvailable = true,
                    Image = null,
                    PublishedOn = new DateOnly(1927, 1, 27),
                    Authors = new List<Author> { authors[0] }
                },
                new Book
                {
                    Title = "The Shining",
                    Description = "A horror novel by Stephen King.",
                    IsAvailable = true,
                    Image = null,
                    PublishedOn = new DateOnly(1977, 1, 28),
                    Authors = new List<Author> { authors[1] }
                },
                new Book
                {
                    Title = "Murder on the Orient Express",
                    Description = "Another famous mystery novel by Agatha Christie.",
                    IsAvailable = true,
                    Image = null,
                    PublishedOn = new DateOnly(1934, 1, 1),
                    Authors = new List<Author> { authors[0] }
                },
                new Book
                {
                    Title = "It",
                    Description = "A horror novel by Stephen King about a monstrous being.",
                    IsAvailable = true,
                    Image = null,
                    PublishedOn = new DateOnly(1986, 9, 15),
                    Authors = new List<Author> { authors[1] }
                }
            };

            _context.Authors.AddRange(authors);
            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();
        }
    }
}
