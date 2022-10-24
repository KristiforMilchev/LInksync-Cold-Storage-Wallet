


var oneDelimiter = false;

var isNotificationOpen = false;
function OpenNotificationPanel() {
    isNotificationOpen = true;
    document.getElementById("NotificationPanel").click();
}


function logError(e) {

    // Get 'label' html control
    var label = document.getElementById("LABEL_ID");

    label.value = e;

}

 

function OpenSeedConfimr() {
    $("#Options").hide();
    $("#Preview").show();
    $("#pincode").hide();
    $("#ImportConfirm").hide();
}

function OpenImportConfirm() {
    $("#Options").hide();
    $("#Preview").hide();
    $("#pincode").hide();
    $("#ImportConfirm").show();


}

function ConfirmSeed() {
    $("#Preview").hide();
    $("#confirm").show();
    $("#pincode").hide();

}

function ReturnToSeedSave() {
    $("#Preview").show();
    $("#confirm").hide();
    $("#pincode").hide();
    $("#ImportConfirm").hide();
}

function SetPin() {

    $("#pincode").show();
    $("#confirm").hide();
    $("#Preview").hide();
    $("#ImportConfirm").hide();
}

function ImportTokens() {
    document.getElementById("ImportTokenSettings").style.setProperty("display", "");
    document.getElementById("SetupNetworkPanel").style.setProperty("display", "none");
    $("#SidebarMenu").click();
}


function ImportNewNetwork() {
    document.getElementById("ImportTokenSettings").style.setProperty("display", "none");
    document.getElementById("SetupNetworkPanel").style.setProperty("display", "");
    $("#SidebarMenu").click();
}

function FixPosition() {
    var transform = document.getElementById("dropdownNetworks").style.getPropertyValue("Transform");
    document.getElementById("dropdownNetworks").style.setProperty("postion", "absolute !important");
    document.getElementById("dropdownNetworks").style.setProperty("left", 0);
    document.getElementById("dropdownNetworks").style.setProperty("top", 0);

}

function GetImportWords() {
    var words = [];
    for (var i = 0; i <= 13; i++) {
        words.push($("#Val_" + i).val());
    }
    return words;
}

function RiseError(title, message)
{
    Swal.fire(title,message);
}

function InitDropdown()
{
    $(".select2").select2().on('select2:select', function (e) {
        console.log(e.params.data);
        DotNet.invokeMethodAsync('LInksync-Cold-Storage-Wallet', 'TokenSelected', e.params.data.id);

    });
}
var root;
function InitBalanceChart(marketData)
{
 
    var data = [];
    for(var item in marketData)
    {
        var x = marketData[item];
        data.push({
            date: new Date(x.date).getTime(),
            value: x.balance,
        });
    }
 
  
    if(root === undefined)
    {
        root = am5.Root.new("chartdiv");
        root.numberFormatter.set("numberFormat", "#,###.00000000000000000");
        root.setThemes([am5themes_Animated.new(root),   am5themes_Dark.new(root)]);

       
    }
 
    var chart = root.container.children.push(
        am5xy.XYChart.new(root, {
            focusable: true,
            panX: true,
            panY: true,
            wheelX: "panX",
            wheelY: "zoomX"
        })
    );
  
// Create chart
// https://www.amcharts.com/docs/v5/charts/xy-chart/
 
    


    
// Create axes
// https://www.amcharts.com/docs/v5/charts/xy-chart/axes/

// Create axes
// https://www.amcharts.com/docs/v5/charts/xy-chart/axes/
    var xAxis = chart.xAxes.push(am5xy.DateAxis.new(root, {
        baseInterval: { timeUnit: "day", count: 1 },
        renderer: am5xy.AxisRendererX.new(root, {}),
        tooltip: am5.Tooltip.new(root, {})
    }));

    var yAxis = chart.yAxes.push(am5xy.ValueAxis.new(root, {
        renderer: am5xy.AxisRendererY.new(root, {})
    }));

    var color = root.interfaceColors.get("background");

    // https://www.amcharts.com/docs/v5/charts/xy-chart/series/
    var series = chart.series.push(am5xy.LineSeries.new(root, {
        name: "Series",
        xAxis: xAxis,
        yAxis: yAxis,
        valueYField: "value",
        valueXField: "date",
        tooltip: am5.Tooltip.new(root, {
            labelText: "{valueY}"
        })
    }));

// Add cursor
// https://www.amcharts.com/docs/v5/charts/xy-chart/cursor/
    var cursor = chart.set(
        "cursor",
        am5xy.XYCursor.new(root, {
            xAxis: xAxis
        })
    );
    cursor.lineY.set("visible", false);

// Stack axes vertically
// https://www.amcharts.com/docs/v5/charts/xy-chart/axes/#Stacked_axes
    chart.leftAxesContainer.set("layout", root.verticalLayout);

    series.data.setAll(data);

    
// Make stuff animate on load
// https://www.amcharts.com/docs/v5/concepts/animations/
    series.appear(1000);
    chart.appear(1000, 100);

}

function LoadSocialMediaFeed(key, feedData)
{
    $("#"+key).html(feedData);
}
 
 