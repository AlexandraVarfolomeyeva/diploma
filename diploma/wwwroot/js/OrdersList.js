const uriStatus = "/Admin/ChangeStatus/";

function ChangeStatus(id) {
    try {
        var option = $('#Status').val();
        var optionName = $('#Status option:selected').text();
        $.ajax({
            url: uriStatus,
            data: {
                id: id,
                option: option
            },
            type: "PUT",
            success: response => {
                var className = "";
                switch (option) {
                    case "0": className = "text-danger"; break;
                    case "2": className = "text-info"; break;
                    case "3": className = "text-success"; break;
                };
                var name = "status" + id;
                var html = "<p class=\"" + className + "\">" + optionName + "</p>";
                document.getElementById(name).innerHTML = html;
            },
            error: response => {
                console.log("Error " + response.responseText);
            }
        });
    } catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!"); }
}