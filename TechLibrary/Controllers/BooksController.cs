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
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int recordsPerPage)
        {
            _logger.LogInformation("Get all books");
            _logger.LogInformation("queryString: " + Request.QueryString);

            // expose total books count header value because CORS is currently enabled
            HttpContext.Response.Headers.Add("access-control-expose-headers", "x-total-books-count");
            // set total items in custom header value
            var totalBooksCount = _bookService.GetTotalBooksCount();
            // send total books count in custom header
            HttpContext.Response.Headers.Add("x-total-books-count", totalBooksCount.ToString());

            // initialize return variable
            List<Book> books;

            // if page URL parameter is passed in and int parse successful
            if (page != 0)
            {
                // call async method in BookService to retrieve books with pagination param
                books = await _bookService.GetBooksByPageAsync(page, recordsPerPage);
            }
            else
            {
                // give full response, /books/ endpoint without params should be
                // either be backwards compatable or deprecated
                _logger.LogInformation($"total books count is {totalBooksCount}");
                books = await _bookService.GetBooksAsync();
            }

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
    }
}
