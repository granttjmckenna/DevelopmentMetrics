function loadBuildCache() {

    $.ajax({
        url: "/BuildStability/ReturnWhenBuildDataCached",
        dataType: "json",
        data: {},
        type: "GET",
        error: function () {
        },
        beforeSend: function () {
        },
        success: function (data) {
        },
        complete: function () {
        }
    });

    return false;
};