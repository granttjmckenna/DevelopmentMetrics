﻿function drawChart(days) {
    var chartDays = 42;

    if (days && typeof days == "number") {
        chartDays = days;
    };

    $.ajax({
        url: "/Cards/GetCardChartDataFor",
        dataType: "json",
        data: {
            numberOfDays: chartDays
        },
        type: "POST",
        error: function () {
            showChartMessage("Error retrieving chart data");
        },
        beforeSend: function () {
            showChartLoading();
        },
        success: function (data) {
            renderChartData(data);
        },
        complete: function () {
            showChartLoaded();
        }
    });

    return false;
};

function getChartDiv() {
    return document.getElementById("chart_div");
}

function showChartMessage(msg) {
    var cardChart = getChartDiv();

    cardChart.innerHTML = msg;
}

function showChartLoading() {
    var cardChart = getChartDiv();

    cardChart.classList.add("loading");

    showChartMessage("Loading");
}

function showChartLoaded() {
    var cardChart = getChartDiv();

    cardChart.classList.remove("loading");
    cardChart.classList.add("loaded");
}

function renderChartData(data) {
    var dataTable = new google.visualization.DataTable();
    dataTable.addColumn("date", "Day");
    dataTable.addColumn("number", "Done");
    dataTable.addColumn("number", "Total");
    dataTable.addColumn("number", "Rework (%)");

    $.each(data,
        function (i, item) {
            dataTable.addRows([[new Date(getDateIfDate(item.Date)), item.DoneTotal, item.Total, item.DefectRate]]);
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
        vAxes: {
            0: {
                viewWindowMode: "explicit",
                gridlines: {
                    color: "transparent"
                },
                title: "Number of items",
                titleTextStyle: {
                    fontSize: 20,
                    italic: false
                }
            },
            1: {
                gridlines: {
                    color: "transparent"
                },
                title: "Rework",
                titleTextStyle: {
                    fontSize: 20,
                    italic: false
                },
                format: "#%",
                viewWindow: {
                    max: 1,
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
                lineWidth: 2
            }
        },
        colors: ["#34A853", "#FF6600", "#FF0000"]
    };

    var chartDiv = getChartDiv();

    var chart = new google.visualization.LineChart(chartDiv);

    // Wait for the chart to finish drawing before calling the getImageURI() method.
    google.visualization.events.addListener(chart, "ready", function () {
        document.getElementById("print_chart").innerHTML = "<a href='" + chart.getImageURI() + "'><p><span class='glyphicon glyphicon-print'></span></p></a>";
    });

    chart.draw(dataTable, options);

    return false;
}

function getDateIfDate(d) {
    var m = d.match(/\/Date\((\d+)\)\//);
    return m
        ? (new Date(+m[1])).toString("dd/MM/yyyy")
        : d;
};