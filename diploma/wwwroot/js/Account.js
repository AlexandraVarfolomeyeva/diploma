// Обработка клика по кнопке регистрации
document.querySelector("#loginBtn").addEventListener("click", 
    logIn);


function logIn() {
    try {
        var user, password = "";
        var remember;
        // Считывание данных с формы
        user = document.getElementById("User").value;
        password = document.getElementById("Password").value;
        remember = document.getElementById("Remember").checked;
        var request = new XMLHttpRequest();
        request.open("POST", "/Account/Login");
        request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        request.onreadystatechange = function () {
            // Очистка контейнера вывода сообщений
            document.getElementById("msg").innerHTML = "";
            var mydiv = document.getElementById('formError');
            while (mydiv.firstChild) {
                mydiv.removeChild(mydiv.firstChild);
            }
            // Обработка ответа от сервера
            if (request.responseText !== "") {
                var msg = null;
                msg = JSON.parse(request.responseText);
                document.getElementById("msg").innerHTML = msg.message;
                // Вывод сообщений об ошибках
                if (typeof msg.error !== "undefined" && msg.error.length >
                    0) {
                    for (var i = 0; i < msg.error.length; i++) {
                        var ul = document.getElementsByTagName("ul");
                        var li = document.createElement("li");
                        li.appendChild(document.createTextNode(msg.error[i]));
                        ul[0].appendChild(li);
                    }
                }
                document.getElementById("Password").value = "";
            }
            if (request.status === 200) {
                //   alert("Вы успешно вошли!");
                window.location.href = "index.html";
            }
        };
        // Запрос на сервер
        request.send(JSON.stringify({
            user: user,
            password: password,
            rememberMe: remember
        }));
        // вывести результат
    }
    catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!"); }
}
