function drawChart(days) {
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
    return $("#chart_div");
}

function showChartMessage(msg) {
    var cardChart = getChartDiv();

    cardChart.text(msg);
}

function showChartLoading() {
    var cardChart = getChartDiv();

    cardChart.addClass("loading");

    showChartMessage("Loading");
}

function showChartLoaded() {
    var cardChart = getChartDiv();

    cardChart.removeClass("loading").addClass("loaded");
}

function renderChartData(data) {
    var dataTable = new google.visualization.DataTable();
    dataTable.addColumn('date', 'Day');
    dataTable.addColumn('number', "Done");
    dataTable.addColumn('number', "Total");
    dataTable.addColumn('number', "Defect rate (%)");

    $.each(data,
        function (i, item) {
            dataTable.addRows([[new Date(getDateIfDate(item.Date)), item.DoneTotal, item.Total, item.DefectRate]]);
        });

    var options = {
        title: 'Cumulative flow diagram',
        curveType: 'function',
        hAxis: {
            showTextEvery: 1,
            gridlines: {
                color: 'transparent'
            },
            title: 'Date'
        },
        vAxes: {
            0: {
                viewWindowMode: 'explicit',
                gridlines: {
                    color: 'transparent'
                },
                title: 'Number of items'
            },
            1: {
                gridlines: {
                    color: 'transparent'
                },
                title: 'Defect rate',
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
                lineWidth: 2
            },
            1: {
                targetAxisIndex: 0,
                lineWidth: 2
            },
            2: {
                targetAxisIndex: 1,
                lineWidth: 1
            }
        },
        colors: ['#34A853', '#FF6600', '#FF0000']
    };

    var chart = new google.visualization.LineChart(document.getElementById("chart_div"));
    chart.draw(dataTable, options);

    return false;
}

function getDateIfDate(d) {
    var m = d.match(/\/Date\((\d+)\)\//);
    return m
        ? (new Date(+m[1])).toString('dd/MM/yyyy')
        : d;
};