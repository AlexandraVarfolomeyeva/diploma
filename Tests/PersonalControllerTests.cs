using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using BLL.Interfaces;
using diploma.Controllers;
using Microsoft.AspNetCore.Mvc;
using BLL.Models;
using Microsoft.AspNetCore.Identity;
using DAL.Entities;

namespace Tests
{
   public class PersonalControllerTests
    {
        [Fact]
        public void MakeOrderBadRequestTest()
        {
            var mockCrud = new Mock<IDBCrud>();
            //var mockUser = new Mock<UserManager<User>>();
            var mockEnvironment = new Mock<IHostingEnvironment>();

            // Arrange
            PersonalController controller = new PersonalController(mockCrud.Object, null, mockEnvironment.Object);

            // Act
            var result = controller.MakeOrder(-1);
          
            // Assert
             Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void MakeOrderNotEnoughTest()
        {
            int id = 0;
            int IdBook = 0;
            var mockCrud = new Mock<IDBCrud>();
            //var mockUser = new Mock<UserManager<User>>();
            var mockEnvironment = new Mock<IHostingEnvironment>();
            mockCrud.Setup(mock => mock.GetOrder(id)).Returns(GetTestOrder(id));
            mockCrud.Setup(mock => mock.GetAllBookOrders()).Returns(GetAllBookOrdersTest());
            mockCrud.Setup(mock => mock.GetBook(IdBook)).Returns(GetBookTest(IdBook));
            // Arrange
            PersonalController controller = new PersonalController(mockCrud.Object, null, mockEnvironment.Object);

            // Act
            var result = controller.MakeOrder(id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        private OrderModel GetTestOrder(int id)
        {
            return new OrderModel
            {
                Active = 1,
                Amount = 1,
                Id = id,
                Weight = 340,
                DateDelivery = DateTime.Now.AddDays(30),
                SumOrder = 455,
                SumDelivery = 300,
                UserId = "newUserId"
            };
        }

       private List<BookOrderModel> GetAllBookOrdersTest()
        {
            List<BookOrderModel> ad = new List<BookOrderModel>();
            ad.Add(new BookOrderModel
            {
                IdBook=0,
                IdOrder=0,
                Id=0,
                Amount=5
            });
            return ad;
        }

        private BookAdd GetBookTest(int id)
        {
          return new BookAdd
            {
                Id = id,
                isDeleted = false,
                Cost = 455,
                Weight = 340,
                Stored = 4
            };
        }

        [Fact]
        public void MakeOrderAllFineTest()
        {
            int id = 0;
            int IdBook = 0;
            var mockCrud = new Mock<IDBCrud>();
            var mockUser = new Mock<UserManager<User>>();
            var mockEnvironment = new Mock<IHostingEnvironment>();
            mockCrud.Setup(mock => mock.GetOrder(id)).Returns(GetTestOrder(id));
            mockCrud.Setup(mock => mock.GetAllBookOrders()).Returns(GetAllBookOrdersTest());
            mockCrud.Setup(mock => mock.GetBook(IdBook)).Returns(GetBookEnoughTest(IdBook));
           
            // Arrange
            PersonalController controller = new PersonalController(mockCrud.Object, null, mockEnvironment.Object);

            // Act
            var result = controller.MakeOrder(id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }


        private BookAdd GetBookEnoughTest(int id)
        {
            return new BookAdd
            {
                Id = id,
                isDeleted = false,
                Cost = 455,
                Weight = 340,
                Stored = 15
            };
        }


    }
}
