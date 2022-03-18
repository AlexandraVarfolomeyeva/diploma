document.querySelector("#logoffBtn").addEventListener("click", logOff);

const uriGetCities = "/Personal/GetCities/";
const uriGetAddress = "/Personal/GetAddress/";
const uriAddAddress = "/Personal/AddAddress/";
const uriEditAddress = "/Personal/EditAddress/";
const uriDeleteAddress = "/Personal/DeleteAddress/";

document.addEventListener("DOMContentLoaded", function () {
    reloadAddresses();
    downloadCities();
});

function EditAddress(id) {
    var url = uriGetAddress + id;
    var modalElem = document.querySelector('.modal1[data-modal="8"]');
    modalElem.classList.add('active');
    var overlay = document.querySelector('#overlay-modal');
    overlay.classList.add('active');
    var button = document.getElementById("AddAddress");
    button.setAttribute('onclick', 'Edit(' + id + ');');
    document.getElementById("titleCity").innerHTML = "Редактировать адрес";

    $.ajax({
        type: "GET",
        url: url,
        success: function (data, textStatus, jqXHR) {
            console.log(data);
            document.getElementById("Address").value = data.name;
            document.querySelector("#citySelect").value = data.idCity;
        }
    });
}

function Edit(id){
    var url = uriEditAddress + id;
    var address = {
        name: document.getElementById("Address").value,
        idCity: document.querySelector("#citySelect").value
    };
    $.ajax({
        type: "PUT",
        url: url,
        data: address, 
        success: function (data, textStatus, jqXHR) {
            if (jqXHR.status == 204) {
                reloadAddresses();
                document.getElementById("Address").value = "";
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
            button.setAttribute('onclick', 'Edit(' + id + ');');
        }
    });
}


function SureDeleteAddress(id, name) {
    try {
        document.getElementById("message").innerHTML = "Вы уверены, что хотите удалить адрес \"" + name + "\"?";
        var modalElem = document.querySelector('.modal1[data-modal="4"]');
        modalElem.classList.add('active');
        var overlay = document.querySelector('#overlay-modal');
        overlay.classList.add('active');
        let form = document.getElementById("prompt-form");

        form.submit.onclick = function () {
            modalElem.classList.remove('active');
            overlay.classList.remove('active');
            DeleteAddress(id);
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
                DeleteAddress(id);
            }
        };
    } catch (e) { }
}

function DeleteAddress(id) {
    var url = uriDeleteAddress + id;
    $.ajax({
        type: "DELETE",
        url: url,
        success: function (data, textStatus, jqXHR) {
            if (jqXHR.status == 200) {
                reloadAddresses();
            }
            if (jqXHR.status == 204) {
                document.getElementById("message").innerHTML = "Вы не можете удалить этот адрес, т.к. он последний!";
                var modalElem = document.querySelector('.modal1[data-modal="4"]');
                modalElem.classList.add('active');
                var overlay = document.querySelector('#overlay-modal');
                overlay.classList.add('active');
                let form = document.getElementById("prompt-form");
            }
        }
    });
}


function downloadCities() {
    let request = new XMLHttpRequest();
    request.open("GET", uriGetCities, true);
    request.onload = function () {
        if (request.status === 200) {
            citySelector = document.querySelector("#citySelect");
            var Options = JSON.parse(request.responseText);
            for (i in Options) {
                var newOption = new Option(Options[i].name, Options[i].id);
                citySelector.options[citySelector.options.length] = newOption;
            }
        }
    };
    request.send();
}


function AddAddressbtn() {
    var modalElem = document.querySelector('.modal1[data-modal="8"]');
    document.getElementById("Address").value = "";
    document.getElementById("titleCity").innerHTML = "Добавить город";
    modalElem.classList.add('active');
    var overlay = document.querySelector('#overlay-modal');
    overlay.classList.add('active');
    var button = document.getElementById("AddAddress");
    button.setAttribute('onclick', 'createNewAddress();');
}

function reloadAddresses() {
    try {
        $(".js__edit_addresses_table").load("/Personal/GetView", { viewName: "_EditAddress", message: "" });
    } catch (e) { }
}

function createNewAddress(){
        var button = document.getElementById("AddAddress");
    button.setAttribute('onclick', '');
    var address = {
        name: document.getElementById("Address").value,
        idCity: document.querySelector("#citySelect").value
    };
        $.ajax({
            type: "POST",
            url: uriAddAddress,
            data: address, 
            success: function (data, textStatus, jqXHR) {
                if (jqXHR.status == 201) {
                    document.getElementById("Address").value = "";
                    document.querySelector('.modal1.active').classList.remove('active');
                    document.querySelector('#overlay-modal').classList.remove('active');
                    button.setAttribute('onclick', 'createNewAddress();');
                    reloadAddresses();
                }
            },
            error: function (data) {
                console.log(data);
                if (data.status == 409) {
                    alert("Не все поля заполнены!");
                }
                button.setAttribute('onclick', 'createNewAddress();');
            }
        });
}

function logOff() {
    try {
        var request = new XMLHttpRequest();
        request.open("POST", "/Account/LogOff");
        request.onload = function () {
            if (this.status == 200) {
                window.location.href = "/Home/Index";
            }
        };
        request.setRequestHeader("Content-Type",
            "application/json;charset=UTF-8");
        request.send();
    }
    catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!"); }
}

//function update() {
//        $.ajax({
//            url: '/Personal/Info',
//            type: 'PUT',
//            data: $('#infoForm').serialize(),
//            success: function (data) {
//                alert("asdc");
//            }
//        });
//}
