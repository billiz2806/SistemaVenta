let valorImpuesto=0;

$(document).ready(function () {

    fetch("/Venta/ListaTipoDocumentoVenta")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboTipoDocumentoVenta").append(
                        $("<option>").val(item.idTipoDocumentoVenta).text(item.descripcion)
                    )
                })
            }
        })

    fetch("/Negocio/Obtener")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto;
                console.log(d);
                $("#inputGroupSubTotal").text(`Sub Total - ${d.simboloMoneda}`)
                $("#inputGroupIGV").text(`IGV(${d.porcentajeImpuesto}%) - ${d.simboloMoneda}`)
                $("#inputGroupTotal").text(`Total - ${d.simboloMoneda}`)
                valorImpuesto = parseFloat(d.porcentajeImpuesto);
            }
        })

    $("#cboBuscarProducto").select2({
        ajax: {
            url: "/Venta/ObtenerProductos",
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            delay: 250,
            data: function (params) {
                return {
                    busqueda: params.term
                };
            },
            processResults: function (data) {
                return {
                    results: data.map((item) => (
                        {
                            id: item.idProducto,
                            text: item.descripcion,
                            marca: item.marca,
                            categoria: item.nombreCategoria,
                            urlImagen: item.urlImagen,
                            precio: item.precio
                        }
                        
                    ))
                };
            }
        },
        language:'es',
        placeholder: 'Buscar producto',
        minimumInputLength: 1,
        templateResult: formatoResultado,
    });

})

function formatoResultado(data) {
    if (data.loading) {
        return data.text;
    }

    var contenedor = $(
        `<table width="100%">
                <tr>
                    <td style="width:60px">
                        <img style="height:60px;width:60px; margin-right:20px" src="${data.urlImagen}" />
                    </td>
                    <td>
                        <p style="font-weight:bold; margin:2px">${data.marca}</p
                        <p style="margin:2px">${data.text}</p>
                    </td>
                </tr>
            </table>`
    );

    return contenedor;
}

$(document).on("select2:open", function () {
    document.querySelector(".select2-search__field").focus();
})

let productosParaVenta = [];

$("#cboBuscarProducto").on("select2:select", function (e) {
    const data = e.params.data;

    let producto_encontrado = productosParaVenta.filter(P => P.idProducto == data.id)
    if (producto_encontrado.length > 0) {
        $("#cboBuscarProducto").val("").trigger("change")
        toastr.warning("", "El producto ya fue agregado");
        return false;
    }

    swal({
        title: data.marca,
        text: data.text,
        imageUrl: data.urlImagen,
        showCancelButton: true,
        type:"input",
        //showConfirmButton: true,
        //confirmButtonClass: "btn-danger",
        //confirmButtonText: "Si, eliminar",
        //cancelButtonText: "No, cancelar",
        closeOnConf‌irm: false,
        inputPlaceholder:"Ingrese cantidad"
    //    closeOnCancel: true
    },
        function (valor) {
            if (valor === false) { return false }
            if (valor === "")
            {
                toastr.warning("", "Nesecita ingresar la cantidad");
                return false;
            }
            if (isNaN(parseInt(valor))) {
                toastr.warning("", "Debe ingresar un valor numérico");
                return false;
            }

            let producto = {
                idProducto: data.id,
                marcaProducto: data.marca,
                descripcionProducto: data.text,
                categoriaProducto: data.categoria,
                cantidad: parseInt(valor),
                precio: data.precio.toString(),
                total: (parseFloat(valor)*data.precio).toString()
            }

            productosParaVenta.push(producto);

            mostrarProducto_Precios();
            $("#cboBuscarProducto").val("").trigger("change");
            swal.close();
            console.log(producto);

        }
    );

})

function mostrarProducto_Precios() {
    let total = 0;
    let igv = 0;
    let subTotal = 0;
    let porcentaje = valorImpuesto / 100;

    $("#tbProducto tbody").html("")

    productosParaVenta.forEach((item) => {
        total = total + parseFloat(item.total);
        $("#tbProducto tbody").append(
            $("<tr>").append(
                $("<td>").append(
                    $("<button>").addClass("btn btn-danger btn-eliminar btn-sm").append(
                        $("<i>").addClass("fas fa-trash-alt")
                    ).data("idProducto",item.idProducto),
                ),
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total),

            )
        )
    })

    subTotal = total / (1 + porcentaje);
    igv = total - subTotal;

    $("#txtSubTotal").val(subTotal.toFixed(2));
    $("#txtIGV").val(igv.toFixed(2));
    $("#txtTotal").val(total.toFixed(2));


}

$(document).on("click","button.btn-eliminar", function () {
    const _idProducto = $(this).data("idProducto")
    productosParaVenta = productosParaVenta.filter(p => p.idProducto != _idProducto);
    mostrarProducto_Precios();
})

$("#btnTerminarVenta").click(function () {
    if (productosParaVenta < 1) {
        toastr.warning("", "Debe ingresar un producto");
        return;
    }

    const detalleVentaDto = productosParaVenta;

    const venta = {
        idTipoDocumentoVenta: $("#cboTipoDocumentoVenta").val(),
        documentoCliente: $("#txtDocumentoCliente").val(),
        nombreCliente: $("#txtNombreCliente").val(),
        subTotal: $("#txtSubTotal").val(),
        impuestoTotal: $("#txtIGV").val(),
        total: $("#txtTotal").val(),
        DetalleVenta: detalleVentaDto
    }

    $("#btnTerminarVenta").LoadingOverlay("show");

    fetch("/Venta/RegistrarVenta", {
        method: "POST",
        headers: { "Content-type": "application/json; charset=utf-8" },
        body: JSON.stringify(venta),
    })
        .then(response => {
            $("#btnTerminarVenta").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                productosParaVenta = [];
                mostrarProducto_Precios();
                $("#txtDocumentoCliente").val("");
                $("#txtNombreCliente").val("");
                $("#cboTipoDocumentoVenta").val($("#cboTipoDocumentoVenta option:first").val())
                $("#txtSubTotal").val("");
                $("#txtIGV").val("");
                $("#txtTotal").val("");

                swal("Registrado", `Numero de venta:${responseJson.objeto.numeroVenta}  `, "success")
            } else {
                swal("Error", responseJson.mensaje, "error")

            }
            
        })
})