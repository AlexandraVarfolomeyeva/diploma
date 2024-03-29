﻿const uriBookOrder = "/api/BookOrder/";
const uriIndex = "/Home/Index/";
const uriBooks = "/api/Books/";

document.addEventListener("DOMContentLoaded", function () {
    if (window.location.pathname !== "/") {
        reloadBasket();
    }
    clapTitles();
});

function clapTitles() {
    var titles = document.querySelectorAll(".js__clap_text");
    titles.forEach(function (item) {
        var title = item.innerHTML;
        var words = title.split(' ').filter(function (el) {
            return el != "";
        });
        title = "";
        words.forEach(function (word) {
            if (word != 0) {
                title += word + " ";
            }
        });
        if (title.length > 40) {
            var length = 0;
            title = "";
            words.forEach(function (word) {
                if (word != 0 && length < 30) {
                    title += word + " ";
                    length += word.length;
                }
            });
            title += "...";
        };
        item.innerHTML = title;
    });
}

function loadBooks() {
    try {
        var page = document.getElementById("pageHidden").innerHTML;
        var search_word = document.getElementById("searchHidden").innerHTML;
        var OrderBy = document.getElementById("orderHidden").innerHTML;
        var Stored = document.getElementById("storedHidden").innerHTML;
        var Genre = document.getElementById("genreHidden").innerHTML;
        var AuthorSearch = document.getElementById("authorHidden").innerHTML;
        $(".js__bookList").load("/Home/GetBookView", { page: page, searchString: search_word, sortOrder: OrderBy, Stored: Stored, Genre: Genre, AuthorSearch: AuthorSearch });
    } catch (e) { }
}

function reloadBasket() {
    try {
        $(".js___basket").load("/Home/GetBasketView");
    } catch (e) {  }
}

//id -- книги ; sum -- цена книги
function add(id, order) {
    var button = document.getElementById(id);
    button.setAttribute('onclick', '');
    //добавление к заказу книги
    //добавляем новое поле в промежуточную таблицу
    try {
        var bookOrder = {
            'IdBook': id,
            'IdOrder': order,
            'Amount': 1
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
        var url = uriBooks + id;
        request.open("DELETE", url, false);
        request.onload = function () {
            // Обработка кода ответа
            var msg = "";
            if (request.status === 401) {
                msg = "У вас не хватает прав для удаления";
            } else if (request.status === 204) {
                if (window.location.pathname === "/") {
                    loadBooks();
                } else {
                    window.location.href = "/Home/Index";
                }

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