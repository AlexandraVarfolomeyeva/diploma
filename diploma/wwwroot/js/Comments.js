const uriComments = "/Admin/Comments/";

function Sent(id)
{
    var form = $("#commentForm");

    $.ajax({
        type: "POST",
        url: uriComments,
        data: form.serialize(), // serializes the form's elements.
        success: function (data) {
            $(".js__comments_all").load("/Admin/GetCommentsView/", {id: id});
        }
    });
}