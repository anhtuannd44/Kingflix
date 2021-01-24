$(function () {
    // Summernote
    $('.textarea').summernote({
        height: 500,
        focus: true,
        toolbar: [
            ['style', ['style']],
            ['style', ['bold', 'italic', 'underline', 'clear']],
            ['fontsize', ['fontsize']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['table', ['table']],
            ['insert', ['link', 'image', 'video']],
            ['view', ['fullscreen', 'codeview']],
        ],
        buttons: {
            image: uploadImageButton
        },
        callbacks: {
            onPaste: function (e) {
                var bufferText = ((e.originalEvent || e).clipboardData || window.clipboardData).getData('Text');
                e.preventDefault();
                document.execCommand('insertText', false, bufferText);
            }
        }
    });
});
var textAreaId = 0;
var textareaClass = '';
var uploadImageButton = function () {
    var id = textAreaId;
    var ui = $.summernote.ui;
    var button = ui.button({
        className: "imageTextarea_" + id,
        contents: '<i class="note-icon-picture"/>',
        tooltip: 'Upload hình ảnh',
        click: function () {
            textareaClass = '.text-area-' + id;
            $(textareaClass).summernote('editor.saveRange');
            $("#ModalTemplate-Img").modal("show");
            loadLibraryImage(false, null, true);
        }
    });
    textAreaId++;
    return button.render();
}