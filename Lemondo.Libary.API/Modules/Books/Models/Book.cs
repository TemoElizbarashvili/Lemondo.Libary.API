using Lemondo.Libary.API.Modules.Authors.Models;

namespace Lemondo.Libary.API.Modules.Books.Models;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public byte[]? Image { get; set; }
    public DateOnly PublishedOn { get; set; }
    public bool IsAvailable { get; set; }

    public ICollection<Author>? Authors { get; set; }
}
