﻿function filterBuildChartByWeeks(weeks) {
    var input = getElementById("numberOfWeeks");

    input.value = weeks;

    filterBuildChart();
};

function filterBuildChartByBuildAgent(buildAgent) {
    var input = getElementById("buildAgent");

    input.value = buildAgent;

    filterBuildChart();
};

function filterBuildChartByBuildTypeId(buildTypeId) {
    var input = getElementById("buildTypeId");

    input.value = buildTypeId;

    filterBuildChart();
};

function filterBuildChart() {
    var chartWeeks = getChartValue("numberOfWeeks");
    var filterByBuildAgent = getChartValue("buildAgent");
    var filterByBuildTypeId = getChartValue("buildTypeId");

    drawBuildChart(chartWeeks, filterByBuildAgent, filterByBuildTypeId);
};

function drawBuildChart(weeks, buildAgent, buildTypeId) {
    setChartValues(weeks, buildAgent, buildTypeId);

    $.ajax({
        url: "/BuildStability/GetBuildChartDataFor",
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
    dataTable.addColumn("number", "Failure rate");
    dataTable.addColumn("number", "Recovery time");
    dataTable.addColumn("number", "Recovery time (std dev)");

    $.each(data,
        function (i, item) {
            dataTable.addRows([[new Date(getDateIfDate(item.Date)), item.FailureRate, item.RecoveryTime, item.RecoveryTimeStdDev]]);
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
        vAxes: {
            0: {
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
            1: {
                gridlines: {
                    color: "transparent"
                },
                title: "Recovery time (hours)",
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
                targetAxisIndex: 1,
                lineWidth: 3
            },
            2: {
                targetAxisIndex: 1,
                lineWidth: 3
            }
        },
        colors: ["#FF0000", "#34A853", "#FF6600"]
    };

    drawChart(dataTable, options);

    return false;
};

function getBuildChartTitle() {
    var weeks = getChartValue("numberOfWeeks");
    var buildAgent = getChartValue("buildAgent");
    var buildTypeId = getChartValue("buildTypeId");

    return "Build stability - weeks: " + getChartTitleWeeks(weeks) + " & build agent: " + getChartTitleAgentName(buildAgent) + " & build type: " + getChartTitleBuildTypeId(buildTypeId);
};

function getChartTitleAgentName(agentName) {
    if (agentName === "All") {
        return agentName;
    };
    return "TC-A" + agentName.replace("lon-devtca", "");
}

function getChartTitleWeeks(weeks) {
    switch (weeks) {
        case -1:
            return "6";
        case -2:
            return "All";
        default:
            return weeks;
    }
};

function getChartTitleBuildTypeId(buildTypeId) {
    switch (buildTypeId) {
    case -1:
        return "All";
    default:
        return buildTypeId;
    }
};

function setChartValues(weeks, buildAgent, buildTypeId) {
    var input = getElementById("numberOfWeeks");

    input.value = weeks;

    input = getElementById("buildAgent");

    input.value = buildAgent;

    input = getElementById("buildTypeId");

    input.value = buildTypeId;
};

function getChartValue(id) {
    var input = getElementById(id);

    return input.value;
};;