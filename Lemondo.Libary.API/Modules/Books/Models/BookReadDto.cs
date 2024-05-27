namespace Lemondo.Libary.API.Modules.Books.Models;

public record BookReadDto(int Id, string Title, string? Description, DateOnly PublishedOn, bool IsAvailable, List<AuthorForBookReadDto>? Authors);

public record AuthorForBookReadDto(int Id, string FirstName, string LastName);