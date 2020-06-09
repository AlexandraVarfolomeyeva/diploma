using BLL.Interfaces;
using BLL.Models;
using diploma.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests
{
   public class BookOrderControllerTests
    {
        BookOrderForm bkordr = new BookOrderForm
        {
            Amount=1,
            IdBook=5,
            IdOrder=1
        };

        [Fact]
        public void ExceptionTest()
        {
            var mockCrud = new Mock<IDBCrud>();
            mockCrud.Setup(mock => mock.GetAllBookOrders()).Returns(GetBookOrdersTest());
            mockCrud.Setup(mock => mock.GetBook(bkordr.IdBook)).Returns(new BookAdd());
            // Arrange
            BookOrderController controller = new BookOrderController(mockCrud.Object);

            // Act
            var result = controller.Create(bkordr);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ValidationTest()
        {
            var mockCrud = new Mock<IDBCrud>();
            mockCrud.Setup(mock => mock.GetAllBookOrders()).Returns(GetBookOrdersTest());

            // Arrange
            BookOrderController controller = new BookOrderController(mockCrud.Object);

            // Act
            var result = controller.Create(null);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void AnotherBookMore5000Test()
        {
            var mockCrud = new Mock<IDBCrud>();
            mockCrud.Setup(mock => mock.GetAllBookOrders()).Returns(GetBookOrdersTest());
            mockCrud.Setup(mock => mock.GetBook(bkordr.IdBook)).Returns(GetBookTest(bkordr.IdBook));
            mockCrud.Setup(mock => mock.GetOrder(bkordr.IdOrder)).Returns(GetOrderMore5000Test(bkordr.IdOrder));
            // Arrange
            BookOrderController controller = new BookOrderController(mockCrud.Object);

            // Act
            var result = controller.Create(bkordr);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void AnotherBookLess5000Test()
        {
            var mockCrud = new Mock<IDBCrud>();
            mockCrud.Setup(mock => mock.GetAllBookOrders()).Returns(GetBookOrdersTest());
            mockCrud.Setup(mock => mock.GetBook(bkordr.IdBook)).Returns(GetBookTest(bkordr.IdBook));
            mockCrud.Setup(mock => mock.GetOrder(bkordr.IdOrder)).Returns(GetOrderLess5000Test(bkordr.IdOrder));
            // Arrange
            BookOrderController controller = new BookOrderController(mockCrud.Object);

            // Act
            var result = controller.Create(bkordr);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }
        [Fact]
        public void NoBookMore5000Test()
        {
            var mockCrud = new Mock<IDBCrud>();
            mockCrud.Setup(mock => mock.GetAllBookOrders()).Returns(GetNoBookOrdersTest());
            mockCrud.Setup(mock => mock.GetBook(bkordr.IdBook)).Returns(GetBookTest(bkordr.IdBook));
            mockCrud.Setup(mock => mock.GetOrder(bkordr.IdOrder)).Returns(GetOrderMore5000Test(bkordr.IdOrder));
            // Arrange
            BookOrderController controller = new BookOrderController(mockCrud.Object);

            // Act
            var result = controller.Create(bkordr);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }
        [Fact]
        public void NoBookLess5000Test()
        {
            var mockCrud = new Mock<IDBCrud>();
            mockCrud.Setup(mock => mock.GetAllBookOrders()).Returns(GetNoBookOrdersTest());
            mockCrud.Setup(mock => mock.GetBook(bkordr.IdBook)).Returns(GetBookTest(bkordr.IdBook));
            mockCrud.Setup(mock => mock.GetOrder(bkordr.IdOrder)).Returns(GetOrderLess5000Test(bkordr.IdOrder));
            // Arrange
            BookOrderController controller = new BookOrderController(mockCrud.Object);

            // Act
            var result = controller.Create(bkordr);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }

        private IEnumerable<BookOrderModel> GetBookOrdersTest()
        {
            List<BookOrderModel> bookOrders = new List<BookOrderModel>();
            bookOrders.Add(new BookOrderModel { Amount = 1, IdBook = 5, IdOrder = 1 });
            return bookOrders;
        }

        private IEnumerable<BookOrderModel> GetNoBookOrdersTest()
        {
            return  new List<BookOrderModel>();
        }

        private BookAdd GetBookTest(int id)
        {
            return new BookAdd()
            {
                Id = id,
                Cost=400,
                Weight = 450
            };
        }

        private OrderModel GetOrderMore5000Test(int id)
        {
            return new OrderModel()
            {
                Id = id,
                Weight = 6000,
                Active = 1,
                SumOrder = 4000,
                SumDelivery = 1200,
                Amount = 12
            };
        }

        private OrderModel GetOrderLess5000Test(int id)
        {
            return new OrderModel()
            {
                Id = id,
                Weight = 450,
                Active = 1,
                SumOrder = 400,
                SumDelivery = 500,
                Amount = 1
            };
        }
    }
}
