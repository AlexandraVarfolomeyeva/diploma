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
    button.setAttribute('onclick', 'Edit('+id+');');
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
        $(".js__cities_table").load("/Admin/GetCitiesTable/");
    } catch (e) { }
}