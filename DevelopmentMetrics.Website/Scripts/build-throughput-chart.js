function filterBuildThroughputChart() {
    drawBuildThroughputChart(6);
};

function drawBuildThroughputChart(weeks) {

    $.ajax({
        url: "/BuildThroughput/GetBuildThroughputChartDataFor",
        dataType: "json",
        data: {
            numberOfWeeks: weeks
        },
        type: "POST",
        error: function () {
            showChartMessage("Error retrieving chart data");
        },
        beforeSend: function () {
            showChartLoading();
        },
        success: function (data) {
            renderBuildThroughputChartData(data);
        },
        complete: function () {
            showChartLoaded();
        }
    });

    return false;
};

function renderBuildThroughputChartData(data) {
    var dataTable = new google.visualization.DataTable();
    dataTable.addColumn("date", "Day");
    dataTable.addColumn("number", "Build Interval Time");
    dataTable.addColumn("number", "Build Interval Time (std dev)");

    $.each(data,
        function (i, item) {
            dataTable.addRows([[new Date(getDateIfDate(item.Date)), item.BuildIntervalTime, item.BuildIntervalTimeStdDev]]);
        });

    var options = {
        title: getBuildChartTitle(),
        titleTextStyle: {
            fontSize: 20,
            italic: false
        },
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
            title: "Hours",
            titleTextStyle: {
                fontSize: 20,
                italic: false
            }
        },
        colors: ["#FF0000", "#34A853", "#FF6600"]
    };

    drawChart(dataTable, options);

    return false;
};