const uri = "Account/Register";
let myObj = "";
var elForm = document.querySelector("#regForm");

document.addEventListener("DOMContentLoaded", function () {
    document.querySelector("#registerBtn").addEventListener("click", function () {
        if (elForm.checkValidity() === false) {
            event.preventDefault()
            event.stopPropagation()
        }
        else {
            if (Validation()) { Register(); }
        }
        elForm.classList.add('was-validated')
    });
});//// Обработка клика по кнопке регистрации


function Register() {
    try {
        // Считывание данных с формы
        myObj = "";
        var fio = document.querySelector("#fio").value;
        var userName = document.querySelector("#name").value;
        var email = document.querySelector("#email").value;
        var address = document.querySelector("#address").value;
        var phoneNumber = document.querySelector("#phone").value;
        var password = document.querySelector("#password").value;
        var citySelect = document.querySelector("#citySelect").value;
        var passwordConfirm = document.querySelector("#passwordConfirm").value;
        let request = new XMLHttpRequest();

        request.open("POST", uri);//запрос на регистрацию - -создание нового пользователя
        request.setRequestHeader("Accepts",
            "application/json;charset=UTF-8");
        request.setRequestHeader("Content-Type",
            "application/json;charset=UTF-8");
        // Обработка ответа
        request.onload = function () {
            ParseResponse(this); //выводим список ошибок или результат регистрации
        };
        // Запрос на сервер
        request.send(JSON.stringify({
            fio: fio,
            email: email,
            userName: userName,
            phoneNumber: phoneNumber,
            address: address,
            idCity: citySelect,
            password: password,
            passwordConfirm: passwordConfirm
        }));
    }
    catch (e) { alert("Возникла непредвиденая ошибка при добавлении пользователя! Попробуйте позже!"); }
}

// Разбор ответа
function ParseResponse(e) {
    // Очистка контейнера вывода сообщений
    try {
        document.querySelector("#msg").innerHTML = "";
        var formError = document.querySelector("#formError");
        while (formError.firstChild) {
            formError.removeChild(formError.firstChild);
        }
        // Обработка ответа от сервера
        if (e.status != 200) {
            let response = JSON.parse(e.responseText);

            document.querySelector("#msg").innerHTML = response.message;
            alert(response.message);

            // Вывод сообщений об ошибках
            if (response.error.length > 0) {
                for (var i = 0; i < response.error.length; i++) {
                    let ul = document.querySelector("ul");
                    let li = document.createElement("li");
                    li.appendChild(document.createTextNode(response.error[i]));
                    ul.appendChild(li);
                }
            }
        }
        else {
            window.location.href = "index.html";
        }
        // Очистка полей паролей
        document.querySelector("#password").value = "";
        document.querySelector("#passwordConfirm").value = "";
    }
    catch (e) { alert("Возникла непредвиденая ошибка при чтении ответа! Попробуйте позже!"); }
}





