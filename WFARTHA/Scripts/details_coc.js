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
                "className": 'BRTWR',
                "defaultContent": '',
                "orderable": false
            },
            {
                "className": 'FondoGarantia',
                "defaultContent": '',
                "orderable": false
            },
            {
                "name": 'PorAnt',
                "className": 'PorAnt',
                "orderable": false,
                "visible": true
            }//,
            //{
            //    "name": 'AntSol',
            //    "className": 'AntSol',
            //    "orderable": false,
            //    "visible": true
            //},
            //{
            //    "name": 'MontoAntT',
            //    "className": 'MontoAntT',
            //    "orderable": false,
            //    "visible": true
            //},
            //{
            //    "name": 'AntAmort',
            //    "className": 'AntAmort',
            //    "orderable": false,
            //    "visible": true
            //},
            //{
            //    "name": 'AntTr',
            //    "className": 'AntTr',
            //    "orderable": false,
            //    "visible": true
            //},
            //{
            //    "name": 'AmortAnt',
            //    "className": 'AmortAnt',
            //    "orderable": false,
            //    "visible": true
            //}
        ]
    });

    $('#tableOC2').DataTable({
        language: {
            "url": "../Scripts/lang/ES.json"
        },
        "paging": false,
        "info": false,
        "ordering": false,
        "searching": false,
        "columns": [
            {
                "className": 'POSC',
                "defaultContent": '',
                "orderable": false
            },
            {
                "className": 'POS',
                "defaultContent": '',
                "orderable": false
            },
            {
                "className": 'NDOC',
                "defaultContent": '',
                "orderable": false
            },
            {
                "className": 'EJERCICIO',
                "orderable": false,
                "visible": true
            },
            {
                "className": 'ANTAMOR',
                "orderable": false,
                "visible": true
            },
            {
                "name": 'TOANT',
                "className": 'TOANT',
                "orderable": false,
                "visible": true
            },
            {
                "className": 'MONEDA',
                "defaultContent": '',
                "orderable": false
            },
            {
                "name": 'AntTr',
                "className": 'AntTr',
                "orderable": false,
                "visible": true
            },
            {
                "className": 'AntXAMOR',
                "orderable": false,
                "visible": true
            }
        ]
    });
});

$(window).on('load', function () {
    var e = $('#norden_compra').val();
    $.ajax({
        type: "POST",
        url: 'getEKKOInfo',
        dataType: "json",
        data: { "ebeln": e },
        success: function (data) {
            var ekko = data.ekmo;
            var cuentas = data.res;
            var mtr = data.mtr;
            var brtwr = data.brtwr;
            llenarTablaOc(ekko, cuentas, mtr, brtwr);
        }
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