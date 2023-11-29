using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models;
using AutoMapper;
using BookStoreApp.API.Models.Dtos.Book;
using AutoMapper.QueryableExtensions;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<AuthorsController> _logger;
        private readonly IMapper _mapper;
        private readonly BookStoreDbContext _context;

        public BooksController(BookStoreDbContext context, ILogger<AuthorsController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadOnlyBookDTO>>> GetBooks()
        {
            var books = await _context.Books.Include(q=> q.Author).ProjectTo<ReadOnlyBookDTO>(_mapper.ConfigurationProvider).ToListAsync();
            var booksDTO = _mapper.Map<IEnumerable<ReadOnlyBookDTO>>(books);
            return Ok(booksDTO);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DetailsBookDTO>> GetBook(int id)
        {
            var book = await _context.Books.Include(q => q.Author).ProjectTo<DetailsBookDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(q=> q.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, UpdateBookDTO bookDTO)
        {
            if (id != bookDTO.Id)
            {
                return BadRequest();
            }
            var book = await _context.Books.FindAsync(id);
            if (book is null)
            {
                return NotFound();
            }
            _mapper.Map(bookDTO, book);
            _context.Entry(book).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CreateBookDTO>> PostBook(ReadOnlyBookDTO bookDTO)
        {
            var book = _mapper.Map<Book>(bookDTO);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetBook", new { id = book.Id }, bookDTO);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> BookExists(int id)
        {
            return await _context.Books.AnyAsync(e => e.Id == id);
        }
    }
}
