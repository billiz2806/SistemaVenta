$(document).ready(function () {

    $("div.container-fluid").LoadingOverlay("show");


    fetch("/DashBoard/ObtenerResumen")
        .then(response => {
            $("div.container-fluid").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const obj = responseJson.objeto

                //Mostrar datos para las tarjetas
                $("#totalVenta").text(obj.totalVentas);
                $("#totalIngresos").text("S/. " + obj.totalIngresos);
                $("#totalProductos").text(obj.totalProductos);
                $("#totalCategorias").text(obj.totalCategorias);

                 //Obtener textos y valores para los grafocs de barras
                let barchart_labels;
                let barchart_data;
                console.log(obj);

                if (obj.ventasUltimaSemana.length > 0) {
                    barchart_labels = obj.ventasUltimaSemana.map((item) => { return item.fecha })
                    barchart_data = obj.ventasUltimaSemana.map((item) => { return item.total })
                }
                else {
                    barchart_labels = ["Sin datos"];
                    barchart_data = [0];
                }

                //Obtener textos y valores para los grafocs de Pie
                let piechart_labels;
                let piechart_data;
                if (obj.productosTopUltimaSemana.length > 0) {
                    piechart_labels = obj.productosTopUltimaSemana.map((item) => { return item.producto })
                    piechart_data = obj.productosTopUltimaSemana.map((item) => { return item.cantidad })
                }
                else {
                    piechart_labels = ["Sin datos"];
                    piechart_data = [0];
                }

                // Bar Chart Example
                let controlVenta = document.getElementById("chartVentas");
                let myBarChart = new Chart(controlVenta, {
                    type: 'bar',
                    data: {
                        labels: barchart_labels,
                        datasets: [{
                            label: "Cantidad",
                            backgroundColor: "#FFBA1C", 
                            hoverBackgroundColor: "#FFD500",
                            borderColor: "#FFE76B",
                            data: barchart_data,
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        legend: {
                            display: false
                        },
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                maxBarThickness: 50,
                            }],
                            yAxes: [{
                                ticks: {
                                    min: 0,
                                    maxTicksLimit: 5
                                }
                            }],
                        },
                    }
                });

                // Pie Chart Example
                let controlProducto = document.getElementById("chartProductos");
                let myPieChart = new Chart(controlProducto, {
                    type: 'doughnut',
                    data: {
                        labels: piechart_labels,
                        datasets: [{
                            data: piechart_data,
                            backgroundColor: ['#FFBA1C', '##FF5733', '##C70039', "#FFDF94"],
                            hoverBackgroundColor: ['#FFBA1C', '##FF5733', '##C70039', "#D8B86E"],
                            hoverBorderColor: "rgba(234, 236, 244, 1)",
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        tooltips: {
                            backgroundColor: "rgb(255,255,255)",
                            bodyFontColor: "#858796",
                            borderColor: '#dddfeb',
                            borderWidth: 1,
                            xPadding: 15,
                            yPadding: 15,
                            displayColors: false,
                            caretPadding: 10,
                        },
                        legend: {
                            display: true
                        },
                        cutoutPercentage: 80,
                    },
                });

            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        })
});