namespace Lemondo.Libary.API.Modules.Authors.Models;

public record AuthorReadDto(int Id, string FirstName, string LastName, DateOnly BornDate, IEnumerable<BookForAuthorReadDto>? Books);

public record BookForAuthorReadDto(int Id, string Title);