const uriBookOrder = "/api/BookOrder/";
const uriIndex = "/Home/Index/";

document.addEventListener("DOMContentLoaded", function () {
    reloadBasket();
    loadBooks();
});



function loadBooks() {
    try {
        var page = document.getElementById("page").innerHTML;
        $(".js__bookList").load("/Home/GetBookView", { page: page, searchString: "", sortOrder: "" });
    } catch (e) { }
}

function Search() {
    try {
    var search_word = document.querySelector('#search').value;
    var page = document.getElementById("page").innerHTML;
    $(".js__bookList").load("/Home/GetBookView", { page: page, searchString: search_word, sortOrder: ""});
    } catch (e) { }
}

function reloadBasket() {
    try {
        $(".js___basket").load("/Home/GetView", { viewName: "_BasketDiv"});
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

function areYouSure(id, title) {
    try {
    document.getElementById("message").innerHTML = "Вы уверены, что хотите удалить книгу " + title;
   var modalElem = document.querySelector('.modal1[data-modal="4"]');
    modalElem.classList.add('active');
    var overlay = document.querySelector('#overlay-modal');
    overlay.classList.add('active');
    let form = document.getElementById("prompt-form");

    form.submit.onclick = function () {
        modalElem.classList.remove('active');
        overlay.classList.remove('active');
        deleteBook(id);
    };

    form.cancel.onclick = function () {
        modalElem.classList.remove('active');
        overlay.classList.remove('active');
    };

    document.onkeydown = function (e) {
        if (e.key == 'Escape') {
            modalElem.classList.remove('active');
            overlay.classList.remove('active');
        }
        if (e.key === 13) {
            modalElem.classList.remove('active');
            overlay.classList.remove('active');
            deleteBook(id);
        }
    };
} catch (e) { }
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