using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechLibrary.Data;
using TechLibrary.Domain;
using TechLibrary.Models;
using TechLibrary.Helpers;

namespace TechLibrary.Services
{
    public interface IBookService
    {
        Task<Book> GetBookByIdAsync(int bookid);
        Task<Book> UpdateBookByIdAsync(int bookid, string description);
        Task<int> CreateBook(string title, int isbn, string description);
        Task<List<Book>> GetBooksAsync(int page = 0, int recordsPerPage = 10, string filterType = "", string filterString = "");
        int GetBooksCount(string filterType, string filterString);
    }

    public class BookService : IBookService
    {
        private readonly DataContext _dataContext;

        public BookService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Book> GetBookByIdAsync(int bookid)
        {
            return await _dataContext.Books.SingleOrDefaultAsync(x => x.BookId == bookid);
        }
        public async Task<int> CreateBook(string title, int isbn, string description)
        {
            var book = new Book();
            book.Title = title;
            book.ISBN = isbn.ToString();
            book.ShortDescr = description;

            _dataContext.Add(book);
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception($"Could not create record Books database");
            }

            return book.BookId;
        }

        public async Task<Book> UpdateBookByIdAsync(int id, string description)
        {
            var bookRecord = await _dataContext.Books.SingleAsync(x => x.BookId == id);
            bookRecord.ShortDescr = description;
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception($"Could not update record {id} in Books database");
            }

            return bookRecord;
        }

        public async Task<List<Book>> GetBooksAsync(int page = 0, int recordsPerPage = 10, string filterType = "", string filterString = "")
        {
            // use queryable to prevent performance hit by not loading 
            // entire record set in momory
            var queryable = _dataContext.Books.AsQueryable();

            // ending bounds
            var end = page * recordsPerPage;
            // starting bounds
            var start = end - recordsPerPage;

            // begin queryable variable chain
            queryable = queryable
                // ensure we're starting from the beginning
                .OrderBy(p => p.BookId);

            // if filter params are valid
            queryable = queryable.FilterBooks(filterType, filterString);
            
            queryable = queryable
                //only take the [recordsPerPage] count starting from the given page
                .Skip(start)
                .Take(recordsPerPage);

            return await queryable.ToListAsync();
        }

        public int GetBooksCount(string filterType, string filterString)
        {
            // get total count of books with optional filter parameters
            var queryable = _dataContext.Books.AsQueryable();
            queryable = queryable.FilterBooks(filterType, filterString);
            var bookCount = queryable.Count();
            
            return bookCount;
        }
    }
}
