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
    data.addColumn('date', 'Day');
    data.addColumn('number', "Done");
    data.addColumn('number', "Total");
    data.addColumn('number', "Defects");

    $.each(data,
        function (i, item) {
            dataTable.addRows([[new Date(getDateIfDate(item.Date)), item.DoneTotal, item.Total, item.DefectRate]]);
        });

    var options = {
        title: 'Cumulative flow diagram',
        width: 900,
        height: 500,
        series: {
            // Gives each series an axis name that matches the Y-axis below.
            0: { axis: 'NumberOfItems' },
            1: { axis: 'NumberOfItems' },
            2: { axis: 'Defects' }
        },
        colors: ['#34A853', '#FF6600', '#FF0000'],
        axes: {
            y: {
                NumberOfItems: { label: 'Number Of Items' },
                Defects: {
                    label: 'Defects (%)',
                    range: {
                        min: 0,
                        max: 100
                    }
                }
            }
        }
    };

    var chart = new google.visualization.LineChart(getChartDiv());
    chart.draw(dataTable, options);

    return false;
}

function getDateIfDate(d) {
    var m = d.match(/\/Date\((\d+)\)\//);
    return m
        ? (new Date(+m[1])).toString('dd/MM/yyyy')
        : d;
};