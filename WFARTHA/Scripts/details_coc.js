$(document).ready(function () {
    $('#tableOC').DataTable({
        language: {
            "url": "../Scripts/lang/ES.json"
        },
        "paging": false,
        "info": false,
        "ordering": false,
        "searching": false,
        "columns": [
            {
                "className": 'FondoGarantia',
                "defaultContent": '',
                "orderable": false
            },
            {
                "name": 'AmortAnt',
                "className": 'AmortAnt',
                "orderable": false,
                "visible": true
            },
            {
                "name": 'MontoAntT',
                "className": 'MontoAntT',
                "orderable": false,
                "visible": true
            },
            {
                "name": 'AntAmort',
                "className": 'AntAmort',
                "orderable": false,
                "visible": true
            },
            {
                "name": 'PorAnt',
                "className": 'PorAnt',
                "orderable": false,
                "visible": true
            },
            {
                "name": 'AntSol',
                "className": 'AntSol',
                "orderable": false,
                "visible": true
            },
            {
                "name": 'AntTr',
                "className": 'AntTr',
                "orderable": false,
                "visible": true
            }
        ]
    });

});


function llenarCOC(tsol) {
    
    //Recupero el valor de la sociedad
    var buk = $("#SOCIEDAD_ID").val();
    if (tsol === "SCO") {
        if ($("#PAYER_ID").val() != "") {
            llenaOrdenes($("#D_PAYER_ID").val(), buk);
        }
    }
}

function llenaOrdenes(lifnr, bukrs) {
    var tr = $(this).closest('tr'); //Obtener el row
    var t = $('#table_infoP').DataTable();

    var pedidosNum = [];
    $("#norden_compra").empty();

    $.ajax({
        type: "POST",
        url: '../getPedidos',
        dataType: "json",
        data: { "lifnr": lifnr.trim(), "bukrs": bukrs },
        success: function (data) {
            $("#norden_compra").empty();
            for (var i = 0; i < data.length; i++) {
                var ebeln = data[i];
                $("#norden_compra").append($("<option>").attr('value', ebeln).text(ebeln));
            }

            var elem = document.querySelectorAll("select");
            var instance = M.Select.init(elem, []);
        }
    });
}