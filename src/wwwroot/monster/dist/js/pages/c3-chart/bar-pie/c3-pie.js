/*************************************************************************************/
// -->Template Name: Bootstrap Press Admin
// -->Author: Themedesigner
// -->Email: niravjoshi87@gmail.com
// -->File: c3_chart_JS
/*************************************************************************************/
$(function() {
    var o = c3.generate({
        bindto: "#pie-chart",
        color: { pattern: ["#7460ee", "#7460ee", "#7460ee", "#7460ee", "#7460ee", "#7460ee", "#7460ee", "#7460ee"] },
        data: {
            columns: [
                ["option1", 600],
                ["option2", 600]
            ],
            type: "pie",
            onclick: function(o, n) { console.log("onclick", o, n) },
            onmouseover: function(o, n) { console.log("onmouseover", o, n) },
            onmouseout: function(o, n) { console.log("onmouseout", o, n) }
        }
    });
    setTimeout(function() {
        o.load({
            columns: [
                ["Public sale", 40],
                ["Private sale", 10],
                ["Marketing & Development", 10],
                ["DEX & CEX", 10],
                ["Seeds", 10],
                ["Reserve", 10],
                ["Team", 7],
                ["Advisors", 3]
                
            ]
        });
 
    }, 1500), setTimeout(function () {
        o.unload({ ids: "option1" }), o.unload({ ids: "option2" });
        o.resize({ height: 900   });
    }, 2500)

});