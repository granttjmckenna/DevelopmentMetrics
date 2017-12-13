$(document).ready(function () {
    $("a.dataMenuItem").click(function () {
        var weeks = $(this).attr("data-seq");

        filterBuildChartByWeeks(weeks);
    });
});

$(document).ready(function () {
    $("a.agentsMenuItem").click(function () {
        var buildAgent = $(this).attr("data-seq");

        filterBuildChartByBuildAgent(buildAgent);
    });
});

$(document).ready(function () {
    $("a.buildsMenuItem").click(function () {
        var buildTypeId = $(this).attr("data-seq");

        filterBuildChartByBuildTypeId(buildTypeId);
    });
});

function filterBuildChartByWeeks(weeks) {
    setChartValue("numberOfWeeks", weeks);

    filterBuildChart();
};

function filterBuildChartByBuildAgent(buildAgent) {
    setChartValue("buildAgent", buildAgent);

    filterBuildChart();
};

function filterBuildChartByBuildTypeId(buildTypeId) {
    setChartValue("buildTypeId", buildTypeId);

    filterBuildChart();
};

function filterBuildChart() {
    var chartWeeks = getChartValue("numberOfWeeks");
    var filterByBuildAgent = getChartValue("buildAgent");
    var filterByBuildTypeId = getChartValue("buildTypeId");

    drawBuildChart(chartWeeks, filterByBuildAgent, filterByBuildTypeId);
};

function drawBuildChart(weeks, buildAgent, buildTypeId) {

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

function setChartValue(id, value) {
    var input = getElementById(id);

    input.value = value;
};

function getChartValue(id) {
    var input = getElementById(id);

    return input.value;
};

function showBuilds(numberOfItems) {
    hideAllItems();

    if (numberOfItems === -1) {
        showAllItems();
    } else {
        for (var i = 1; i <= numberOfItems; i++) {
            showBuildItem("failingBuild_" + i);
            showBuildItem("passingBuild_" + i);
        }
    }
};

function hideAllItems() {
    $(".buildItem").each(function () {
        $(this).removeClass("showBuildItem").addClass("hideBuildItem");
    });
};

function showAllItems() {
    $(".buildItem").each(function () {
        $(this).removeClass("hideBuildItem").addClass("showBuildItem");
    });
};

function showBuildItem(id) {
    $("#" + id).removeClass("hideBuildItem").addClass("showBuildItem");
};