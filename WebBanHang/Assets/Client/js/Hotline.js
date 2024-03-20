var hotline = document.querySelector('.hotline');
var hotlineText = document.querySelector('#hotline-text');

hotline.addEventListener('mouseover', function () {
    hotlineText.style.display = 'block';
});

hotline.addEventListener('mouseout', function () {
    hotlineText.style.display = 'none';
});