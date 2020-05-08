using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface IRepositoryUser
    {
        IEnumerable<User> GetList(); // получение всех объектов
        User GetItem(string id); // получение одного объекта по id
        void Create(User item); // создание объекта
        void Update(User item); // обновление объекта
        void Delete(int id); // удаление объекта по id
    }
}
