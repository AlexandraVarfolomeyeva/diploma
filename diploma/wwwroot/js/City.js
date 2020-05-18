const uriCities = "/Admin/Cities/";
const uriCity = "/Admin/City/";
const uriEditCity = "/Admin/EditCity/";
const uriDeleteCity = "/Admin/DeleteCity/";

document.addEventListener("DOMContentLoaded", function () {
    loadCities();
});

function EditCity(id) {
    var url = uriCity + id;
    var modalElem = document.querySelector('.modal1[data-modal="6"]');
    modalElem.classList.add('active');
    var overlay = document.querySelector('#overlay-modal');
    overlay.classList.add('active');
    var button = document.getElementById("AddCity");
    button.setAttribute('onclick', 'Edit(' + id + ');');
    document.getElementById("titleCity").innerHTML = "Редактировать город";

    $.ajax({
        type: "GET",
        url: url,
        success: function (data, textStatus, jqXHR) {
            console.log(data);
            document.getElementById("CityName").value = data.name;
            document.querySelector("#DeliverySum").value = data.deliverySum;
            document.querySelector("#DeliveryTime").value = data.deliveryTime;
        }
    });
}

function Edit(id) {
    var url = uriEditCity + id;
    var form = $("#cityForm");
    $.ajax({
        type: "PUT",
        url: url,
        data: form.serialize(), // serializes the form's elements.
        success: function (data, textStatus, jqXHR) {
            if (jqXHR.status == 204) {
                loadCities();
                document.getElementById("CityName").value = "";
                document.querySelector("#DeliverySum").value = "";
                document.querySelector("#DeliveryTime").value = "";
                document.querySelector('.modal1.active').classList.remove('active');
                document.querySelector('#overlay-modal').classList.remove('active');
                button.setAttribute('onclick', 'createNewCity();');
                
            }
        },
        error: function (data) {
            console.log(data);
            if (data.status == 409) {
                alert("Не все поля заполнены!");
            }
            button.setAttribute('onclick', 'Edit('+id+');');
        }
    });
}

function Addbtn() {
    var modalElem = document.querySelector('.modal1[data-modal="6"]');
    document.getElementById("CityName").value = "";
    document.querySelector("#DeliverySum").value = "";
    document.querySelector("#DeliveryTime").value = "";
    document.getElementById("titleCity").innerHTML = "Добавить город";
    modalElem.classList.add('active');
    var overlay = document.querySelector('#overlay-modal');
    overlay.classList.add('active');
    var button = document.getElementById("AddCity");
    button.setAttribute('onclick', 'createNewCity();');
}

function SureDeleteCity(id,name) {
    try {
        document.getElementById("message").innerHTML = "Вы уверены, что хотите удалить город "+name+"?";
        var modalElem = document.querySelector('.modal1[data-modal="4"]');
        modalElem.classList.add('active');
        var overlay = document.querySelector('#overlay-modal');
        overlay.classList.add('active');
        let form = document.getElementById("prompt-form");

        form.submit.onclick = function () {
            modalElem.classList.remove('active');
            overlay.classList.remove('active');
            DeleteCity(id);
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

function DeleteCity(id) {
    var url = uriDeleteCity + id;
        $.ajax({
            type: "DELETE",
            url: url,
            success: function (data, textStatus, jqXHR) {
                if (jqXHR.status == 200) {
                    loadCities();
                }
            }
        });
}

function createNewCity() {
    var button = document.getElementById("AddCity");
    button.setAttribute('onclick', '');
    var form = $("#cityForm");
    $.ajax({
        type: "POST",
        url: uriCities,
        data: form.serialize(), // serializes the form's elements.
        success: function (data, textStatus, jqXHR) {
            if (jqXHR.status == 201) {
                document.getElementById("CityName").value = "";
                document.querySelector("#DeliverySum").value = "";
                document.querySelector("#DeliveryTime").value = "";
                document.querySelector('.modal1.active').classList.remove('active');
                document.querySelector('#overlay-modal').classList.remove('active');
                button.setAttribute('onclick', 'createNewCity();');
                loadCities();
            }
        },
        error: function (data) {
            console.log(data);
            if (data.status == 409) {
                alert("Не все поля заполнены!");
            }
            button.setAttribute('onclick', 'createNewCity();');
        }
    });
}

function loadCities() {
    try {
        var search = document.getElementById("searchCity").value;
        var order = document.getElementById("orderCitySort").value;
        $(".js__cities_table").load("/Admin/GetCitiesTable/", { search: search, order: order});
    } catch (e) { }
}