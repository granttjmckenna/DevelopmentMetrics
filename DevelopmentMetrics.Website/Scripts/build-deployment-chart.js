$(document).ready(function () {
    $("a.dataDeploymentMenuItem").click(function () {
        var weeks = $(this).attr("data-seq");

        filterBuildDeploymentChartByWeeks(weeks);
    });
});

$(document).ready(function () {
    $("a.deploymentAgentsMenuItem").click(function () {
        var buildAgent = $(this).attr("data-seq");

        filterBuildDeploymentChartByBuildAgent(buildAgent);
    });
});

$(document).ready(function () {
    $("a.deploymentBuildTypeMenuItem").click(function () {
        var buildTypeId = $(this).attr("data-seq");

        filterBuildDeploymentChartByBuildTypeId(buildTypeId);
    });
});

function filterBuildDeploymentChartByWeeks(weeks) {
    setChartValue("numberOfWeeks", weeks);

    filterBuildDeploymentChart();
};

function filterBuildDeploymentChartByBuildAgent(buildAgent) {
    setChartValue("buildAgent", buildAgent);

    filterBuildDeploymentChart();
};

function filterBuildDeploymentChartByBuildTypeId(buildTypeId) {
    setChartValue("buildTypeId", buildTypeId);

    filterBuildDeploymentChart();
};

function filterBuildDeploymentChart() {
    var chartWeeks = getChartValue("numberOfWeeks");
    var filterByBuildAgent = getChartValue("buildAgent");
    var filterByBuildTypeId = getChartValue("buildTypeId");

    drawBuildDeploymentChart(chartWeeks, filterByBuildAgent, filterByBuildTypeId);
};

function drawBuildDeploymentChart(weeks, buildAgent, buildTypeId) {

    $.ajax({
        url: "/BuildDeployment/GetBuildChartDataFor",
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

    //TODO: check values
    //TODO: set axis values etc

    $.each(data,
        function (i, item) {
            dataTable.addRows([[new Date(getDateIfDate(item.Date)), item.IntervalTime, item.IntervalTimeStdDev, item.DurationTime, item.DurationTimeStdDev]]);
        });

    var options = {
        title: getBuildDeploymentChartTitle(),
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

function getBuildDeploymentChartTitle() {
    var weeks = getChartValue("numberOfWeeks");
    var buildAgent = getChartValue("buildAgent");
    var buildTypeId = getChartValue("buildTypeId");

    return "Build deployment - weeks: " + getChartTitleWeeks(weeks) + " & build agent: " + getChartTitleAgentName(buildAgent) + " & build type: " + getChartTitleBuildTypeId(buildTypeId);
};