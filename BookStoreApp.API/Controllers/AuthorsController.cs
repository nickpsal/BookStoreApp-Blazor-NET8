﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models;
using BookStoreApp.API.Models.Dtos.Author;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly BookStoreDbContext _context;
        private readonly ILogger<AuthorsController> _logger;
        private readonly IMapper _mapper;

        public AuthorsController(BookStoreDbContext context, ILogger<AuthorsController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadOnlyAuthorDTO>>> GetAuthors()
        {
            try
            {
                List<Author> authors = await _context.Authors.ToListAsync();
                IEnumerable<ReadOnlyAuthorDTO> authorsDTO = _mapper.Map<List<ReadOnlyAuthorDTO>>(authors);
                return Ok(authorsDTO);
            }catch (Exception exp)
            {
                _logger.LogError($"Error performing GET {nameof(GetAuthors)}");
                return BadRequest($"Error : {exp.Message}");
            }
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadOnlyAuthorDTO>> GetAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    return NotFound();
                }
                var authorDTO = _mapper.Map<ReadOnlyAuthorDTO>(author);
                return Ok(authorDTO);
            }catch (Exception exp)
            {
                _logger.LogError($"Error performing GET {nameof(GetAuthors)}");
                return BadRequest($"Error : {exp.Message}");
            }
        }

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> PutAuthor(int id, UpdateAuthorDTO authorDTO)
        {
            if (id != authorDTO.Id)
            {
                return BadRequest("You dont give thw same ID");
            }
            var authorDB = await _context.Authors.FindAsync(id);
            if (authorDB is null)
            {
                return NotFound("Book not found");
            }
            _mapper.Map(authorDTO, authorDB);
            _context.Entry(authorDB).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exp)
            {
                if (!AuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Error: {exp}");
                    throw;
                }
            }
            return Ok("Book Operatoin Submitted Succesfully");
        }

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<CreateAuthorDTO>> PostAuthor(CreateAuthorDTO authorDTO)
        {
            try
            {
                var author = _mapper.Map<Author>(authorDTO);
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
            }
            catch (Exception exp)
            {
                _logger.LogError($"Error performing GET {nameof(GetAuthors)}");
                return BadRequest($"Error : {exp.Message}");
            }
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    return NotFound();
                }
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception exp)
            {
                _logger.LogError($"Error performing GET {nameof(GetAuthors)}");
                return BadRequest($"Error : {exp.Message}");
            }
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
