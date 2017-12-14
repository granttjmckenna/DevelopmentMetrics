$(document).ready(function () {
    $("a.dataThroughputMenuItem").click(function () {
        var weeks = $(this).attr("data-seq");

        filterBuildThroughputChartByWeeks(weeks);
    });
});

$(document).ready(function () {
    $("a.throughputAgentsMenuItem").click(function () {
        var buildAgent = $(this).attr("data-seq");

        filterBuildThroughputChartByBuildAgent(buildAgent);
    });
});

$(document).ready(function () {
    $("a.throughputBuildTypeMenuItem").click(function () {
        var buildTypeId = $(this).attr("data-seq");

        filterBuildThroughputChartByBuildTypeId(buildTypeId);
    });
});

function filterBuildThroughputChartByWeeks(weeks) {
    setChartValue("numberOfWeeks", weeks);

    filterBuildThroughputChart();
};

function filterBuildThroughputChartByBuildAgent(buildAgent) {
    setChartValue("buildAgent", buildAgent);

    filterBuildThroughputChart();
};

function filterBuildThroughputChartByBuildTypeId(buildTypeId) {
    setChartValue("buildTypeId", buildTypeId);

    filterBuildThroughputChart();
};

function filterBuildThroughputChart() {
    var chartWeeks = getChartValue("numberOfWeeks");
    var filterByBuildAgent = getChartValue("buildAgent");
    var filterByBuildTypeId = getChartValue("buildTypeId");

    drawBuildThroughputChart(chartWeeks, filterByBuildAgent, filterByBuildTypeId);
};

function drawBuildThroughputChart(weeks, buildAgent, buildTypeId) {

    $.ajax({
        url: "/BuildThroughput/GetBuildThroughputChartDataFor",
        dataType: "json",
        data: {
            numberOfWeeks: weeks,
            buildAgent: buildAgent,
            buildTypeId: buildTypeId
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
        title: getBuildThroughputChartTitle(),
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
        colors: ["#000099", "#8080ff", "#33cc33", "#99e699"]
    };

    drawChart(dataTable, options);

    return false;
};

function getBuildThroughputChartTitle() {
    var weeks = getChartValue("numberOfWeeks");
    var buildAgent = getChartValue("buildAgent");
    var buildTypeId = getChartValue("buildTypeId");

    return "Build throughput - weeks: " + getChartTitleWeeks(weeks) + " & build agent: " + getChartTitleAgentName(buildAgent) + " & build type: " + getChartTitleBuildTypeId(buildTypeId);
};