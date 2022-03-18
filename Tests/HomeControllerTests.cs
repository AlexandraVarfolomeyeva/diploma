using BLL.Interfaces;
using BLL.Models;
using DAL.Entities;
using diploma.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Tests
{
   public class HomeControllerTests
    {

        public HomeControllerTests()
        {
            var mockCrud = new Mock<IDBCrud>();
            var mockUser = new Mock<UserManager<User>>();
            var mockEnvironment = new Mock<IHostingEnvironment>();
        }

        [Fact]
        public void AboutTest()
        {
            var mockCrud = new Mock<IDBCrud>();
            //var mockUser = new Mock<UserManager<User>>();
            var mockEnvironment = new Mock<IHostingEnvironment>();

            // Arrange
            HomeController controller = new HomeController(mockCrud.Object, null, mockEnvironment.Object);

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.Equal("Your application description page.", result?.ViewData["Message"]);
            Assert.NotNull(result);
        }

        [Fact]
        public void IndexReturnsAViewResultWithAListOfBooks()
        {
            var mockCrud = new Mock<IDBCrud>();
            //var mockUser = new Mock<UserManager<User>>();
            var mockEnvironment = new Mock<IHostingEnvironment>();

            mockCrud.Setup(mock => mock.GetAllBookViews()).Returns(GetTestBooks());
          
            // Arrange
            HomeController controller = new HomeController(mockCrud.Object, null, mockEnvironment.Object);

            // Act
            var result = controller.Index(new BookListViewModel(),1,"","",false,0,"");
            Assert.NotNull(result);
            //// Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookListViewModel>(
                viewResult.Model);
            Assert.Equal(GetTestBooks().Count, model.Books.Count());
        }
        
        private List<BookView> GetTestBooks()
        {
            var books = new List<BookView>
            {
                new BookView { Id=1, Authors=new string[0],Content="",Cost=300, Genres=new string[0],image="empty.png",Publisher="",Rated=0,Score=0,Stored=0,Title="",Weight=0,Year="2019"},
                new BookView { Id=2, Authors=new string[0],Content="",Cost=300, Genres=new string[0],image="empty.png",Publisher="",Rated=0,Score=0,Stored=0,Title="",Weight=0,Year="2020"},
                new BookView { Id=3, Authors=new string[0],Content="",Cost=300, Genres=new string[0],image="empty.png",Publisher="",Rated=0,Score=0,Stored=0,Title="",Weight=0,Year="2018"},
                new BookView { Id=4, Authors=new string[0],Content="",Cost=300, Genres=new string[0],image="empty.png",Publisher="",Rated=0,Score=0,Stored=0,Title="",Weight=0,Year="2021"}
            };
            return books;
        }



    }
}
