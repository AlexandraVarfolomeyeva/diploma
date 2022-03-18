using BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    public interface IDBCrud
    {
        //Books
        IEnumerable<BookView> GetAllBookViews();
        IEnumerable<BookModel> GetAllBooks();
        BookAdd GetBook(int id);
        BookModel GetBookModel(int id);
        void CreateBook(BookAdd item);
        void UpdateBook(BookAdd item);
        void UpdateBook(BookModel item);
        void DeleteBook(int id);


        //BookOrders
        IEnumerable<BookOrderModel> GetAllBookOrders();
        BookOrderModel GetBookOrder(int id);
        void CreateBookOrder(BookOrderModel item);
        void UpdateBookOrder(BookOrderModel item);
        void DeleteBookOrder(int id);

        //Orders
        IEnumerable<OrderModel> GetAllOrders();
        OrderModel GetOrder(int id);
        void CreateOrder(OrderModel item);
        void UpdateOrder(OrderModel item);
        void DeleteOrder(int id);

        //BookAuthors
        IEnumerable<BookAuthorModel> GetAllBookAuthors();
        BookAuthorModel GetBookAuthor(int id);
        void CreateBookAuthor(BookAuthorModel item);
        void UpdateBookAuthor(BookAuthorModel item);
        void DeleteBookAuthor(int id);

        //Authors
        IEnumerable<AuthorModel> GetAllAuthors();
        AuthorModel GetAuthor(int id);
        void CreateAuthor(AuthorModel item);
        void UpdateAuthor(AuthorModel item);
        void DeleteAuthor(int id);

        //BookGenres
        IEnumerable<BookGenreModel> GetAllBookGenres();
        BookGenreModel GetBookGenre(int id);
        void CreateBookGenre(BookGenreModel item);
        void UpdateBookGenre(BookGenreModel item);
        void DeleteBookGenre(int id);

        //Genres
        IEnumerable<GenreModel> GetAllGenres();
        GenreModel GetGenre(int id);
        void CreateGenre(GenreModel item);
        void UpdateGenre(GenreModel item);
        void DeleteGenre(int id);

        //Cities
        IEnumerable<CityModel> GetAllCities();
        CityModel GetCity(int id);
        void CreateCity(CityModel item);
        void UpdateCity(CityModel item);
        void DeleteCity(int id);

        //Comments
        IEnumerable<CommentModel> GetAllComments();
        CommentModel GetComment(int id);
        void CreateComment(CommentModel item);
        void UpdateComment(CommentModel item);
        void DeleteComment(int id);

        //Publishers
        IEnumerable<PublisherModel> GetAllPublishers();
        PublisherModel GetPublisher(int id);
        void CreatePublisher(PublisherModel item);
        void UpdatePublisher(PublisherModel item);
        void DeletePublisher(int id);

        //Users
        IEnumerable<UserModel> GetAllUsers();
        UserModel GetUser(string id);
        void CreateUser(UserModel item);
        void UpdateUser(UserModel item);
        void DeleteUser(string id);

        //Addresses
        IEnumerable<AddressModel> GetAllAddresses();
        AddressModel GetAddress(int? id);
        void CreateAddress(AddressModel item);
        void UpdateAddress(AddressModel item);
        void DeleteAddress(int id);
    }
}
