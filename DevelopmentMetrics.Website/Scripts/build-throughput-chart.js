$(document).ready(function () {
    $("a.dataThroughputMenuItem").click(function () {
        var weeks = $(this).attr("data-seq");

        drawBuildThroughputChart(weeks);
    });
});

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
    dataTable.addColumn("number", "Build Interval");
    dataTable.addColumn("number", "Build Interval (std dev)");
    dataTable.addColumn("number", "Build Duration");
    dataTable.addColumn("number", "Build Duration (std dev)");

    $.each(data,
        function (i, item) {
            dataTable.addRows([[new Date(getDateIfDate(item.Date)), item.BuildIntervalTime, item.BuildIntervalTimeStdDev, item.BuildDurationTime, item.BuildDurationTimeStdDev]]);
        });

    var options = {
        title: "Build throughput",
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
        vAxes: {
            0: {
                gridlines: {
                    color: "transparent"
                },
                title: "Build Interval (hours)",
                titleTextStyle: {
                    fontSize: 20,
                    italic: false
                },
                format: "short",
                viewWindow: {
                    min: 0
                }
            },
            1: {
                gridlines: {
                    color: "transparent"
                },
                title: "Build Duration (minutes)",
                titleTextStyle: {
                    fontSize: 20,
                    italic: false
                },
                format: "short",
                viewWindow: {
                    min: 0
                }
            }
        },
        series: {
            0: {
                targetAxisIndex: 0,
                lineWidth: 3
            },
            1: {
                targetAxisIndex: 0,
                lineWidth: 3
            },
            2: {
                targetAxisIndex: 1,
                lineWidth: 3
            },
            3: {
                targetAxisIndex: 1,
                lineWidth: 3
            }
        },
        colors: ["#FF0000", "#34A853", "#FF6600", "#34BB35"]
    };

    drawChart(dataTable, options);

    return false;
};