function loadLibraryImage(IsMultiple, selectedImage, textArea, notLoadAll) {
    loadSpinner();
    var replaceElm = "#modalContent-Img";
    if (notLoadAll == "notLoadAll")
        var replaceElm = "#libraryImageList";
    $("#submitModalImage").data("textarea", textArea);
    $("#submitModalImage").data("is-multi", IsMultiple);
    $.ajax({
        method: "POST",
        url: "/Image/LoadLibraryImage",
        data: {
            IsMultiple: IsMultiple,
            selectedImage: selectedImage,
            notLoadAll: notLoadAll
        },
        success: function (data) {
            $(replaceElm).html(data);
            var windowHeight = $(window).height();
            $("#libraryImageList").css("max-height", windowHeight - 270);
            $('#libraryImageList .lazy').lazyload();
            exitSpinner();
        }
    });
}
//Select Multiple Images
function selectImage(e) {
    if ($('input[name="imageListSelect"]').attr('type') == 'checkbox') {
        $('#image-' + $(e).data('id')).data('checked', 'checked');
        $(e).attr("onclick", "unSelectImage(this)");
        $(e).addClass("active");
    }
    else {
        $('.image-item').removeClass('active');
        $('.image-item').attr("onclick", "selectImage(this)");
        $(e).attr("onclick", "unSelectImage(this)");
        $(e).addClass("active");
        $('input[name="imageListSelect"]').data('checked', '');
        $('#image-' + $(e).data('id')).data('checked', 'checked');
    }
}
function unSelectImage(e) {
    if ($('input[name="imageListSelect"]').attr('type') == 'checkbox') {
        $('#image-' + $(e).data('id')).data('checked', '');
        $(e).attr("onclick", "selectImage(this)");
        $(e).removeClass("active");
    }
    else {
        $('.image-item').removeClass('active');
        $('.image-item').attr("onclick", "selectImage(this)");
        $('input[name="imageListSelect"]').data('checked', '');
    }
}

function addToTextArea(image) {
    $(textareaClass).summernote('editor.restoreRange');
    $(textareaClass).summernote('editor.focus');
    $(textareaClass).summernote('editor.insertImage', 'https://kingflix.vn/Content/Upload/Images/' + image);
}


function getListImageSelect() {
    var imageAlbum = new Array;
    $('input[name="imageListSelect"]').each(function () {
        if ($(this).data('checked') == 'checked')
            imageAlbum.push($(this).val());
    });
    return imageAlbum;
}
function getSingleImageSelect() {
    var image = "";
    $('input[name="imageListSelect"]').each(function () {
        if ($(this).data('checked') == 'checked') {
            image = $(this).val();
            return false;
        }
    });
    return image;
}

//DeleteImage
$(document).on("click", ".edit-img", function () {
    var elm = $(this);
    $("#currentImageId").val(elm.data('id'));
    $("#newImageId").val(elm.data('id').split('.')[0]);
})

$(document).on("click", ".delete-img", function () {
    var elm = $(this);
    var r = confirm("Bạn có muốn xóa hình này không?");
    if (r == true) {
        $.ajax({
            method: "POST",
            url: "/Image/DeleteImage",
            data: { imageId: elm.data("id") },
            success: function (data) {
                if (data.status == "success") {
                    toastr.success(data.message);
                    elm.parent().parent().remove();
                }
                else {
                    toastr.error(data.message);
                }
            }
        })
    }
})
