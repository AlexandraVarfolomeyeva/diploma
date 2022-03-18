using BLL.Interfaces;
using BLL.Models;
using diploma.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests
{
    public class BooksControllerTests
    {
        [Fact]
        public void AddBookAddsBook()
        {
            var mockCrud = new Mock<IDBCrud>();

            // Arrange
            BooksController controller = new BooksController(mockCrud.Object);

            var newBook = new BookAdd()
            {
                Content = "",
                Cost = 300,
                isDeleted = false,
                idAuthors = new int[1] { 1 },
                idGenres = new int[1] { 1 },
                image = "empty.png",
                Publisher = 1,
                Stored = 0,
                Title = "",
                Weight = 0,
                Year = "2019"
            };

            // Act
            var result = controller.Create(newBook);

            // Assert
            mockCrud.Verify(r => r.CreateBook(newBook));
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public void AddBookreturnsBadRequestIfModelStateInvalide()
        {
            var mockCrud = new Mock<IDBCrud>();

            // Arrange
            BooksController controller = new BooksController(mockCrud.Object);
            // Act
            var result = controller.Create(null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

    }
}
