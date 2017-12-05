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

function renderBuildChartData(data) {
    var dataTable = new google.visualization.DataTable();
    dataTable.addColumn("date", "Day");
    dataTable.addColumn("number", "FailureRate");

    $.each(data,
        function (i, item) {
            dataTable.addRows([[new Date(getDateIfDate(item.Date)), item.Rate]]);
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

    drawChart(dataTable, options);

    return false;
};
