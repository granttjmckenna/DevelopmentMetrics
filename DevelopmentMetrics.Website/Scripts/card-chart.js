function drawCardChart(days) {
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
            renderChartData(data, chartDays);
        },
        complete: function () {
            showChartLoaded();
        }
    });

    return false;
};

function renderChartData(data, chartDays) {
    var dataTable = new google.visualization.DataTable();
    dataTable.addColumn("date", "Day");
    dataTable.addColumn("number", "Done");
    dataTable.addColumn("number", "Total");
    dataTable.addColumn("number", "Rework (%)");

    $.each(data,
        function (i, item) {
            dataTable.addRows([[new Date(getDateIfDate(item.Date)), item.DoneTotal, item.Total, item.Rate]]);
        });

    var options = {
        title: getBuildChartTitle(chartDays),
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

    drawChart(dataTable, options);

    return false;
};

function getBuildChartTitle(days) {
    return "Cumulative flow diagram - days: " + days;
};