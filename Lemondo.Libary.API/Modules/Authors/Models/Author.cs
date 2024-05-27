using Lemondo.Libary.API.Modules.Books.Models;

namespace Lemondo.Libary.API.Modules.Authors.Models;

public class Author
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly BornDate { get; set; }

    public ICollection<Book>? Books { get; set; }
}
