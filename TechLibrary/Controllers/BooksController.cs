using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TechLibrary.Domain;
using TechLibrary.Models;
using TechLibrary.Services;
using System;

namespace TechLibrary.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : Controller
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BooksController(ILogger<BooksController> logger, IBookService bookService, IMapper mapper)
        {
            _logger = logger;
            _bookService = bookService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int recordsPerPage, [FromQuery] string filterString, [FromQuery] string filterType)
        {
            _logger.LogInformation("Get all books");
            _logger.LogInformation("queryString: " + Request.QueryString);

            // expose total books count header value because CORS is enabled
            HttpContext.Response.Headers.Add("access-control-expose-headers", "x-total-books-count");
            // if empty params give full response for backwards compatability
            int totalBooksCount = _bookService.GetBooksCount(filterType, filterString);
            _logger.LogInformation($"total books count is {totalBooksCount}");
            // send total books count in custom header
            HttpContext.Response.Headers.Add("x-total-books-count", totalBooksCount.ToString());


            // call async method in BookService to retrieve books with pagination param
            // if empty params give full response for backwards compatability
            var books = await _bookService.GetBooksAsync(page, recordsPerPage, filterString, filterType);
            var bookResponse = _mapper.Map<List<BookResponse>>(books);

            return Ok(bookResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Get book by id {id}");

            var book = await _bookService.GetBookByIdAsync(id);

            var bookResponse = _mapper.Map<BookResponse>(book);

            return Ok(bookResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookById(int id, [FromForm] string description)
        {
            _logger.LogInformation($"Update (PUT) book by id {id} with description {description}");
            //var book = await _bookService.GetBookByIdAsync(id);
            var book = await _bookService.UpdateBookByIdAsync(id, description);

            var bookResponse = _mapper.Map<BookResponse>(book);

            return Ok(bookResponse);
        }
    }
}
