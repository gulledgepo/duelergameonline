$('.changable').on('change', function (e) {
    $('#log').append(e.target.id + ' changed to ' + $(e.target).val() + '<br/>');
});