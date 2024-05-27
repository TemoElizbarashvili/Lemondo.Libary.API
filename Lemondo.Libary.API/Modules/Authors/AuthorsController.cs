using AutoMapper;
using Lemondo.Libary.API.Modules.Authors.Models;
using Lemondo.Libary.API.Modules.Books.Models;
using Lemondo.Libary.API.Modules.Shared.Controller;
using Lemondo.Libary.API.UnitOfWork.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lemondo.Libary.API.Modules.Authors;
[Route("[controller]")]
[ApiController]
public class AuthorsController(IUnitOfWork uow, IMapper mapper) : BaseController(uow, mapper)
{
    [HttpGet("List")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<BookReadDto>>> ListBooksAsync(AuthorControlFlags flag = AuthorControlFlags.Basic)
    {
        if (flag.HasFlag(AuthorControlFlags.Basic))
            return Ok(_mapper.Map<IEnumerable<AuthorReadDto>>(await _unitOfWork.AuthorRepository.GetAllAsync()));
        return Ok(_mapper.Map<IEnumerable<AuthorReadDto>>(await _unitOfWork.AuthorRepository.GetAllAsync(includeProperties: a => a.Books!)));
    }

    [HttpGet("{id:int}", Name = "GetAuthorByIdAsync")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<BookReadDto>> GetAuthorByIdAsync([FromRoute] int id, AuthorControlFlags flag = AuthorControlFlags.Basic)
    {
        try
        {
            Author authorFromDb = flag switch
            {
                (AuthorControlFlags.Basic) => await _unitOfWork.AuthorRepository.GetByIdAsync(id),
                _ => await _unitOfWork.AuthorRepository.GetByIdAsync(id, includeProperties: a => a.Books!),
            };
            return Ok(_mapper.Map<AuthorReadDto>(authorFromDb));
        }
        catch (ArgumentNullException)
        {
            return NotFound($"Book with ID {id}, not found!");

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
    }

    [HttpDelete("{id:int}/Delete")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [Authorize]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        try
        {
            await _unitOfWork.AuthorRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (ArgumentNullException)
        {
            return BadRequest($"Can not Find Book with ID - {id}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
        return NoContent();
    }

    [HttpPost("Add")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<int>> AddBookAsync(AuthorCreateDto authorCreateDto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var author = _mapper.Map<Author>(authorCreateDto);

            List<Book> existingBooks = new(), newBooks = new();

            if (authorCreateDto.BookIds is not null && authorCreateDto.BookIds.Any())
                existingBooks = await _unitOfWork.BookRepository.ListByIdsAsync(authorCreateDto.BookIds);

            if (authorCreateDto.Books is not null && authorCreateDto.Books.Any())
                newBooks = _mapper.Map<List<Book>>(authorCreateDto.Books);

            author.Books = existingBooks.Concat(newBooks).ToList();

            await _unitOfWork.AuthorRepository.AddAsync(author);
            var result = await _unitOfWork.CommitAsync();

            if (result.Item1 < 0)
                return StatusCode(500, $"An error occurred while processing your request, Trace -> {result.exceptionMessage}");

            return CreatedAtRoute(nameof(GetAuthorByIdAsync), new { author.Id }, author.Id);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }

    }

    [HttpPut("Update")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<AuthorReadDto>> UpdateAsync(AuthorUpdateDto authorUpdateDto)
    {
        var mappedAuthor = _mapper.Map<Author>(authorUpdateDto);
        try
        {
            await _unitOfWork.AuthorRepository.UpdateAsync(mappedAuthor);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
        return Ok(_mapper.Map<AuthorReadDto>(mappedAuthor));
    }
}
