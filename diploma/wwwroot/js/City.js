const uriCities = "/Admin/Cities/";

document.addEventListener("DOMContentLoaded", function () {
    loadCities();
});

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