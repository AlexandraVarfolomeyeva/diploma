﻿uriDeleteAll = "/Personal/DeleteAll/"
uriDecrease = "/Personal/Decrease/"
uriIncrease = "/Personal/Increase/"
uriDeleteItem = "/Personal/DeleteItem/"
uriMakeOrder = "/Personal/MakeOrder/"

function GetDetails(id) {
    try {
        $(".js__details_history").load("/Personal/GetView", { viewName: "_DetailsOrder", message: id });
        modalElem = document.querySelector('.modal1[data-modal="3"]');
        modalElem.classList.add('active');
        document.querySelector('#overlay-modal').classList.add('active');
    } catch (e) { }
}

function reloadBasketTable() {
    try {
        $(".js__get_basket_table").load("/Personal/GetView", { viewName: "_BasketTable", message: "" });
    } catch (e) { }
}

function reloadMessage(message) {
    try {
        $(".js__get_basket_message").load("/Personal/GetView", { viewName: "_AdminMsg", message: message });
    } catch (e) { }
}

function reloadHistory() {
    try {
        $(".js__get_basket_history").load("/Personal/GetView", { viewName: "_BasketHistory", message: "" });
    } catch (e) { }
}



document.addEventListener("DOMContentLoaded", function () {
    reloadBasketTable();
    reloadHistory();

    var overlay = document.querySelector('#overlay-modal'),
    closeButtons = document.querySelectorAll('.js-modal-close');

    overlay.addEventListener('click', function () {
        document.querySelector('.modal1.active').classList.remove('active');
        this.classList.remove('active');
    });

    closeButtons.forEach(function (item) {
        item.addEventListener('click', function (e) {
            var parentModal = this.closest('.modal1');

            parentModal.classList.remove('active');
            overlay.classList.remove('active');
        });
    }); // end foreach  
});

function Increase(id) {
    reloadMessage("");
    $.ajax({
            url: uriIncrease+id,
            type: "PUT",
        success: response => {
            reloadBasketTable();
            if (response == true) {
                reloadMessage("На складе больше нет!");
                    }
            },
        error: response => {
            console.log("Error " + response.responseText);
            }
        });

}

function Decrease(id) {
    reloadMessage("");
    $.ajax({
        url: uriDecrease + id,
        type: "PUT",
        success: response => {
                reloadBasketTable();
        },
        error: response => {
            console.log("Error" + response.responseText);
        }
    });

}

function DeleteItem(id) {
    reloadMessage("");
    $.ajax({
        url: uriDeleteItem + id,
        type: "PUT",
        success: response => {
                reloadBasketTable();
        },
        error: response => {
            console.log("Error" + response.responseText);
        }
    });
}


function deleteAll(id) {
    //добавление к заказу книги
    //добавляем новое поле в промежуточную таблицу
    try {
        var request = new XMLHttpRequest();
        url = uriDeleteAll + id;
        request.open("DELETE", url, false);
        request.onload = function () {
            // Обработка кода ответа
            if (request.status === 401) {
                msg = "У вас не хватает прав для удаления";
            } else if (request.status === 204) {
                msg = "Запись удалена";
                reloadBasketTable();
                //$.ajax({
                //    type: "GET",
                //    url: "Personal/Basket",
                //    dataType: "html",
                //    success: function (result) {
                //        $("#BasketContent").html(result);
                //        //$("#SmallCartCount").text($("#SmallCartList .SmallCartCount").val());
                //        //$("#SmallCartPrice").text($("#SmallCartList .SmallCartPrice").val());
                //    }
                //});
                //window.location.href = "/Personal/Basket";
            } else {
                msg = "Неизвестная ошибка";
            }
        };
        request.send();

    } catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!" + e); }
}

function OrderBooks(id) {
    reloadMessage("");
    $.ajax({
        url: uriMakeOrder + id,
        type: "PUT",
        success: response => {
            reloadBasketTable();
            reloadHistory();
            reloadMessage("Успешно заказано!");
        },
        error: response => {
            alert("Error" + response.responseText);
        }
    });
}
