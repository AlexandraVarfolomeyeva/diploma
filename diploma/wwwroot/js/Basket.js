﻿uriDeleteAll = "/Personal/DeleteAll/"


function deleteAll(id) {
    //добавление к заказу книги
    //добавляем новое поле в промежуточную таблицу
    try {
        var request = new XMLHttpRequest();
        url = uriDeleteAll + id;
        request.open("DELETE", url, false);
        request.onload = function () {
            // Обработка кода ответа
            //if (request.status === 401) {
            //    msg = "У вас не хватает прав для удаления";
            //} else if (request.status === 204) {
            //    msg = "Запись удалена";
            //} else {
            //    msg = "Неизвестная ошибка";
            //}
        };
        request.send();

    } catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!" + e); }
}
