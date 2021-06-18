using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TechLibrary.Domain;
using TechLibrary.Models;
using TechLibrary.Services;

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
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Get all books");

            var books = await _bookService.GetBooksAsync();

            var bookResponse = _mapper.Map<List<BookResponse>>(books);

            return Ok(bookResponse);
        }

        // define new route built on existing /books/ endpoint
        // pass in pageNumber parameter
        [HttpGet("page/{pageNumber}")]
        public async Task<IActionResult> GetBooksByPage(int pageNumber)
        {
            _logger.LogInformation($"Get page {pageNumber} from all books, count 10");
            // call async method in BookService to retrieve 10 books with pagination param
            var books = await _bookService.GetBooksByPageAsync(pageNumber);
            // set total items in custom header value
            var totalBooksCount = _bookService.GetTotalBooksCount();
            _logger.LogInformation($"total books count is {totalBooksCount}");
            // expose total books count header value because CORS is currently enabled
            HttpContext.Response.Headers.Add("access-control-expose-headers", "x-total-books-count");
            // send total books count in custom header
            HttpContext.Response.Headers.Add("x-total-books-count", totalBooksCount.ToString());
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
