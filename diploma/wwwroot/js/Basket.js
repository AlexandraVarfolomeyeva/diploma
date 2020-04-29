uriDeleteAll = "/Personal/DeleteAll/"

function reloadBasketTable() {
    try {
        $(".js__get_basket_table").load("/Personal/GetView");
    } catch (e) { }
}

document.addEventListener("DOMContentLoaded", function () {
    reloadBasketTable();
});

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
