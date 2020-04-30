uriDeleteAll = "/Personal/DeleteAll/"
uriDecrease = "/Personal/Decrease/"
uriIncrease = "/Personal/Increase/"
uriDeleteItem = "/Personal/DeleteItem/"
uriMakeOrder = "/Personal/MakeOrder/"


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

document.addEventListener("DOMContentLoaded", function () {
    reloadBasketTable();
});

function Increase(id) {
    reloadMessage("");
    $.ajax({
            url: uriIncrease+id,
            type: "PUT",
        success: response => {
            if (response.overload == true) {
                    reloadMessage("На складе больше нет!");
                    }
                document.getElementById(id).innerHTML = response.bookAmount;
                document.getElementById("Amount").innerHTML = response.orderAmount;
                document.getElementById("SumOrder").innerHTML = response.sumOrder;
            },
            error: response => {         
                    alert("Error" + response.responseText);
            }
        });

}

function Decrease(id) {
    reloadMessage("");
    $.ajax({
        url: uriDecrease + id,
        type: "PUT",
        success: response => {
            document.getElementById(id).innerHTML = response.bookAmount;
            document.getElementById("Amount").innerHTML = response.orderAmount;
            document.getElementById("SumOrder").innerHTML = response.sumOrder;
            if (response.bookAmount == 0) {
                reloadBasketTable();
            }
        },
        error: response => {
            alert("Error" + response.responseText);
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
            alert("Error" + response.responseText);
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
            reloadMessage("Успешно заказано!");
        },
        error: response => {
            alert("Error" + response.responseText);
        }
    });
}
