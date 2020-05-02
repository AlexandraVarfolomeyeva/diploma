!function (e) { "function" != typeof e.matches && (e.matches = e.msMatchesSelector || e.mozMatchesSelector || e.webkitMatchesSelector || function (e) { for (var t = this, o = (t.document || t.ownerDocument).querySelectorAll(e), n = 0; o[n] && o[n] !== t;)++n; return Boolean(o[n]) }), "function" != typeof e.closest && (e.closest = function (e) { for (var t = this; t && 1 === t.nodeType;) { if (t.matches(e)) return t; t = t.parentNode } return null }) }(window.Element.prototype);

const uriBooks = "/api/Books/";
const uriView = "/api/View/";
const uriAuthors = "/api/Authors/";
const uriPublishers = "/api/Publisher/";
const uriGenres = "/api/Genre/";
var idBook;
var selectedPub;
var authors = [];
var genres = [];

var elForm = document.querySelector("#addForm");

document.addEventListener("DOMContentLoaded", function () {
    downloadAuthors(); downloadPublishers(); downloadGenres();
    var params = decodeURIComponent(location.search.substr(1)).split('&');
    params.splice(0, 1);
    idBook = params[0];
    console.log(idBook);
    if (!idBook) {
        document.querySelector("#addBtn").addEventListener("click", function () {
            if (elForm.checkValidity() === false) {
                event.preventDefault()
                event.stopPropagation()
            }
            else {
                addBook();
            }
            elForm.classList.add('was-validated')
        });
    } else {
        getBookData();
        document.querySelector("#addBtn").addEventListener("click", function () {
            if (elForm.checkValidity() === false) {
                event.preventDefault()
                event.stopPropagation()
            }
            else {
                saveBook();
            }
            elForm.classList.add('was-validated')
        });
    }
});

function downloadAuthors() {
    let request = new XMLHttpRequest();
    request.open("GET", uriAuthors, true);
    request.onload = function () {
        if (request.status === 200) {
            var authorsOptions = JSON.parse(request.responseText);
            for (i in authorsOptions) {
                var newOption = new Option(authorsOptions[i].name, authorsOptions[i].id);
                addForm.authorSelect.options[addForm.authorSelect.options.length] = newOption;
            }
            if (authors.length > 0) {
                document.querySelector("#authorSelect").value = authors[0].value;
                deleteauthoroption(0);
            }
        }
    };
    request.send();
}

function downloadGenres() {
    let request = new XMLHttpRequest();
    request.open("GET", uriGenres, true);
    request.onload = function () {
        if (request.status === 200) {
            var genreOptions = JSON.parse(request.responseText);
            for (i in genreOptions) {
                var newOption = new Option(genreOptions[i].name, genreOptions[i].id);
                addForm.GenreSelect.options[addForm.GenreSelect.options.length] = newOption;
            }
            if (genres.length > 0) {
                document.querySelector("#GenreSelect").value = genres[0].value;
                deletegenreoption(0);
            }
        }
    };
    request.send();
}


function getSelectedAuthors() {
    var x = "";
    for (i in authors) {
        x += "<div class=\"input-group mb-3\"> <input type=\"text\" readonly class=\"form-control\" aria-describedby=\"button-addon2\" value=\"" + authors[i].text + "\"> <div class=\"input-group-append\"><button class=\"btn btn-outline-secondary\" type=\"button\" onclick=\"deleteauthoroption(" + i + ");\">Удалить</button></div></div>";
    }
    document.getElementById("authorDiv").innerHTML = x;
}
function newauthor() {
    var selector = document.querySelector("#authorSelect");
    var value = selector[selector.selectedIndex].value;
    var text = selector[selector.selectedIndex].text;
    authors.push({ value: value, text: text });
    selector.removeChild(selector[selector.selectedIndex]);
    getSelectedAuthors();
}
function deleteauthoroption(index) {
    var newOption = new Option(authors[index].text, authors[index].value);
    addForm.authorSelect.options[addForm.authorSelect.options.length] = newOption;
    authors.splice(index, 1);
    getSelectedAuthors();
}


function getSelectedGenres() {
    var x = "";
    for (i in genres) {
        x += "<div class=\"input-group mb-3\"> <input type=\"text\" readonly class=\"form-control\" aria-describedby=\"button-addon2\" value=\"" + genres[i].text + "\"> <div class=\"input-group-append\"><button class=\"btn btn-outline-secondary\" type=\"button\" onclick=\"deletegenreoption(" + i + ");\">Удалить</button></div></div>";
    }
    document.getElementById("GenreDiv").innerHTML = x;
}
function newgenre() {
    var selector = document.querySelector("#GenreSelect")
    var value = selector[selector.selectedIndex].value;
    var text = selector[selector.selectedIndex].text;
    genres.push({ value: value, text: text });
    selector.removeChild(selector[selector.selectedIndex]);
    getSelectedGenres();
}
function deletegenreoption(index) {
    var newOption = new Option(genres[index].text, genres[index].value);
    addForm.GenreSelect.options[addForm.GenreSelect.options.length] = newOption;
    genres.splice(index, 1);
    getSelectedGenres();
}


function createAuthor() {
    try {
        var AuthorName = document.querySelector("#AuthorName").value;
        var request = new XMLHttpRequest();
        request.open("POST", uriAuthors);
        request.onload = function () {
            // Обработка кода ответа
            if (request.status == 201) {
                document.getElementById("AuthorName").value = "";
                document.querySelector('.modal1.active').classList.remove('active');
                document.querySelector('#overlay-modal').classList.remove('active');
                var author = JSON.parse(request.response);
                var newOption = new Option(author.name, author.id);
                addForm.authorSelect.options[addForm.authorSelect.options.length] = newOption;
            } else if (request.status == 409) {
                alert("Автор с таким именем уже существует!");
            } else {
                alert("Error " + request.status + ": " + request.responseText);
            }
        };
        request.setRequestHeader("Accepts", "application/json;charset=UTF-8");
        request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        request.send(JSON.stringify({
            name: AuthorName
        }));//добавление строки заказа
    } catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!"); }
}

function createPublisher() {
    try {
        var PublisherTitle = document.querySelector("#PublisherTitle").value;
        var request = new XMLHttpRequest();
        request.open("POST", uriPublishers);
        request.onload = function () {
            // Обработка кода ответа
            if (request.status == 201) {
                document.getElementById("PublisherTitle").value = "";
                document.querySelector('.modal1.active').classList.remove('active');
                document.querySelector('#overlay-modal').classList.remove('active');
                var publisher = JSON.parse(request.response);
                var newOption = new Option(publisher.name, publisher.id);
                addForm.publisherSelect.options[addForm.publisherSelect.options.length] = newOption;
            } else if (request.status == 409) {
                alert("Издательство с таким названием уже существует!");
            }
            else {
                alert("Error " + request.status + ": " + request.responseText);
            }
        };
        request.setRequestHeader("Accepts", "application/json;charset=UTF-8");
        request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        request.send(JSON.stringify({
            name: PublisherTitle
        }));//добавление строки заказа
    } catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!"); }
}

function getImg() {
    document.getElementById("ImgForm").submit();
    var x = document.getElementById("file");
    document.getElementById('labelImg').innerHTML = x.files[0].name;
    document.getElementById('bookImg').src = window.URL.createObjectURL(x.files[0]);
}

function downloadPublishers() {
    let request = new XMLHttpRequest();
    request.open("GET", uriPublishers, true);
    request.onload = function () {
        if (request.status === 200) {
            var publishers = JSON.parse(request.responseText);
            for (i in publishers) {
                var newOption = new Option(publishers[i].name, publishers[i].id);
                addForm.publisherSelect.options[addForm.publisherSelect.options.length] = newOption;
            }
            if (selectedPub) { document.querySelector("#publisherSelect").value = selectedPub; }
        }
    };
    request.send();
}

function addBook() {
    //добавление к заказу книги
    //добавляем новое поле в промежуточную таблицу
    try {
        var title = document.querySelector("#title").value;
        var authorSelect = document.querySelector("#authorSelect").value; ///authorSelect
        var genreSelect = document.querySelector("#GenreSelect").value; ///authorSelect
        var content = document.querySelector("#content").value;
        var year = document.querySelector("#year").value;
        var publisherSelect = document.querySelector("#publisherSelect").value; ///publisherSelect
        var cost = document.querySelector("#cost").value;
        var stored = document.querySelector("#Stored").value;
        var inputImg = document.getElementById("labelImg").innerHTML;
        var author = [];
        var genre = [];
        for (i in authors) {
            author.push(authors[i].value);
        }
        author.push(authorSelect);
        for (i in genres) {
            genre.push(genres[i].value);
        }
        genre.push(genreSelect);
        var request = new XMLHttpRequest();
        request.open("POST", uriBooks);
        request.onload = function () {
            // Обработка кода ответа
            if (request.status == 201) {
                window.location.href = "/Home/Index";
            } else {
                alert("Error");
            }
        };
        request.setRequestHeader("Accepts", "application/json;charset=UTF-8");
        request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        request.send(JSON.stringify({
            image: inputImg,
            year: year,
            cost: cost,
            stored: stored,
            content: content,
            title: title,
            publisher: publisherSelect,
            idAuthors: author,
            idGenres: genre
        }));//добавление строки заказа
    } catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!"); }
}

function getBookData() {
    var request2 = new XMLHttpRequest();
    var url = uriView + idBook;
    request2.open("GET", url, false);//получение данных текущего заказа
    request2.onload = function () {
        if (request2.status === 200) {
            var book = JSON.parse(request2.responseText);
            document.querySelector("#title").value = book.title;
            document.querySelector("#content").value = book.content;
            document.querySelector("#year").value = book.year;
            document.querySelector("#cost").value = book.cost;
            document.querySelector("#Stored").value = book.stored;
            document.getElementById("bookImg").src = "../img/" + book.image;
            document.getElementById('labelImg').innerHTML = book.image;
            selectedPub = book.publisher;
            for (i in book.authors) {
                authors.push({ value: book.idAuthors[i], text: book.authors[i] });
            }

            for (i in book.genres) {
                genres.push({ value: book.idGenres[i], text: book.genres[i] });
            }
        } else {
            alert("Возникла ошибка, попробуйте обновить.");
        }
    };
    request2.send();
}

function saveBook() {
    try {
        var title = document.querySelector("#title").value;
        var authorSelect = document.querySelector("#authorSelect").value; ///authorSelect
        var genreSelect = document.querySelector("#GenreSelect").value; ///authorSelect
        var content = document.querySelector("#content").value;
        var year = document.querySelector("#year").value;
        var publisherSelect = document.querySelector("#publisherSelect").value; ///publisherSelect
        var cost = document.querySelector("#cost").value;
        var stored = document.querySelector("#Stored").value;

        var author = [];
        var genre = [];
        var file = document.getElementById("labelImg").innerHTML;
        var inputImg = file;

        for (i in authors) {
            author.push(authors[i].value);
        }
        author.push(authorSelect);
        for (i in genres) {
            genre.push(genres[i].value);
        }
        genre.push(genreSelect);
        var request = new XMLHttpRequest();
        var url = uriView + idBook;
        request.open("PUT", url);
        request.onload = function () {
            // Обработка кода ответа
            if (request.status == 204) {
                window.location.href = "/Home/Index";
            } else {
                alert("Error");
            }
        };
        request.setRequestHeader("Accepts", "application/json;charset=UTF-8");
        request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        request.send(JSON.stringify({
            image: inputImg,
            year: year,
            cost: cost,
            stored: stored,
            content: content,
            title: title,
            publisher: publisherSelect,
            idAuthors: author,
            idGenres: genre
        }));//добавление строки заказа
    } catch (e) { alert("Возникла непредвиденая ошибка! Попробуйте позже!"); }
}