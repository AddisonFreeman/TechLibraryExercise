using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TechLibrary.Controllers.Tests
{
    [TestFixture()]
    [Category("ControllerTests")]
    public class BooksControllerTests
    {

        private  Mock<ILogger<BooksController>> _mockLogger;
        private  Mock<IBookService> _mockBookService;
        private  Mock<IMapper> _mockMapper;
        private NullReferenceException _expectedException;
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Failed...");
            _mockLogger = new Mock<ILogger<BooksController>>();
            _mockBookService = new Mock<IBookService>();
            _mockMapper = new Mock<IMapper>();
            _httpContext = new DefaultHttpContext();
            //_httpContext.Request.Headers["X-Example"] = "test-header";
        }

        [Test()]
        public async Task Books_Endpiont_Calls_GetBooksCount_Exactly_Once()
        {
            //  Arrange
            _mockBookService.Setup(b => b.GetBooksCount(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<int>());
            var sut = new BooksController(_mockLogger.Object, _mockBookService.Object, _mockMapper.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = _httpContext
                }
            };

            //  Act
            var result = await sut.GetAll();

            //  Assert
            _mockBookService.Verify(s => s.GetBooksCount(It.IsAny<string>(), It.IsAny<string>()), Times.Once, "Expected GetBooksCount to have been called once");
        }

        [Test()]
        public async Task Books_Endpiont_Calls_GetBooksAsync_Exactly_Once()
        {
            //  Arrange
            _mockBookService.Setup(b => b.GetBooksAsync(It.IsAny<int>(),It.IsAny<int>(),It.IsAny<string>(),It.IsAny<string>())).Returns(Task.FromResult(It.IsAny<List<Domain.Book>>()));
            var sut = new BooksController(_mockLogger.Object, _mockBookService.Object, _mockMapper.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = _httpContext
                }
            };

            //  Act
            var result = await sut.GetAll();

            //  Assert
            _mockBookService.Verify(s => s.GetBooksAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once, "Expected GetBooksAsync to have been called once");
        }

        [Test()]
        public async Task Create_Book_Endpoint_Calls_GetBookById()
        {
            //  Arrange
            _mockBookService.Setup(b => b.CreateBook(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(It.IsAny<int>()));
            _mockBookService.Setup(b => b.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(It.IsAny<Domain.Book>()));
            var sut = new BooksController(_mockLogger.Object, _mockBookService.Object, _mockMapper.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = _httpContext
                }
            };

            //  Act
            var result = await sut.CreateBook("title",12345,"description");

            //  Assert
            _mockBookService.Verify(s => s.GetBookByIdAsync(It.IsAny<int>()), Times.Once, "Expected CreateBook to call GetBookById");
        }
    }
}