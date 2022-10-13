var oneDelimiter = false;
document.addEventListener('keyup', function (e) {
    InputKey(e.key);
});

function removeFlex() {
    document.getElementById("PageHeader").style.setProperty("display", "block");
}

function ReturnBack() {
    window.history.go(-1);

}


function KeyboardToggle(state) {
    if (state == "true")
        document.getElementById("VirtualKeyboard").classList.add("visibleMenu");
    else
        document.getElementById("VirtualKeyboard").classList.remove("visibleMenu");

    return "OK";
}




var selectedNetowrkId;
var selectedChain = 97;
var selectedContract = "0x78867BbEeF44f2326bF8DDd1941a4439382EF2A7";
function MetworkChanged(args) {
    var parsed = JSON.parse(args);
    document.getElementById("networkLogo").setAttribute("src", parsed.Logo);
    selectedChain = parsed.NetworkId;
    selectedNetowrkId = undefined;
    selectedContract = undefined;
    console.log(args);

}

function CurrencyChanged(args) {
    var parsed = JSON.parse(args);
    document.getElementById("currencyLogo").setAttribute("src", parsed.Logo);
    selectedNetowrkId = parsed.NetworkId;
    selectedContract = parsed.ContractAddress;
    console.log(args);
    UpdateNetwork(selectedChain, selectedContract);
}

var cardAmount;
function SetCardAmount() {
    cardAmount = $("#price").html();
    document.getElementById("submitTrigger").click();
}

function GetCardAmount() {
    return cardAmount;
}

var isNotificationOpen = false;
function OpenNotificationPanel() {
    isNotificationOpen = true;
    document.getElementById("NotificationPanel").click();
}



document.addEventListener('touchstart', handleTouchStart, false);
document.addEventListener('touchmove', handleTouchMove, false);

var xDown = null;
var yDown = null;

function getTouches(evt) {
    return evt.touches ||             // browser API
        evt.originalEvent.touches; // jQuery
}

function handleTouchStart(evt) {
    const firstTouch = getTouches(evt)[0];
    xDown = firstTouch.clientX;
    yDown = firstTouch.clientY;
};

function handleTouchMove(evt) {
    if (!xDown || !yDown) {
        return;
    }

    var xUp = evt.touches[0].clientX;
    var yUp = evt.touches[0].clientY;

    var xDiff = xDown - xUp;
    var yDiff = yDown - yUp;

    if (Math.abs(xDiff) > Math.abs(yDiff)) {/*most significant*/
        if (xDiff > 0) {
            /* right swipe */


        } else {
            /* left swipe */
            if (isNotificationOpen) {
                isNotificationOpen = false;
                document.getElementById("closeHidden").click();

            }
        }
    } else {
        if (yDiff > 0) {
            /* down swipe */
        } else {
            /* up swipe */
        }
    }
    /* reset values */
    xDown = null;
    yDown = null;
};


function ToggleLock(args) {
    if (args === 1) {
        document.getElementById("unlockBtn").style.setProperty("display", "none");
        document.getElementById("lockBtn").style.setProperty("display", "");
        // Send .net command to unlock the door.
    }
    else {
        document.getElementById("unlockBtn").style.setProperty("display", "");
        document.getElementById("lockBtn").style.setProperty("display", "none");
        // Send .net command to lock the door.

    }
    return "ok"
}
var resultContainer = document.getElementById('qr-reader-results');
var lastResult, countResults = 0;

function onScanSuccess(decodedText, decodedResult) {
    if (decodedText !== lastResult) {
        ++countResults;
        lastResult = decodedText;
        // Handle on success condition with the decoded message.
        console.log(`Scan result ${decodedText}`, decodedResult);
    }
}

function StartCam() {
    //Call the API to retrive the door camera

    var html5QrcodeScanner = new Html5QrcodeScanner(
        "qr-reader", { fps: 10, qrbox: 450 });
    html5QrcodeScanner.render(onScanSuccess);
}


function requestMedia() {

    // Create request options
    let options = {
        audio: true,
        video: true
    };


    // Warning: Below commented code causes "Illegal invocation error"
    //// Set up proper function
    //navigator.mediaDevices.getUserMedia =
    //    navigator.getUserMedia ||
    //    navigator.webkitGetUserMedia ||
    //    navigator.mozGetUserMedia ||
    //    navigator.msGetUserMedia;

    try {
        // Request user media
        navigator.mediaDevices
            .getUserMedia(options)
            .then(gotLocalStream)
            .catch(logError);
    }
    catch (e) {
        logError(e);
    }

}

// Receives local stream data
function gotLocalStream(stream) {

    // Get 'video' html control
    var videoLocalPanel = document.getElementById("LOCAL_VIDEO_ID");

    if ("srcObject" in videoLocalPanel) {
        videoLocalPanel.srcObject = stream;
    } else {
        videoLocalPanel.src = window.URL.createObjectURL(stream);
    }

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

function InitBalanceChart(marketData)
{

 
    var data = [];
    for(var item in marketData)
    {
        var x = marketData[item];
        data.push({
            date: new Date(x.date).getTime(),
            value: x.close,
            open: x.open,
            low: x.low,
            high: x.high
        });
    }
   var example = generateChartData();
// Create root element
// https://www.amcharts.com/docs/v5/getting-started/#Root_element
    var root = am5.Root.new("chartdiv");
    root.numberFormatter.set("numberFormat", "#,###.00000000000000000");

// Set themes
// https://www.amcharts.com/docs/v5/concepts/themes/
    root.setThemes([am5themes_Animated.new(root)]);

    function generateChartData() {
        var chartData = [];
        var firstDate = new Date();
        firstDate.setDate(firstDate.getDate() - 1000);
        firstDate.setHours(0, 0, 0, 0);
        var value = 1200;
        for (var i = 0; i < 5000; i++) {
            var newDate = new Date(firstDate);
            newDate.setDate(newDate.getDate() + i);

            value += Math.round((Math.random() < 0.5 ? 1 : -1) * Math.random() * 10);
            var open = value + Math.round(Math.random() * 16 - 8);
            var low = Math.min(value, open) - Math.round(Math.random() * 5);
            var high = Math.max(value, open) + Math.round(Math.random() * 5);

            chartData.push({
                date: newDate.getTime(),
                value: value,
                open: open,
                low: low,
                high: high
            });
        }
        return chartData;
    }


    
// Create chart
// https://www.amcharts.com/docs/v5/charts/xy-chart/
    var chart = root.container.children.push(
        am5xy.XYChart.new(root, {
            focusable: true,
            panX: true,
            panY: true,
            wheelX: "panX",
            wheelY: "zoomX"
        })
    );
 
// Create axes
// https://www.amcharts.com/docs/v5/charts/xy-chart/axes/
    var xAxis = chart.xAxes.push(
        am5xy.DateAxis.new(root, {
            groupData: true,
            maxDeviation:0.1,
            baseInterval: { timeUnit: "minute", count: 1 },
            renderer: am5xy.AxisRendererX.new(root, {pan:"zoom"}),
            tooltip: am5.Tooltip.new(root, {})
        })
    );

    var yAxis = chart.yAxes.push(
        am5xy.ValueAxis.new(root, {
            maxDeviation:1,
            renderer: am5xy.AxisRendererY.new(root, {pan:"zoom"})
        })
    );

    var color = root.interfaceColors.get("background");

// Add series
// https://www.amcharts.com/docs/v5/charts/xy-chart/series/
    var series = chart.series.push(
        am5xy.CandlestickSeries.new(root, {
            fill: color,
            calculateAggregates: true,
            stroke: color,
            name: "MDXI",
            xAxis: xAxis,
            yAxis: yAxis,
            valueYField: "value",
            openValueYField: "open",
            lowValueYField: "low",
            highValueYField: "high",
            valueXField: "date",
            lowValueYGrouped: "low",
            highValueYGrouped: "high",
            openValueYGrouped: "open",
            valueYGrouped: "close",
            legendValueText:
                "open: {openValueY} low: {lowValueY} high: {highValueY} close: {valueY}",
            legendRangeValueText: "{valueYClose}",
            tooltip: am5.Tooltip.new(root, {
                pointerOrientation: "horizontal",
                labelText: "open: {openValueY}\nlow: {lowValueY}\nhigh: {highValueY}\nclose: {valueY}"
            })
        })
    );

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

 

    
    
// set data
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

function  TrackerCurrencyChanged(args)
{
    console.log(args);
}

 