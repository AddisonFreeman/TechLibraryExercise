using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechLibrary.Data;
using TechLibrary.Domain;
using TechLibrary.Models;

namespace TechLibrary.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetBooksAsync();
        Task<Book> GetBookByIdAsync(int bookid);
        Task<List<Book>> GetBooksByPageAsync(int page, int recordsPerPage);
        int GetTotalBooksCount();
    }

    public class BookService : IBookService
    {
        private readonly DataContext _dataContext;

        public BookService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            var queryable = _dataContext.Books.AsQueryable();

            return await queryable.ToListAsync();
        }

        public async Task<Book> GetBookByIdAsync(int bookid) 
        {
            return await _dataContext.Books.SingleOrDefaultAsync(x => x.BookId == bookid);
        }  

        public async Task<List<Book>> GetBooksByPageAsync(int page, int recordsPerPage)
        {
            // use queryable to prevent performance hit by not loading 
            // entire record set in momory
            var queryable = _dataContext.Books.AsQueryable();
            // default to 10 records per page if param 0 (not passed in)
            recordsPerPage = recordsPerPage == 0 ? 10 : recordsPerPage;

            // ending bounds
            var end = page * recordsPerPage;
            // starting bounds
            var start = end - recordsPerPage;

            var takeTenRecords = queryable
                // ensure we're starting from the beginning
                .OrderBy(p => p.BookId)
                //only take the 10 starting from the given page
                .Skip(start)
                .Take(recordsPerPage);

            return await takeTenRecords.ToListAsync();
        }

        public int GetTotalBooksCount()
        {
            var queryable = _dataContext.Books.AsQueryable();
            var bookCount = queryable.Count();

            return bookCount;
        }
    }
}
