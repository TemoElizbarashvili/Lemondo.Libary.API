namespace Lemondo.Libary.API.Modules.Authors.Models;

public record AuthorUpdateDto(int Id, string FirstName, string LastName, DateOnly BornDate);
