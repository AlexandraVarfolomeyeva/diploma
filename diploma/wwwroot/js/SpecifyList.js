document.addEventListener("DOMContentLoaded", function () {
    let input = document.getElementById("search");
    input.onkeydown = function (e) {
        if (e.key === 13) {
            var search_word = input.value;
            console.log(search_word);
        }
    }

    $("#search").keyup(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            var search_word = document.querySelector('#search').value;
            console.log(search_word);
        }
    });
});

function Search(){
    var search_word = document.querySelector('#search').value;



}