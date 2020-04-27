function validateRegForm() {
    // проверяем пароли
    // выбираем элементы
    var password1 = document.getElementById('password');
    var password2 = document.getElementById('passwordConfirm');
    // сравниваем написанное, если не равно, то:
    if (password1.value !== password2.value) {
        // сообщаем пользователю, можно сделать как угодно
        alert('Пароли не совпадают!');
        password1.validateForm = false;
        // сообщаем браузеру, что не надо обрабатывать нажатие кнопки
        // как обычно, т. е. не надо отправлять форму
        return false;
    }
    var password_regexp = /(?=.*[0-9])(?=.*[!@#$%^&*])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z!@#$%^&*]{6,}/g;
    if (!password_regexp.test(password1.value)) {
        alert('Пароль должен содержать строчные и прописные латинские буквы, цифры и специальные символы!');
        return false;
    }
    return true;
}

function validateTextForm() {
    // проверяем email
    var email = document.getElementById('email');

    // /[0-9a-z_^-A-Z]+@[a-z_^-A-Z^.]+\.[a-zA-Z]{2,4}/i;
    var email_regexp = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
    // проверяем значение поля email, если нет, то: 
    if (!email_regexp.test(email.value)) {
        alert('Ошибка в email!');
        email.isvalid = false;
        email.setAttribute("isvalid", "false")
        return false;
    }

    var phone = document.getElementById('phone');
    var phone_regexp = /[0-9]{10}/i;
    // проверяем значение поля , если нет, то: 
    if (!phone_regexp.test(phone.value)) {
        alert('Ошибка в номере телефона! Пожалуйста, введите только цифры! Номер телефона должен состоять из 11 цифр, включая +7');
        return false;
    }

    var FIO = document.getElementById('fio');
    var FIO_regexp = /[A-Za-zА-Яа-я- ]{9,}/i;
  //  var FIO_regexp ="[A-Za-zА-Яа-яЁё]+(\s+[A-Za-zА-Яа-яЁё]+)?"
    // проверяем значение поля, если нет, то: 
    if (!FIO_regexp.test(FIO.value)) {
        alert('Ошибка в  поле ФИО!');
        return false;
    }

    var Username = document.getElementById('name');
    var Username_regexp = /[0-9a-z_-]+/g;
    if (!Username_regexp.test(Username.value)) {
        alert('Ошибка в  поле имени пользователя! Допускаются только латинские буквы нижнего регистра, цифры и символы дефис и нижнее подчеркивание!');
        return false;
    }
    return true;
}

function validateForm() {
    if (validateRegForm() && validateTextForm() === true) {
        return true;
    }
    else {
        return false;
    }
}

class obj {
    addThreeToNumber (arr1, arr2) {
        let coeffs = [];
        for (let i = 0; i < arr1.length + arr2.length - 1; ++i) {
            coeffs.push(0);
        }
        for (let i = 0; i < arr1.length; ++i) {
            for (let j = 0; j < arr2.length; ++j) {

                coeffs[i + j] += this.multiply(arr1[i], arr2[j]);
            }
        }
        return coeffs;
    };
    multiply (el1, el2) {
        return el1 * el2;
    };
}


