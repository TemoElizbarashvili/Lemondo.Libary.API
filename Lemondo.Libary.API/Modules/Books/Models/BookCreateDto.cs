namespace Lemondo.Libary.API.Modules.Books.Models;

public record BookCreateDto(string Title, string? Description, DateOnly PublishedOn, bool IsAvailable, AuthorForBookCreateDto[]? Authors, int[]? AuthorIds);

public record AuthorForBookCreateDto(string FirstName, string LastName, DateOnly BornDate);
