function setChartValue(id, value) {
    var input = getElementById(id);

    input.value = value;
};

function getChartValue(id) {
    var input = getElementById(id);

    return input.value;
};

function getElementById(id) {
    return document.getElementById(id);
};

function getChartDiv() {
    return getElementById("chart_div");
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

    // Wait for the chart to finish drawing before calling the getImageURI() method.
    google.visualization.events.addListener(chart, "ready", function () {
        document.getElementById("print_chart").innerHTML = "<a href='" + chart.getImageURI() + "'><p><span class='glyphicon glyphicon-print'></span></p></a>";
    });

    chart.draw(dataTable, options);
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