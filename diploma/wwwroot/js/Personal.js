document.querySelector("#logoffBtn").addEventListener("click",
    logOff);
document.addEventListener("DOMContentLoaded", function () {
    getUserInfo();
});

function logOff() {
    try {
        var request = new XMLHttpRequest();
        request.open("POST", "api/account/logoff");
        request.onload = function () {
            if (this.status == 200) {
                window.location.href = "index.html";
            }
        };
        request.setRequestHeader("Content-Type",
            "application/json;charset=UTF-8");
        request.send();
    }
    catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!"); }
}
