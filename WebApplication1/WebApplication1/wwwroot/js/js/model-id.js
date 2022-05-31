var clickId;
$("#exampleModalCenter").on('show.bs.modal', function (event) {
    $(event.currentTarget).find('asp-route-id').val(getId);
    var btn = $(this).find('#excluir-model');
    btn.attr('formaction', '/Home/Excluir/' + clickId);
});

function getId(clicked_id) {
    this.clickId = clicked_id;
}