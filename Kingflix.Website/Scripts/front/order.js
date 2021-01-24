// Step1


//Payment Information
var clipboard = new ClipboardJS('.clipboard');
clipboard.on('success', function (e) {
    toastr.success("Đã sao chép vào bộ nhớ tạm");
});

function uploadImageSuccess(data) {
    if (data.status == "success") {
        toastr.success(data.message);
        $("#uploadText").html("Upload thành công!");
        $(".upload-show").empty();
    }
    else {
        toastr.error(data.message);
    }
}

$(document).on("click", "#btnid", function (event) {
    event.preventDefault();
    var fileOptions = {
        success: res,
        dataType: "json"
    }
    $("#formid").ajaxSubmit(fileOptions);
});


