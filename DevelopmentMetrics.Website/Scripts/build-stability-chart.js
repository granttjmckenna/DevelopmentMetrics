﻿$(document).ready(function () {
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

$(document).ready(function () {
    $("a.showBuildsMenuItem").click(function () {
        var numberOfItems = $(this).attr("data-seq");

        if (numberOfItems === "-1") {
            showAllItems();
        } else {
            showBuilds(numberOfItems);
        }
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
            renderBuildData(data);
        },
        complete: function () {
            showChartLoaded();
        }
    });

    return false;
};

function renderBuildData(data) {
    renderBuildChartData(data);

    renderBuildIgnoredTestsData(data);
}

function renderBuildIgnoredTestsData(data) {
    var dataTable = new google.visualization.DataTable();
    dataTable.addColumn("date", "Day");
    dataTable.addColumn("number", "Ignored test count");

    $.each(data,
        function (i, item) {
            dataTable.addRows([[new Date(getDateIfDate(item.Date)), item.IgnoredTestCount]]);
        });

    var options = {
        height: 200,
        hAxis: {
            gridlines: {
                color: "transparent"
            },
            title: "Ignored tests (count)",
            minValue: 0,
            titleTextStyle: {
                fontSize: 20,
                italic: false
            }
        },
        vAxis: {
            gridlines: {
                color: "transparent"
            },
            type: "category",
            title: "Date",
            format: "dd MMM",
            titleTextStyle: {
                fontSize: 20,
                italic: false
            }
        },
        legend: {
            position: "none"
        }
    };

    var chart = new google.visualization.BarChart(document.getElementById("chart_IgnoredTests"));

    chart.draw(dataTable, options);
}

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
            type: "category",
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
                title: "Build Failure Rate (%)",
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
                title: "Build Failure Recovery Time (hours)",
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

function formatDate(date) {
    var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    var day = date.getDate();
    var monthIndex = date.getMonth();
    var year = date.getFullYear();

    return day + ' ' + monthNames[monthIndex] + ' ' + year;
}