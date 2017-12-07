function filterBuildChart(buildAgent) {
    var weeks = getChartValue("numberOfWeeks");

    drawBuildChart(weeks, buildAgent);
};

function drawBuildChart(weeks, buildAgent) {
    var chartWeeks = getChartValue("numberOfWeeks");
    var filterByBuildAgent = getChartValue("buildAgent");

    if (weeks && typeof weeks == "number") {
        chartWeeks = weeks;
    };

    if (buildAgent && typeof buildAgent == "string") {
        filterByBuildAgent = buildAgent;
    };

    setChartValues(chartWeeks, filterByBuildAgent);

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

    return "Build stability - weeks: " + getChartTitleWeeks(weeks) + " & build agent: " + getChartTitleAgentName(buildAgent);
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

function setChartValues(weeks, buildAgent) {
    var input = getElementById("numberOfWeeks");

    input.value = weeks;

    input = getElementById("buildAgent");

    input.value = buildAgent;
};

function getChartValue(id) {
    var input = getElementById(id);

    return input.value;
};;