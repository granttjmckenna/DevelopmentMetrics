function loadBuildCache() {

    $.ajax({
        url: "/BuildStability/ReturnWhenBuildDataCached",
        type: "GET",
        complete: function () {
            $(".loadingCache").toggle();
            $(".loadedCache").toggle();
        }
    });

    return false;
};