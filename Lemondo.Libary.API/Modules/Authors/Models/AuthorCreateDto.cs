namespace Lemondo.Libary.API.Modules.Authors.Models;

public record AuthorCreateDto(string FirstName, string LastName, DateOnly BornDate, IEnumerable<BookForAuthorCreateDto>? Books, int[]? BookIds);

public record BookForAuthorCreateDto(string Title, string? Description, DateOnly PublishedOn, bool IsAvialable);
