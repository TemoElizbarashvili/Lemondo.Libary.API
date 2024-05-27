namespace Lemondo.Libary.API.Modules.Books.Models;

public record BookUpdateDto(int Id, string Title, string Description, DateOnly PublishedOn, bool IsAvailable);