const uriDeleteAll = "/Personal/DeleteAll/"
const uriDecrease = "/Personal/Decrease/"
const uriIncrease = "/Personal/Increase/"
const uriDeleteItem = "/Personal/DeleteItem/"
const uriMakeOrder = "/Personal/MakeOrder/"
const uriCancelOrder = "/Personal/CancelOrder/"

function SureDeleteAll(id) {
    try {
        document.getElementById("message").innerHTML = "Вы уверены, что хотите очистить заказ?";
        var modalElem = document.querySelector('.modal1[data-modal="4"]');
        modalElem.classList.add('active');
        var overlay = document.querySelector('#overlay-modal');
        overlay.classList.add('active');
        let form = document.getElementById("prompt-form");

        form.submit.onclick = function () {
            modalElem.classList.remove('active');
            overlay.classList.remove('active');
            deleteAll(id);
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
                deleteAll(id);
            }
        };
    } catch (e) { }
}

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

function CancelOrder(id) {
    try {
        document.querySelector("#message").innerHTML = "Вы уверены, что хотите отменить заказ?";
        var modalElem = document.querySelector('.modal1[data-modal="4"]');
        modalElem.classList.add('active');
        var overlay = document.querySelector('#overlay-modal');
        overlay.classList.add('active');
        let form = document.getElementById("prompt-form");

        form.submit.onclick = function () {
            modalElem.classList.remove('active');
            overlay.classList.remove('active');
            Cancel(id);
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
                Cancel(id);
            }
        };
    } catch (e) { }
}

function Cancel(id) {
    try {
        var url = uriCancelOrder + id;
        $.ajax({
            url: url,
            type: "PUT",
            success: response => {
                reloadHistory();
                document.getElementById("status-msg").innerHTML = "<p class=\"text-danger\">Отменен</p>"
            },
            error: response => {
                reloadMessage(response.responseText);
            }
        });
    } catch (e) { }
}