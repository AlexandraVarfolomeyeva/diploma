// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const uriBookOrder = "/api/BookOrder/";
const uriIndex = "/Home/Index/";


function reloadPage() {
    try {
        var request = new XMLHttpRequest();
        request.open("GET", uriIndex,true);
        request.onload = function () {
           
        };
        request.send();
    } catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!" + e); }
}

//id -- книги ; sum -- цена книги
function add(id, sum, order) {
    var button = document.getElementById(id);
    button.setAttribute('onclick', '');
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
                //загрузка корзины для обновления данных о заказе
                button.innerHTML = "Перейти к корзине";
                button.setAttribute('class', 'btn btn-light');
                button.setAttribute('onclick', 'GetBasket()');
                reloadPage();
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

function deleteBook(id) {//удаление книги -- метод, доступный только администратору
    try {
        var request = new XMLHttpRequest();
        var url = "/api/Books/" + id;
        request.open("DELETE", url, false);
        request.onload = function () {
            // Обработка кода ответа
            var msg = "";
            if (request.status === 401) {
                msg = "У вас не хватает прав для удаления";
            } else if (request.status === 204) {
                window.location.href = "/Home/Index";
                loadBooks();
            } else {
                msg = "Неизвестная ошибка";
            }
            document.querySelector("#actionMsg").innerHTML = msg;
        };
        request.send();
    }
    catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!"); }
}

function editBook(id) {//редактирование книги -- метод, доступный только администратору
    try {
        window.location.href = "/Admin/AddBook?&" + id;
    }
    catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!"); }
}

function GetBasket()
{
    window.location.href = "/Personal/Basket";
}