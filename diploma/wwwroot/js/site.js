// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const uriBookOrder = "/api/BookOrder/";


//id -- книги ; sum -- цена книги
function add(id, sum,order) {
    //добавление к заказу книги
    //добавляем новое поле в промежуточную таблицу
    try {
        var bookOrder = {
            'IdBook': id,
            'IdOrder': order,
            'Amount': 1,
            'Sum': sum
        }
        var request = new XMLHttpRequest();
        request.open("POST", uriBookOrder);
        request.onload = function () {
            // Обработка кода ответа
            if (request.status === 201) {
                loadBasket();//загрузка корзины для обновления данных о заказе
            } else if (request.status === 401) {
                alert("Пожалуйста, авторизируйтесь");
            } else {
                alert("Неизвестная ошибка");
            }
        };
        request.setRequestHeader("Accepts", "application/json;charset=UTF-8");
        request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        request.send(JSON.stringify(bookOrder));//добавление строки заказа
    } catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!"+e); }
}
