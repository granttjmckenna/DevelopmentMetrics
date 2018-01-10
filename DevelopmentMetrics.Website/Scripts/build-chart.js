function loadBuildCache() {

    $.ajax({
        url: "/BuildStability/ReturnWhenBuildDataCached",
        type: "GET",
        complete: function () {
            $(".Builds-LoadingCache").toggle();
            $(".Builds-LoadedCache").toggle();
        }
    });

    return false;
};