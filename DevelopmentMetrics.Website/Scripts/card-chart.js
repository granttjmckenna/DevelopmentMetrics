﻿function drawChart(days) {
    var chartDays = 42;

    if (days && typeof days == "number") {
        chartDays = days;
    };

    $.ajax({
        url: "/Cards/GetCardCountByDay",
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
    dataTable.addColumn('date', 'day');
    dataTable.addColumn('number', 'Done');
    dataTable.addColumn('number', 'Total');

    $.each(data,
        function (i, item) {
            dataTable.addRows([[new Date(getDateIfDate(item.Date)), item.DoneTotal, item.Total]]);
        });

    var minValue = dataTable.getFormattedValue(0, 2) - 30;

    var options = {
        title: 'Cumulative flow diagram',
        titleTextStyle: {
            italic: false,
            color: '#00BBF1',
            fontSize: '20'
        },
        height: 600,
        pointSize: 3,
        curveType: 'function',
        chartArea: {
            top: 50
        },
        legend:
        {
            position: 'bottom',
            textStyle:
            {
                color: '#666'
            }
        },
        colors: ['#34A853', 'ff6600', '#FBBC05'],
        hAxis:
        {
            title: 'Time (days)',
            titleTextStyle:
            {
                italic: false,
                color: '#00BBF1',
                fontSize: '20'
            },
            textStyle:
            {
                color: '#666'
            },
            format: 'dd MMM'
        },
        vAxis:
        {
            baselineColor: '#f5f5f5',
            title: 'Number of items',
            titleTextStyle:
            {
                color: '#00BBF1',
                italic: false,
                fontSize: '20'
            },
            textStyle:
            {
                color: '#666'
            },
            viewWindow:
            {
                min: minValue,
                format: 'long'
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