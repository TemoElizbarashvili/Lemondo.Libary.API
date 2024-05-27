using AutoMapper;
using Lemondo.Libary.API.Modules.Authors.Models;
using Lemondo.Libary.API.Modules.Books.Models;
using Lemondo.Libary.API.Modules.Shared.Controller;
using Lemondo.Libary.API.UnitOfWork.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lemondo.Libary.API.Modules.Books;

[Route("[controller]")]
[ApiController]
public class BooksController(IUnitOfWork uow, IMapper mapper) : BaseController(uow, mapper)
{
    [HttpGet("List")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<BookReadDto>>> ListBooksAsync(BookControlFlags flag = BookControlFlags.Basic)
        => Ok(_mapper.Map<IEnumerable<BookReadDto>>(await GetListWithIncludes(flag: flag)));

    [HttpGet("{id:int}", Name = "GetByIdAsync")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<BookReadDto>> GetByIdAsync([FromRoute] int id, BookControlFlags flag = BookControlFlags.Basic)
    {
        try
        {
            var bookFromDb = (await GetListWithIncludes(id: id, flag: flag)).FirstOrDefault();
            return Ok(_mapper.Map<BookReadDto>(bookFromDb));
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
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [Authorize]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        try
        {
            await _unitOfWork.BookRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
        catch (ArgumentNullException)
        {
            return BadRequest($"Can not Find Book with ID - {id}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
    }

    [HttpPut("Update")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<BookReadDto>> UpdateAsync(BookUpdateDto bookDto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var book = _mapper.Map<Book>(bookDto);
            await _unitOfWork.BookRepository.UpdateAsync(book);
            await _unitOfWork.CommitAsync();
            return Ok(_mapper.Map<BookReadDto>(book));
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
    }

    [HttpPost("Add")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<int>> AddBookAsync(BookCreateDto bookCreateDto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var book = _mapper.Map<Book>(bookCreateDto);

            List<Author> existingAuthors = new(), newAuthors = new();

            if (bookCreateDto.AuthorIds is not null && bookCreateDto.AuthorIds.Any())
                existingAuthors = await _unitOfWork.AuthorRepository.ListByIdsAsync(bookCreateDto.AuthorIds);

            if (bookCreateDto.Authors is not null && bookCreateDto.Authors.Any())
                newAuthors = _mapper.Map<List<Author>>(bookCreateDto.Authors);

            book.Authors = newAuthors.Concat(existingAuthors).ToList();

            await _unitOfWork.BookRepository.AddAsync(book);
            var result = await _unitOfWork.CommitAsync();

            if (result.Item1 < 0)
                return StatusCode(500, $"An error occurred while processing your request: {result.exceptionMessage}");

            return CreatedAtRoute(nameof(GetByIdAsync), new { book.Id }, book.Id);

        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }

    }

    [HttpPatch("{id:int}/add-image")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UploadBookImageAsync([FromRoute] int id, IFormFile image)
    {
        if (image is null || image.Length == 0)
            return BadRequest();
        try
        {
            var bookToUpdate = await _unitOfWork.BookRepository.GetByIdAsync(id);
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                bookToUpdate.Image = ms.ToArray();
            }
            await _unitOfWork.BookRepository.UpdateAsync(bookToUpdate);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
    }

    [HttpGet("{id:int}/image")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetBookImageAsync(int id)
    {
        try
        {
            var bookFromDb = await _unitOfWork.BookRepository.GetByIdAsync(id);
            if (bookFromDb.Image is null)
                return NotFound($"Book with ID {id}, does not have an image!");

            return File(bookFromDb.Image, "image/jpeg");
        }
        catch (ArgumentNullException)
        {
            return BadRequest($"Can not find book with ID - {id}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
    }

    [HttpPatch("{id:int}/change-available")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<BookReadDto>> ChangeAvailable(int id)
    {
        try
        {
            var bookInDB = await _unitOfWork.BookRepository.GetByIdAsync(id);
            bookInDB.IsAvailable = !bookInDB.IsAvailable;

            await _unitOfWork.BookRepository.UpdateAsync(bookInDB);
            await _unitOfWork.SaveChangesAsync();

            return Ok(_mapper.Map<BookReadDto>(bookInDB));
        }
        catch (ArgumentNullException)
        {
            return BadRequest("Book not found!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
    }

    private async Task<IEnumerable<Book>> GetListWithIncludes(int? id = null, BookControlFlags flag = BookControlFlags.Basic)
    {
        if (id.HasValue)
        {
            if (flag.HasFlag(BookControlFlags.All) || flag.HasFlag(BookControlFlags.Authors))
                return new List<Book> { await _unitOfWork.BookRepository.GetByIdAsync(id.Value, includeProperties: b => b.Authors!) };
            return new List<Book> { await _unitOfWork.BookRepository.GetByIdAsync(id.Value) };
        }
        else
        {
            if (flag.HasFlag(BookControlFlags.All) || flag.HasFlag(BookControlFlags.Authors))
                return await _unitOfWork.BookRepository.GetAllAsync(includeProperties: b => b.Authors!);
            return await _unitOfWork.BookRepository.GetAllAsync();
        }
    }
}
