document.querySelector("#logoffBtn").addEventListener("click",
    logOff);
document.addEventListener("DOMContentLoaded", function () {
 
});

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
