function drawBuildChart() {

    $.ajax({
        url: "/BuildStability/GetBuildChartDataFor",
        dataType: "json",
        data: {
            numberOfWeeks: 6
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

function renderBuildChartData(data) {
    var dataTable = new google.visualization.DataTable();
    dataTable.addColumn("date", "Day");
    dataTable.addColumn("number", "FailureRate");

    $.each(data,
        function (i, item) {
            dataTable.addRows([[new Date(getDateIfDate(item.Date)), item.FailureRate]]);
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
        vAxis: {
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
        colors: ["#FF0000"]
    };

    var chartDiv = getChartDiv();

    var chart = new google.visualization.LineChart(chartDiv);
    
    chart.draw(dataTable, options);

    return false;
}

function getDateIfDate(d) {
    var m = d.match(/\/Date\((\d+)\)\//);
    return m
        ? (new Date(+m[1])).toString("dd/MM/yyyy")
        : d;
};