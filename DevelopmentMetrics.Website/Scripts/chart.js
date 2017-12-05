function getChartDiv() {
    return document.getElementById("chart_div");
};

function showChartMessage(msg) {
    var cardChart = getChartDiv();

    cardChart.innerHTML = msg;
};

function showChartLoading() {
    var cardChart = getChartDiv();

    cardChart.classList.add("loading");

    showChartMessage("Loading");
};

function showChartLoaded() {
    var cardChart = getChartDiv();

    cardChart.classList.remove("loading");
    cardChart.classList.add("loaded");
};

function getDateIfDate(d) {
    var m = d.match(/\/Date\((\d+)\)\//);
    return m
        ? (new Date(+m[1])).toString("dd/MM/yyyy")
        : d;
};

function drawChart(dataTable, options) {
    var chartDiv = getChartDiv();

    var chart = new google.visualization.LineChart(chartDiv);

    chart.draw(dataTable, options);
};
