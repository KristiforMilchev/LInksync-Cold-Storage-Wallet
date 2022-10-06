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


function RenderChart() {

    // -----------------------------------------------------------------------
    // Realtime chart
    // -----------------------------------------------------------------------

    var options_Android_Vs_iOS = {
        series: [{
            name: "Growth ",
            data: [8, 1, 1, 10, 11, 6, 12, 14, 21, 15, 21, 24, 28, 23, 34, 38, 41, 47]
        }, {
            name: "Loss ",
            data: [11, 4, 3, 14, 9, 10, 18, 15, 24, 17, 19, 26, 31, 26, 37, 41, 46, 51]
        },],
        chart: {
            height: 300,
            type: 'area',
            stacked: false,
            fontFamily: 'Rubik,sans-serif',
            zoom: {
                enabled: false
            },
            toolbar: {
                show: false
            },
            sparkline: {
                enabled: true
            }
        },
        colors: ['#706e6e', '#706e6e'],
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: false
        },
        markers: {
            size: 2,
            strokeColors: 'transparent',
            colors: '#706e6e',
        },
        fill: {
            type: 'solid',
            colors: ['#706e6e', '#706e6e'],
            opacity: 1
        },
        grid: {
            show: true,
            strokeDashArray: 3,
            borderColor: "rgba(0,0,0,0.1)"
        },
        xaxis: {
            labels: {
                show: false,
            },
            axisBorder: {
                show: false,
            }
        },
        yaxis: {
            labels: {
                show: false,
            },
            axisBorder: {
                show: false,
            }
        },
        legend: {
            show: false
        },
        tooltip: {
            theme: "dark",
            marker: {
                show: true,
            },
        },
    };

    var chart_line_overview = new ApexCharts(document.querySelector("#android-vs-ios"), options_Android_Vs_iOS);
    chart_line_overview.render();
    return "OK";
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

function InitDonut()
{
    $(".select2").select2();

    //#c43939 red
    //39c449 green
    Morris.Donut({
        element: 'morris-donut-chart',
        data: [{
            label: "Daily",
            value: 12,

        }, {
            label: "Monthly",
            value: 30
        }, {
            label: "Yearly",
            value: 20
        }],
        resize: true,
        colors:['#009efb', '#39c449', '#2f3d4a']
    });
    var myChart = echarts.init(document.getElementById('basic-line'));

    // specify chart configuration item and data
    var option = {
        // Setup grid
        grid: {
            left: '1%',
            right: '2%',
            bottom: '0%',
            containLabel: true
        },

        // Add Tooltip
        tooltip : {
            trigger: 'axis'
        },

        // Add Legend


        // Add custom colors
        color: ['#009efb', '#f62d51'],

        // Enable drag recalculate
        calculable : false,

        // Horizontal Axiz
        xAxis : [
            {
                type : 'category',
                boundaryGap : true,
                data : ['Mon','Tue','Wed','Thu','Fri','Sat','Sun']
            }
        ],

        // Vertical Axis
        yAxis : [
            {
                type : 'value',
                axisLabel : {
                    formatter: '{value} °C'
                }
            }
        ],

        // Add Series
        series : [
            {
                name:'Max temp',
                type:'line',
                data:[5, 15, 11, 15, 12, 13, 10],
                markPoint : {
                    data : [
                        {type : 'max', name: 'Max'},
                        {type : 'min', name: 'Min'}
                    ]
                },
                markLine : {
                    data : [
                        {type : 'average', name: 'Average'}
                    ]
                },
                lineStyle: {
                    normal: {
                        width: 2,
                        shadowColor: 'rgba(0,0,0,0.1)',
                        shadowBlur: 10,
                        shadowOffsetY: 10
                    }
                },
            },

        ]
    };
    // use configuration item and data specified to show chart
    myChart.setOption(option);

}

function LoadSocialMediaFeed(key, feedData)
{
    $("#"+key).html(feedData);
}
