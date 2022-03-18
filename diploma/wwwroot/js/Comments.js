const uriComments = "/Admin/Comments/";
const uriDeleteComment = "/Admin/DeleteComment/"

function Sent(id)
{
    var form = $("#commentForm");

    $.ajax({
        type: "POST",
        url: uriComments,
        data: form.serialize(), // serializes the form's elements.
        success: function (data) {
            loadComments(id);
        }
    });
}

function loadComments(bookId) {
    $(".js__comments_all").load("/Admin/GetCommentsView/", { id: bookId });
}

function DeleteComment(id, bookId) {
    var url = uriDeleteComment + id
    $.ajax({
        type: "DELETE",
        url: url,
        success: function (data) {
            loadComments(bookId);
        }
    });
}