document.addEventListener("DOMContentLoaded", function () {

    var modalButtons = document.querySelectorAll('.js-open-modal'),
        overlay = document.querySelector('#overlay-modal'),
        closeButtons = document.querySelectorAll('.js-modal-close');

    overlay.addEventListener('click', function () {
        document.querySelector('.modal1.active').classList.remove('active');
        this.classList.remove('active');
    });

    modalButtons.forEach(function (item) {
        item.addEventListener('click', function (e) {
            e.preventDefault();
            var modalId = this.getAttribute('data-modal'),
                modalElem = document.querySelector('.modal1[data-modal="' + modalId + '"]');

            modalElem.classList.add('active');
            overlay.classList.add('active');
        }); // end click
    }); // end foreach

    closeButtons.forEach(function (item) {

        item.addEventListener('click', function (e) {
            var parentModal = this.closest('.modal1');

            parentModal.classList.remove('active');
            overlay.classList.remove('active');
        });

    }); // end foreach  
});