function Concern() {
    try {
        $(".js__concern").load("/Account/Concern");
        modalElem = document.querySelector('.modal1[data-modal="5"]');
        modalElem.classList.add('active');
        document.querySelector('#overlay-modal').classList.add('active');
    } catch (e) { }
}