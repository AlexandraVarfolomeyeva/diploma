const uriBookOrder = "/api/BookOrder/";
const uriIndex = "/Home/Index/";

document.addEventListener("DOMContentLoaded", function () {
    reloadBasket();
    loadBooks();
});

function loadBooks() {
    try {
        $(".js__bookList").load("/Home/GetView", { viewName: "_BookList" });
    } catch (e) { }
}

function reloadBasket() {
    try {
        $(".js___basket").load("/Home/GetView", { viewName: "_BasketDiv" });
    } catch (e) {  }
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
            if (request.status === 200) {
                //загрузка корзины для обновления данных о заказе
                reloadBasket();
                button.innerHTML = "Перейти к корзине";
                button.setAttribute('class', 'btn btn-light');
                button.setAttribute('onclick', 'GetBasket()');
                
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