function filterBuildChart(buildAgent) {
    var weeks = getChartValue("numberOfWeeks");

    drawBuildChart(weeks, buildAgent);
};

function drawBuildChart(weeks, buildAgent) {
    var chartWeeks = 6;
    var filterByBuildAgent = "All";

    if (weeks && typeof weeks == "number") {
        chartWeeks = weeks;
    };

    if (buildAgent && typeof buildAgent == "string") {
        filterByBuildAgent = buildAgent;
    };

    $.ajax({
        url: "/BuildStability/GetBuildChartDataFor",
        dataType: "json",
        data: {
            numberOfWeeks: chartWeeks,
            buildAgent: filterByBuildAgent
        },
        type: "POST",
        error: function () {
            showChartMessage("Error retrieving chart data");
        },
        beforeSend: function () {
            showChartLoading();
        },
        success: function (data) {
            renderBuildChartData(data);
        },
        complete: function () {
            showChartLoaded();
        }
    });

    return false;
};

function renderBuildChartData(data) {
    var dataTable = new google.visualization.DataTable();
    dataTable.addColumn("date", "Day");
    dataTable.addColumn("number", "FailureRate");

    $.each(data,
        function (i, item) {
            dataTable.addRows([[new Date(getDateIfDate(item.Date)), item.Rate]]);
        });

    var options = {
        curveType: "function",
        legend: {
            position: "bottom"
        },
        hAxis: {
            gridlines: {
                color: "transparent"
            },
            title: "Date",
            format: "dd MMM",
            titleTextStyle: {
                fontSize: 20,
                italic: false
            }
        },
        vAxis: {
            gridlines: {
                color: "transparent"
            },
            title: "Failure rate",
            titleTextStyle: {
                fontSize: 20,
                italic: false
            },
            format: "#%",
            viewWindow: {
                max: 1,
                min: 0
            }
        },
        colors: ["#FF0000"]
    };

    drawChart(dataTable, options);

    return false;
};

function setChartValues(weeks, buildAgent) {
    var input = getElementById("numberOfWeeks");

    input.val = weeks;

    input = getElementById("buildAgent");

    input.val = buildAgent;
};

function getChartValue(id) {
    var input = getElementById(id);

    return input.val;
};