/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;-

namespace emisionfacturacolombia
{
    public class Class1
    {
    }
}

*/


using cfdiEntidadesGP;
//using cfdiPeruInterfaces;
using cfdiColombiaInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
//using cfdiPeruDFactureUbl21.DFactureUbl21;
using System.Xml.Linq;
using emisionfacturacolombia.Colombia;
//using System.Xml;

//namespace cfdiPeruOperadorServiciosElectronicos
namespace cfdiColombiaOperadorServiciosElectronicos
{
    public class WebServicesOSE : ICfdiMetodosWebService
    {
        //private DocumentoElectronico DocEnviarWS;
        //private FacturaGeneral;
        private FacturaGeneral DocEnviarWS = new FacturaGeneral();
        //instancia un objeto que tiene la direccion del servicio web
        //ServiceEmision.ServiceClient serviceClient;
        private ServiceClient ServicioWS = new ServiceClient();
        string debug_xml;
        string MensajeError;
        //double pti = 0, vti = 0;

        public WebServicesOSE(string URLwebServPAC)
        {
            //crea un direccion unica de red donde un cliente puede comunicarse con un servicio endpoint
            ServicioWS.Endpoint.Address = new System.ServiceModel.EndpointAddress(URLwebServPAC);
        }

        private FacturaGeneral ArmaDocumentoEnviarWS(DocumentoVentaGP documentoGP)
        {
            //DocEnviarWS es el objeto del servicio web que se arma a traves del  objeto documentoGP
            DocEnviarWS = new FacturaGeneral();
            int i = 0; // Variable para loopear
            double tsi = 0;
            //double tot = 0;
            //double pti = 0, vti = 0;
            //double tde = 0, tsi = 0;
            int correlativo = 1; // Variable para corre de productos;
            //Boolean DescItemIGV = true;
            //string cadena = "";
            //string[] nrd;
            debug_xml = "";

            //FACTURA GENERAL
            debug_xml = "<FACTURA GENERAL> \r\n";
            //SECCION FACTURA COMIENZO
            DocEnviarWS.cantidadDecimales = documentoGP.DocVenta.cantidadDecimales.ToString();
            DocEnviarWS.consecutivoDocumento = documentoGP.DocVenta.consecutivoDocumento;            
            //CLASE CARGOS DESCUENTOS.
            DocEnviarWS.cargosDescuentos = new CargosDescuentos[1];
            CargosDescuentos cargosdescuentos1 = new CargosDescuentos();
            cargosdescuentos1.codigo = documentoGP.DocVenta.cargosdescuentos_codigo;
            cargosdescuentos1.descripcion = documentoGP.DocVenta.cargosdescuentos_descripcion.PadLeft(10, '-');
            cargosdescuentos1.indicador = documentoGP.DocVenta.cargosdescuentos_indicador.ToString();
            cargosdescuentos1.monto= documentoGP.DocVenta.cargosdescuentos_monto.ToString();
            cargosdescuentos1.montoBase = documentoGP.DocVenta.cargosdescuentos_montobase.ToString();
            cargosdescuentos1.porcentaje = documentoGP.DocVenta.cargodescuentos_porcentaje.ToString();
            cargosdescuentos1.secuencia = documentoGP.DocVenta.cargosdescuentos_secuencia;
            DocEnviarWS.cargosDescuentos[0] = cargosdescuentos1;            
            debug_xml = "<CARGOS DESCUENTOS> \r\n";
            debug_xml = debug_xml + DocEnviarWS.cargosDescuentos[0] + "\r\n";
            debug_xml = debug_xml + "<FIN CARGOS DESCUENTOS> \r\n";
            //FIN CARGOS DESCUENTOS            
            //FIN COMIENZO DE FACTURA GENERAL
            //SECCION CLIENTE
            DocEnviarWS.cliente = new Cliente();
            //DocEnviarWS.cliente.apellido = documentoGP.DocVenta.receptorNombre;            
            DocEnviarWS.cliente.nombreRazonSocial = documentoGP.DocVenta.cliente_nombreRazonSocial;
            //DocEnviarWS.cliente.departamento = "no aplica  no hay en la base de datos";
            //DocEnviarWS.cliente.direccion = documentoGP.DocVenta.cliente_difdireccion;
            //DocEnviarWS.cliente.pais = documentoGP.DocVenta.receptorPais;
            //DocEnviarWS.cliente.email = documentoGP.DocVenta.cliente_email;
            DocEnviarWS.cliente.numeroDocumento = documentoGP.DocVenta.cliente_numeroDocumento;
            //DocEnviarWS.cliente.numeroDocumento = "23456783456734567832;
            DocEnviarWS.cliente.tipoIdentificacion = documentoGP.DocVenta.cliente_tipoIdentificacion;
            DocEnviarWS.cliente.tipoPersona = documentoGP.DocVenta.cliente_tipoPersona;
            DocEnviarWS.cliente.notificar = documentoGP.DocVenta.cliente_notificar;
            DocEnviarWS.cliente.telefono = documentoGP.DocVenta.cliente_telefono;
            DocEnviarWS.cliente.email = documentoGP.DocVenta.cliente_email;
            //OBTENER DATOS DEL CLIENTE DE LA VISTA DESTINATARIOS
            DocEnviarWS.cliente.destinatario = new Destinatario[1];
            Destinatario destinatario1 = new Destinatario();
            destinatario1.email = new String[1];
            //string[] em = new String[1];
            //em[0]= documentoGP._clides.cliente_email;
            destinatario1.email[0] = documentoGP._clides.cliente_email;//"email@gmail.com"; //em[0];
            destinatario1.canalDeEntrega = documentoGP._clides.cliente_canalEntrega;            
            DocEnviarWS.cliente.destinatario[0] = destinatario1;
            //FIN DE DATOS DE CLIENTES VISTA DESTINATARIOS

            //DocEnviarWS.cliente.regimen = documentoGP.DocVenta.cliente_regimen;            
            //DocEnviarWS.cliente.tipoIdentificacion = "31";
            //DocEnviarWS.cliente.tipoPersona = "1";
            //DocEnviarWS.cliente.notificar = "NO";
            //DocEnviarWS.cliente.regimen = "2";
            //DocEnviarWS.cliente.nombreRazonSocial = documentoGP.DocVenta.emisorRazonSocial;
            //DocEnviarWS.cliente.referencia2 = documentoGP.DocVenta.emisorReferencia2;
            //DocEnviarWS.cliente.subDivision = documentoGP.DocVenta.emisorSubdivision;
            //DocEnviarWS.cliente.segundoNombre = documentoGP.DocVenta.emisorSegundoNombre;
            //SECCION DETALLES TRIBUTOS
            DocEnviarWS.cliente.detallesTributarios = new Tributos[1];
            Tributos tributos1 = new Tributos();
            tributos1.codigoImpuesto = documentoGP._facimpcab.codigoTotalImp; //"01";
            //tributos1.extras = null;
            DocEnviarWS.cliente.detallesTributarios[0] = tributos1;
            //FIN DETALLES TRIBUTOS

            // SECCION DIRECCION FISCA,
            Direccion direccionFiscal = new Direccion();
            //direccionFiscal.aCuidadoDe = null;
            //direccionFiscal.aLaAtencionDe = null;
            //direccionFiscal.bloque = null;
            //direccionFiscal.buzon = null;
            //direccionFiscal.calle = null;
            //direccionFiscal.calleAdicional = null;
            //direccionFiscal.ciudad = "MANIZALES";
            direccionFiscal.codigoDepartamento = documentoGP.DocVenta.cliente_difcodigoDepartamento;//"11";
            //direccionFiscal.correccionHusoHorario = null;
            //direccionFiscal.departamento = "Bogotá";
            //direccionFiscal.departamentoOrg = null;
            direccionFiscal.direccion = documentoGP.DocVenta.cliente_difdireccion;//"Direccion";
            //direccionFiscal.distrito = null;
            //direccionFiscal.habitacion = null;
            direccionFiscal.lenguaje = documentoGP.DocVenta.cliente_diflenguaje;//"es";
            //direccionFiscal.localizacion = null;
            direccionFiscal.municipio = documentoGP.DocVenta.cliente_difmunicipio;// "11001";
            //direccionFiscal.nombreEdificio = null;
            //direccionFiscal.numeroEdificio = null;
            //direccionFiscal.numeroParcela = null;
            direccionFiscal.pais = documentoGP.DocVenta.cliente_difpais;// "CO";
            //direccionFiscal.piso = null;
            //direccionFiscal.region = null;
            //direccionFiscal.subDivision = null;
            //direccionFiscal.ubicacion = null;
            direccionFiscal.zonaPostal = documentoGP.DocVenta.cliente_difzonapostal;//"110211";
            DocEnviarWS.cliente.direccionFiscal = direccionFiscal;
            //FIN DIRECCION FISCAL

            //INFORMACION LEGAL DEL CLIENTE
            DocEnviarWS.cliente.informacionLegalCliente = new InformacionLegal();
            InformacionLegal informacionlegal1 = new InformacionLegal();
            informacionlegal1.nombreRegistroRUT = documentoGP.DocVenta.cliente_nombreRegistroRUT;
            informacionlegal1.numeroIdentificacion = documentoGP.DocVenta.cliente_numeroIdentificacion;
            informacionlegal1.numeroIdentificacionDV = documentoGP.DocVenta.cliente_numeroIdentificacionDV;
            informacionlegal1.tipoIdentificacion = documentoGP.DocVenta.cliente_tipoIdentificacion;
            DocEnviarWS.cliente.informacionLegalCliente = informacionlegal1;
            //FIN INFORMACION LEGAL DEL CLIENTE

            //OBLIGACIONES VIENE DE LA VISTA vwCfdiClienteObligacioS 
            DocEnviarWS.cliente.responsabilidadesRut = new Obligaciones[1];
            Obligaciones obligaciones1 = new Obligaciones();
            obligaciones1.obligaciones = documentoGP._cliobl.cliente_obligaciones;
            obligaciones1.regimen = documentoGP._cliobl.cliente_regimen;
            DocEnviarWS.cliente.responsabilidadesRut[0] = obligaciones1;
            //FIN OBLIGACIONES DE LA VISTA vwCfdiClienteObligaciones            
            // FORMO EL DEBUG_XML DE CLIENTE      
            debug_xml = debug_xml + "<CLIENTE> \r\n";
            debug_xml = debug_xml  + DocEnviarWS.cliente + "\r\n";
            debug_xml = debug_xml + " <FIN CLIENTE> \r\n";
            //FIN DEL DEBUG_XML DE CLIENTE
            //FIN SECCION CLIENTE
            
            // SECCION RECEPTOR
            //DocEnviarWS.receptor = new Receptor();
            //DocEnviarWS.receptor.email = documentoGP.DocVenta.emailTo;
            //DocEnviarWS.receptor.notificar = documentoGP.DocVenta.emailTo.Trim() == string.Empty ? "NO" : "SI";
            //DocEnviarWS.receptor.numDocumento = documentoGP.DocVenta.receptorNroDoc;
            //DocEnviarWS.receptor.direccion = documentoGP.DocVenta.receptorDireccion;
            //DocEnviarWS.receptor.departamento = documentoGP.DocVenta.receptorCiudad;
            //DocEnviarWS.receptor.distrito = documentoGP.DocVenta.recep
            //DocEnviarWS.receptor.pais = documentoGP.DocVenta.receptorPais;
            //DocEnviarWS.receptor.provincia = documentoGP.DocVenta.receptorProvincia;
            //DocEnviarWS.receptor.razonSocial = documentoGP.DocVenta.receptorNombre;
            //DocEnviarWS.receptor.telefono = documentoGP.DocVenta.
            //DocEnviarWS.receptor.tipoDocumento = documentoGP.DocVenta.receptorTipoDoc;
            //DocEnviarWS.receptor.ubigeo = documentoGP.DocVenta.rece

            //debug_xml = debug_xml + "<RECEPTOR>" + DocEnviarWS.receptor.tipoDocumento + ":" + DocEnviarWS.receptor.numDocumento + "\r\n";
            //debug_xml = debug_xml + "   <RazonSocial>" + DocEnviarWS.receptor.razonSocial + "\r\n";

            // SECCION COMROBANTE
            /*
            if (!string.IsNullOrEmpty(documentoGP.DocVenta.tipoOperacion))
            {
                DocEnviarWS.codigoTipoOperacion = documentoGP.DocVenta.tipoOperacion;
            }
            else
            {
                DocEnviarWS.codigoTipoOperacion = "0101";
            }
            DocEnviarWS.correlativo = documentoGP.DocVenta.numero;
            //DocEnviarWS.correlativo = "10000106"; // se usa para reenviar comprobante.
            DocEnviarWS.fechaEmision = documentoGP.DocVenta.fechaEmision.ToString("yyyy-MM-dd");
            DocEnviarWS.fechaVencimiento = documentoGP.DocVenta.fechaVencimiento.ToString("yyyy-MM-dd");
            DocEnviarWS.horaEmision = documentoGP.DocVenta.horaEmision;
            DocEnviarWS.idTransaccion = documentoGP.DocVenta.idDocumento;
            DocEnviarWS.serie = documentoGP.DocVenta.serie;
            DocEnviarWS.tipoDocumento = documentoGP.DocVenta.tipoDocumento;
            {
                debug_xml = debug_xml + "<COMPROBANTE>" + DocEnviarWS.serie + "-" + DocEnviarWS.correlativo + "\r\n";
                debug_xml = debug_xml + "   <tipoDocumento>" + DocEnviarWS.tipoDocumento + "\r\n";
                debug_xml = debug_xml + "   <codigoTipoOperacion>" + DocEnviarWS.codigoTipoOperacion + "\r\n";
                debug_xml = debug_xml + "   <FechaEmisio>" + DocEnviarWS.fechaEmision + "\r\n";
                debug_xml = debug_xml + "   <fechaVencimiento>" + DocEnviarWS.fechaVencimiento + "\r\n";
                debug_xml = debug_xml + "   <horaEmision>" + DocEnviarWS.horaEmision + "\r\n";
                debug_xml = debug_xml + "   <idTransaccion>" + DocEnviarWS.idTransaccion + "\r\n";
                debug_xml = debug_xml + "<FIN COMPROBANTE>" + "\r\n";
            }
            // FIN SECCION COMPROBANTE

            // SECCION Relacionado. VER mas adelante
            debug_xml = debug_xml + "<RELACIONADO NOTAS>" + documentoGP.LDocVentaRelacionados.Count() + "\r\n";
            if (string.IsNullOrEmpty(documentoGP.DocVenta.cRelacionadoTipoDocAfectado))
            {
                debug_xml = debug_xml + "<SIN DOC RELACIONADO>" + "\r\n";
            }
            else
            {
                if (DocEnviarWS.tipoDocumento == "07" || DocEnviarWS.tipoDocumento == "08")
                {
                    var relacionadoN = new RelacionadoNotas();

                    relacionadoN.codigoTipoNota = documentoGP.DocVenta.infoRelNotasCodigoTipoNota;
                    relacionadoN.observaciones = documentoGP.DocVenta.infoRelNotasObservaciones;
                    relacionadoN.numeroDocAfectado = documentoGP.DocVenta.cRelacionadoNumDocAfectado.Trim();
                    relacionadoN.tipoDocAfectado = documentoGP.DocVenta.cRelacionadoTipoDocAfectado;

                    DocEnviarWS.relacionadoNotas = new RelacionadoNotas();
                    DocEnviarWS.relacionadoNotas = relacionadoN;
                    debug_xml = debug_xml + "  <TIPOAFECTADO>" + DocEnviarWS.relacionadoNotas.tipoDocAfectado + "\r\n";
                    debug_xml = debug_xml + "  <DOCAFECTADO>" + DocEnviarWS.relacionadoNotas.numeroDocAfectado + "\r\n";
                    debug_xml = debug_xml + "  <NOTA>" + DocEnviarWS.relacionadoNotas.codigoTipoNota + "\r\n";
                    debug_xml = debug_xml + "  <observaciones>" + DocEnviarWS.relacionadoNotas.observaciones + "\r\n";
                    debug_xml = debug_xml + "<FIN RELACIONADO NOTAS>\r\n";
                }
                else
                {
                    if (DocEnviarWS.tipoDocumento == "01")
                    {
                        var relacionado = new Relacionado();
                        relacionado.numeroDocRelacionado = documentoGP.DocVenta.cRelacionadoNumDocAfectado.Trim();
                        relacionado.tipoDocRelacionado = documentoGP.DocVenta.cRelacionadoTipoDocAfectado;

                        DocEnviarWS.relacionado = new Relacionado[1];
                        DocEnviarWS.relacionado[0] = relacionado;

                        debug_xml = debug_xml + "  <TIPOAFECTADO>" + DocEnviarWS.relacionado[0].numeroDocRelacionado + "\r\n";
                        debug_xml = debug_xml + "  <DOCAFECTADO>" + DocEnviarWS.relacionado[0].tipoDocRelacionado + "\r\n";
                        debug_xml = debug_xml + "<FIN RELACIONADO NOTAS>\r\n";
                    }
                }

            }


            // debug_xml = debug_xml + "<RELACIONADO NOTAS>" + documentoGP.LDocVentaRelacionados.Count() + "\r\n";
            foreach (vwCfdiRelacionados relacionado_gp in documentoGP.LDocVentaRelacionados)
            {
                if (DocEnviarWS.tipoDocumento == "07" || DocEnviarWS.tipoDocumento == "08")
                {
                    var relacionadoN = new RelacionadoNotas();

                    relacionadoN.codigoTipoNota = documentoGP.DocVenta.infoRelNotasCodigoTipoNota;
                    relacionadoN.observaciones = documentoGP.DocVenta.infoRelNotasObservaciones;
                    relacionadoN.numeroDocAfectado = relacionado_gp.sopnumbeTo.Trim();
                    relacionadoN.tipoDocAfectado = relacionado_gp.tipoDocumento;

                    DocEnviarWS.relacionadoNotas = new RelacionadoNotas();
                    DocEnviarWS.relacionadoNotas = relacionadoN;

                    debug_xml = debug_xml + "  <TIPOAFECTADO>" + DocEnviarWS.relacionadoNotas.tipoDocAfectado + "\r\n";
                    debug_xml = debug_xml + "  <DOCAFECTADO>" + DocEnviarWS.relacionadoNotas.numeroDocAfectado + "\r\n";
                    debug_xml = debug_xml + "  <NOTA>" + DocEnviarWS.relacionadoNotas.codigoTipoNota + "\r\n";
                    debug_xml = debug_xml + "  <observaciones>" + DocEnviarWS.relacionadoNotas.observaciones + "\r\n";
                    debug_xml = debug_xml + "<FIN RELACIONADO NOTAS>\r\n";
                }
                else
                {
                    if (DocEnviarWS.tipoDocumento == "01")
                    {
                        var relacionado = new Relacionado();
                        relacionado.numeroDocRelacionado = relacionado_gp.sopnumbeTo.Trim(); ;
                        relacionado.tipoDocRelacionado = relacionado_gp.tipoDocumento;

                        DocEnviarWS.relacionado = new Relacionado[1];
                        DocEnviarWS.relacionado[0] = relacionado;

                        debug_xml = debug_xml + "  <TIPOAFECTADO>" + DocEnviarWS.relacionado[0].numeroDocRelacionado + "\r\n";
                        debug_xml = debug_xml + "  <DOCAFECTADO>" + DocEnviarWS.relacionado[0].tipoDocRelacionado + "\r\n";
                        debug_xml = debug_xml + "<FIN RELACIONADO NOTAS>\r\n";
                    }

                }

                //Aumenta contadoresDocEnviarWS.producto[i].
                i++;
                correlativo++;
            }

            */

            //SECCION CONCEPTOS O DETALLES DE LA FACTURA
            DocEnviarWS.detalleDeFactura = new FacturaDetalle[documentoGP.LDocVentaConceptos.Count()];
            debug_xml = debug_xml + "FACTURA DETALLE \r\n";
            debug_xml = debug_xml + "<CANT PROD>" + DocEnviarWS.detalleDeFactura.Count() + "\r\n";
            i = 0; correlativo = 1;
            foreach(vwCfdiConceptos detalleDeFactura_gp in documentoGP.LDocVentaConceptos)
            {
                //detalles basicos de la factura
                var detalle1 = new FacturaDetalle();
                //detalle1.codigoProducto = detalleDeFactura_gp.itemnmbr.ToString();
                detalle1.codigoProducto = detalleDeFactura_gp.facturadetalle_codigoproducto;
                detalle1.descripcion = detalleDeFactura_gp.facturadetalle_descripcion;
                detalle1.estandarCodigo = detalleDeFactura_gp.facturadetalle_estandarcodigo;
                //detalle1.estandarCodigoID = detalleDeFactura_gp.facturadetalle_estandarcodigoproducto;
                detalle1.estandarCodigoProducto = detalleDeFactura_gp.facturadetalle_estandarcodigoproducto;
                detalle1.unidadMedida = detalleDeFactura_gp.facturadetalle_unidadMedida;
                detalle1.cantidadReal = detalleDeFactura_gp.facturadetalle_cantidadreal.ToString();
                // cantidad real unidad Medidia debe tener coma maximo 6 caracteres  de longitud y si o si debe tener esa cantidad.
                detalle1.cantidadRealUnidadMedida = detalleDeFactura_gp.facturadetalle_cantidadrealunidadmedida;
                //detalle1.cantidadUnidades = Math.Round((double)detalleDeFactura_gp.facturadetalle_cantidadunidades,2).ToString("000000");
                detalle1.cantidadUnidades = detalleDeFactura_gp.facturadetalle_cantidadunidades.ToString();
                detalle1.secuencia = detalleDeFactura_gp.facturadetalle_secuencia;
                detalle1.cantidadPorEmpaque = detalleDeFactura_gp.facturadetalle_cantidadporempaque.ToString();
                //detalle1.descuento = Math.Round((double)detalleDeFactura_gp.cargosdescuentos_monto,2).ToString("0000000000000.00");
                detalle1.precioVentaUnitario = Math.Round((double)detalleDeFactura_gp.facturadetalle_precioVentaUnitario, 2).ToString();// "00000000000000.00");
                //detalle1.precioTotalSinImpuestos = (Math.Round((double)detalleDeFactura_gp.importe, 2) - Math.Round((double)detalleDeFactura_gp.descuento, 2)).ToString("00000000000000.00");
                detalle1.precioTotalSinImpuestos = detalleDeFactura_gp.facturadetalle_precioTotalSinImpuestos.ToString();//("00000000000000.00");
                detalle1.precioTotal = detalleDeFactura_gp.facturadetalle_precioTotal.ToString();
                debug_xml = debug_xml + "Código Produco \r\n" + detalle1.codigoProducto;
                debug_xml = debug_xml + "Descripción Producto \r\n" + detalle1.descripcion;
                debug_xml = debug_xml + "Unidad de Medida \r\n" + detalle1.unidadMedida;
                debug_xml = debug_xml + "Cantidad de Unidades \r\n" + detalle1.cantidadUnidades;
                debug_xml = debug_xml + "Precio Venta Unitario \r\n" + detalle1.precioVentaUnitario;
                debug_xml = debug_xml + "Precio Total Sin Impuestos \r\n" + detalle1.precioTotalSinImpuestos;
                debug_xml = debug_xml + "Precio Total \r\n" + detalle1.precioTotal;
                //FIN DETALLES BASICOS

                //IMPUESTOS DETALLES DE FACTURA DETALLES              
                detalle1.impuestosDetalles = new FacturaImpuestos[1];
                FacturaImpuestos impuestodetalles1 = new FacturaImpuestos();
                impuestodetalles1.baseImponibleTOTALImp = Math.Round(documentoGP._facimpdet.baseimponibletotalimp).ToString();//("00000000000000.00");
                //impuestodetalles1.codigoTOTALImp = "01";
                impuestodetalles1.codigoTOTALImp = documentoGP._facimpdet.codigoTotalImp;
                //impuestodetalles1.codigoTOTALImp = detalleDeFactura_gp.tipoTributo.ToString();
                //impuestodetalles1.controlInterno = "056";
                impuestodetalles1.controlInterno = documentoGP._facimpdet.controlInterno;
                impuestodetalles1.porcentajeTOTALImp = Convert.ToDecimal(documentoGP._facimpdet.porcentajeTotalImp).ToString(); //("00.00");
                impuestodetalles1.valorTOTALImp = Math.Round((double)documentoGP._facimpdet.valorTotalImp, 2).ToString();// ("00000000000000.00");
                impuestodetalles1.unidadMedida = documentoGP._facimpdet.unidadMedida.ToString();
                impuestodetalles1.unidadMedidaTributo = Convert.ToDecimal(documentoGP._facimpdet.unidadMedidaTributo).ToString();// ("00.00");
                impuestodetalles1.valorTributoUnidad = documentoGP._facimpdet.valorTributoUnidad.ToString();                
                detalle1.impuestosDetalles[0] = impuestodetalles1;
                debug_xml = debug_xml + "<Impuestos Detalles de Factura Detalles>\n\r";
                debug_xml = debug_xml + "<impuestosdetalle>" + detalle1.impuestosDetalles[0] + "\r\n";
                debug_xml = debug_xml + "<FIN de Impuestos Detalles de Factura Detalles>\n\r";
                //FIN IMPUESTOS DETALLES DE FACTURA DETALLES
                
                //calculo del precio total
                //tde = tde + Math.Round((double)detalleDeFactura_gp.cargosdescuentos_monto, 2);
                //tsi = tsi + (Math.Round((double)detalleDeFactura_gp.facturadetalle_precioTotal, 2));
                //detalle1.precioTotal = (Math.Round((double)detalleDeFactura_gp.facturadetalle_precioTotal, 2)- Math.Round((double)detalleDeFactura_gp.cargosdescuentos_monto, 2)).ToString("00000000000000.00");
                //tot = tot + float.Parse(detalle1.precioTotal);                
                //detalle1.precioVentaUnitario = "2000";
                //detalle1.seriales = detalleDeFactura_gp.SERLTNUM;                
                //{
                    // = debug_xml + "<PRODUCTO>" + correlativo + "\r\n";
                    //debug_xml = debug_xml + "<cantidad>" + detalle1.cantidadUnidades + "\r\n";
                    //debug_xml = debug_xml + "   <codigoPLU>" + detalle1.codigoProducto + "\r\n";
                    //debug_xml = debug_xml + "   <descripcion>" + detalle1.descripcion + "\r\n";
                    //debug_xml = debug_xml + "<descuento>" + detalle1.descuento + "\r\n" ;
                    //debug_xml = debug_xml + "<impuestosdetalles>" + detalle1.impuestosDetalles[0] + "\r\n ";
                    // = debug_xml + "<preciototal>" + detalle1.precioTotal + "\r\n";
                    //debug_xml = debug_xml + "<preciototalsinimpuestos>" + detalle1.precioTotalSinImpuestos + "\r\n";
                    //debug_xml = debug_xml + "<precioventaunitario>" + detalle1.precioVentaUnitario + "\r\n";
                    //debug_xml = debug_xml + "<seriales>" + detalle1.seriales + "\r\n";
                    //debug_xml = debug_xml + "<unidadmedida>" + detalle1.unidadMedida + "\r\n";
                //}                

                //IMPUESTOS TOTALES DE FACTURA DETALLES
                detalle1.impuestosTotales = new ImpuestosTotales[1];                
                ImpuestosTotales impuestosTotales1 = new ImpuestosTotales();
                impuestosTotales1.codigoTOTALImp = documentoGP._facimpcab.codigoTotalImp;
                impuestosTotales1.montoTotal = documentoGP._facimpcab.valorTotalImp.ToString();// ("000000000000000.000000");
                detalle1.impuestosTotales[0] = impuestosTotales1;
                //preguntar si esta bien lo de abajo
                impuestosTotales1.montoTotal = impuestosTotales1.montoTotal + Math.Round((double)documentoGP._facimpdet.valorTotalImp, 2);//preguntar si esta bien esto
                debug_xml = debug_xml + "<Impuestos Totales de Facturas Detalles>\n\r";
                debug_xml = debug_xml + "<impuestos totales>" + detalle1.impuestosTotales[0] + "\r\n";
                debug_xml = debug_xml + "<Fin de Impuestos totales de Factura Detalles>\n\r";
                //FIN IMPUESTOS TOTALES DE FACTURA DETALLE

                // SECCION Producto.
                //DocEnviarWS.producto = new Producto[documentoGP.LDocVentaConceptos.Count()];
                //debug_xml = debug_xml + "<CANT PROD>" + DocEnviarWS.producto.Count() + "\r\n";
                //i = 0; correlativo = 1;
                //foreach (vwCfdiConceptos producto_gp in documentoGP.LDocVentaConceptos)
                //{
                //var producto = new Producto();

                //producto.cantidad = producto_gp.cantidad.ToString();
                //producto.codigoPLU = producto_gp.ITEMNMBR;
                //producto.codigoPLUSunat = producto_gp.claveProdSunat.Trim();
                //producto.descripcion = producto_gp.ITEMDESC;
                //producto.montoTotalImpuestoItem = producto_gp.montoIva.ToString("0.00");
                //producto.precioVentaUnitarioItem = producto_gp.precioUniConIva.ToString();
                //producto.unidadMedida = producto_gp.udemSunat;
                //producto.valorReferencialUnitario = producto_gp.precioUniConIva.ToString();
                //producto.valorUnitarioBI = producto_gp.valorUni.ToString();
                //producto.valorVentaItemQxBI = string.Format("{0,14:0.00}", producto_gp.importe).Trim();
                //producto.numeroOrden = correlativo.ToString();
                //{
                //debug_xml = debug_xml + "<PRODUCTO>" + correlativo + "\r\n";
                //debug_xml = debug_xml + "   <cantidad>" + producto.cantidad + "\r\n";
                //debug_xml = debug_xml + "   <codigoPLU>" + producto.codigoPLU + "\r\n";
                //debug_xml = debug_xml + "   <codigoPLUSunat>" + producto.codigoPLUSunat + "\r\n";
                //debug_xml = debug_xml + "   <descripcion>" + producto.descripcion + "\r\n";
                //debug_xml = debug_xml + "   <montoTotalImpuestoItem>" + producto.montoTotalImpuestoItem + "\r\n";
                //debug_xml = debug_xml + "   <precioVentaUnitarioItem>" + producto.precioVentaUnitarioItem + "\r\n";
                //debug_xml = debug_xml + "   <unidadMedida>" + producto.unidadMedida + "\r\n";
                //debug_xml = debug_xml + "   <valorReferencialUnitario>" + producto.valorReferencialUnitario + "\r\n";
                //debug_xml = debug_xml + "   <valorUnitarioBI>" + producto.valorUnitarioBI + "\r\n";
                //debug_xml = debug_xml + "   <valorVentaItemQxBI>" + producto.valorVentaItemQxBI + "\r\n";
                //debug_xml = debug_xml + "   <numeroOrden>" + producto.numeroOrden + "\r\n";
                //}

                // SECCION PRODUCTO IGV
                //producto.IGV = new ProductoIGV();
                //switch (producto_gp.tipoAfectacion.ToString().Trim())
                //{
                //case "20":
                //producto.IGV.baseImponible = producto_gp.montoImponibleExonera.ToString("0.00");
                //break;
                //case "21":
                //producto.IGV.baseImponible = producto_gp.montoImponibleGratuito.ToString("0.00");
                //break;
                //case "30":
                // producto.IGV.baseImponible = producto_gp.montoImponibleInafecto.ToString("0.00");
                //break;
                //case "35":
                //producto.IGV.baseImponible = producto_gp.montoImponibleInafecto.ToString("0.00");
                //break;
                //case "40":
                //producto.IGV.baseImponible = producto_gp.montoImponibleExporta.ToString("0.00");
                //break;
                //default:
                //producto.IGV.baseImponible = producto_gp.montoImponibleIva.ToString("0.00");
                //break;
                //}

                //producto.IGV.monto = producto_gp.montoIva.ToString("0.00");
                //producto.IGV.tipo = producto_gp.tipoAfectacion.ToString().Trim();

                //if (!string.IsNullOrEmpty(documentoGP.DocVenta.infoRelNotasCodigoTipoNota))
                //{
                //producto.IGV.porcentaje = string.Format("{0,8:0.00}", producto_gp.porcentajeIva * 100).Trim();
                //}
                //else
                //{
                //producto.IGV.porcentaje = string.Format("{0,8:0.00}", producto_gp.porcentajeIva * 100).Trim();
                //}
                //{
                /*
                debug_xml = debug_xml + "   <IGV>\r\n";
                debug_xml = debug_xml + "       <baseImponible>" + producto.IGV.baseImponible + "\r\n";
                debug_xml = debug_xml + "           <baseImponibleIVA>" + producto_gp.montoImponibleIva.ToString("0.00") + "\r\n";
                debug_xml = debug_xml + "           <baseImponibleExo>" + producto_gp.montoImponibleExonera.ToString("0.00") + "\r\n";
                debug_xml = debug_xml + "           <baseImponibleExp>" + producto_gp.montoImponibleExporta.ToString("0.00") + "\r\n";
                debug_xml = debug_xml + "           <baseImponibleGra>" + producto_gp.montoImponibleGratuito.ToString("0.00") + "\r\n";
                debug_xml = debug_xml + "           <baseImponibleIna>" + producto_gp.montoImponibleInafecto.ToString("0.00") + "\r\n";
                debug_xml = debug_xml + "       <porcentaje>" + producto.IGV.porcentaje + "\r\n";
                debug_xml = debug_xml + "       <monto>" + producto.IGV.monto + "\r\n";
                debug_xml = debug_xml + "       <tipo>" + producto.IGV.tipo + "\r\n";
            */
                //}

                //SECCION IMPUESTOS DETALLES

                //SECCION PRODUCTO DESCUENTO

                //if (producto_gp.descuento != 0)
                //{
                //producto.descuento = new ProductoDescuento();
                //producto.descuento.baseImponible = string.Format("{0,14:0.00}", producto_gp.descuentoBaseImponible).Trim();
                //producto.descuento.monto = string.Format("{0,14:0.00}", producto_gp.descuento).Trim();
                //producto.descuento.porcentaje = string.Format("{0,8:0.00000}", producto_gp.descuentoPorcentaje * 100).Trim();
                //producto.descuento.codigo = producto_gp.descuentoCodigo;

                //if (producto_gp.descuentoCodigo == "01")
                //{
                //DescItemIGV = false;
                //}

                //{
                //debug_xml = debug_xml + "   <DESC ITEM>" + "\r\n";
                //debug_xml = debug_xml + "       <baseImponible>" + producto.descuento.baseImponible + "\r\n";
                //debug_xml = debug_xml + "       <monto>" + producto.descuento.monto + "\r\n";
                //debug_xml = debug_xml + "       <porcentaje>" + producto.descuento.porcentaje + "\r\n";
                //debug_xml = debug_xml + "       <codigo>" + producto.descuento.codigo + "\r\n";
                //}

                //}

                //DocEnviarWS.producto[i] = producto;
                DocEnviarWS.detalleDeFactura[i] = detalle1;
                //debug_xml = debug_xml + "  <PRODUCTO>" + DocEnviarWS.producto[i].codigoPLU + " Imp:" + DocEnviarWS.producto[i].valorVentaItemQxBI + "\r\n";
                //debug_xml = debug_xml + "       IGVporc: " + DocEnviarWS.producto[i].IGV.porcentaje + "\r\n";

                //Aumenta contadoresDocEnviarWS.producto[i].
                i++;
                correlativo++;
            }
            debug_xml = debug_xml + "<FIN DETALLES>\r\n";
            //FIN SECCION DETALLES DE LA FACTURA

            //SECCION RELACIONADOS
            //DocEnviarWS.documentosReferenciados = new DocumentosReferenciados[1];
            //DocumentosReferenciados documentosreferenciados1 = new DocumentosReferenciados();
            //documentosreferenciados1.codigoInterno = documentoGP._LDocVentaRelacionados[0].codigoInterno;
            //documentosreferenciados1.cufeDocReferenciado = documentoGP._LDocVentaRelacionados[0].cufeDocReferenciado;
            //documentosreferenciados1.cufeDescripcion = documentoGP._LDocVentaRelacionados[0].cufeDescripcion;
            //documentosreferenciados1.fecha = documentoGP._LDocVentaRelacionados[0].fecha;
            //documentosreferenciados1.numeroDocumento = documentoGP._LDocVentaRelacionados[0].numeroDocumento;
            //documentosreferenciados1.tipoCufe = documentoGP._LDocVentaRelacionados[0].tipoCufe;
            //DocEnviarWS.documentosReferenciados[0] = documentosreferenciados1;
            //debug_xml = debug_xml + "<DOCUMENTOS REFERENCIADOS>\r\n";
            //debug_xml = debug_xml + "<documentos referenciados>" + DocEnviarWS.documentosReferenciados[0] + "\r\n";
            //debug_xml = debug_xml + "<FIN DOCUMENTOS REFERENCIADOS>\r\n";
            //FIN DOCUMENTOS REFERENCIADOS

            //SECCION EXTRA
            //DocEnviarWS.extras = new Extras[1];
            //Extras comentarioHeader = new Extras();
            //comentarioHeader.nombre = documentoGP.DocVenta.nombre;
            //comentarioHeader.valor = documentoGP.DocVenta.valor;
            //comentarioHeader.xml = documentoGP.DocVenta.xml.ToString();
            //comentarioHeader.pdf = documentoGP.DocVenta.pdf.ToString();
            //comentarioHeader.controlInterno1 = documentoGP.DocVenta.controlInterno1;
            //comentarioHeader.controlInterno2 = documentoGP.DocVenta.controlInterno2;
            //comentarioHeader.nombre = "100100";
            //comentarioHeader.valor = "MENSAJES PERSONALIZADO 1<br>MENSAJES PERSONALIZADO 2<br>MENSAJES PERSONALIZADO 3<br>MENSAJES PERSONALIZADO 4<br>MENSAJES PERSONALIZADO 5";
            //comentarioHeader.xml = "1";
            //comentarioHeader.pdf = "1";
            //comentarioHeader.controlInterno1 = "encabezado";
            //comentarioHeader.controlInterno2 = "";
            //Extras comentarioFooter = new Extras();
            //comentarioFooter.nombre = "100200";
            //comentarioFooter.valor = "MENSAJES PERSONALIZADO 1<br>MENSAJES PERSONALIZADO 2<br>MENSAJES PERSONALIZADO 3<br>MENSAJES PERSONALIZADO 4<br>MENSAJES PERSONALIZADO 5<br>MENSAJES PERSONALIZADO 6";
            //comentarioFooter.xml = "1";
            //comentarioFooter.pdf = "1";
            //comentarioFooter.controlInterno1 = "pie de pagina";
            //Extras vendedor = new Extras();
            //vendedor.nombre = "443";
            //vendedor.valor = "Pablo Marmol";
            //vendedor.xml = "1";
            //vendedor.pdf = "1";
            //vendedor.controlInterno1 = "";
            //Extras pedido0 = new Extras();
            //pedido0.nombre = "140";
            //pedido0.valor = "PED";
            //pedido0.xml = "1";
            //pedido0.pdf = "1";
            //pedido0.controlInterno1 = "";
            //Extras pedido1 = new Extras();
            //pedido1.nombre = "141";
            //pedido1.valor = "01234";
            //pedido1.xml = "1";
            //pedido1.pdf = "1";
            //pedido1.controlInterno1 = "";
            //DocEnviarWS.extras[0] = comentarioHeader;
            //DocEnviarWS.extras[1] = comentarioFooter;
            //DocEnviarWS.extras[2] = vendedor;
            //DocEnviarWS.extras[3] = pedido0;
            //DocEnviarWS.extras[4] = pedido1;
            //debug_xml = debug_xml + "<EXTRAS>\r\n";
            //debug_xml = debug_xml + DocEnviarWS.extras[0];
            //debug_xml = debug_xml + DocEnviarWS.extras[1];
            //debug_xml = debug_xml + DocEnviarWS.extras[2];
            //debug_xml = debug_xml + DocEnviarWS.extras[3];
            //debug_xml = debug_xml + DocEnviarWS.extras[4];
            //debug_xml = debug_xml + "<FIN EXTRAS>\r\n";
            //FIN EXTRAS

            //SECCION FACTURA MEDIOS DE PAGO
            //MEDIOS DE PAGO VIENE DE LA VISTA vwCfdiMediosDePago]
            DocEnviarWS.mediosDePago = new MediosDePago[1];
            MediosDePago mediopago1 = new MediosDePago();
            mediopago1.medioPago= documentoGP._medpag.mediopago;
            mediopago1.numeroDeReferencia = documentoGP._medpag.numeroreferencia;
            mediopago1.metodoDePago = documentoGP._medpag.metodopago;
            DocEnviarWS.mediosDePago[0] = mediopago1;
            //mediopago1.fechaDeVencimiento = documentoGP._medpag.fechadevencimiento;
            //DocEnviarWS.mediosDePago = new MediosDePago[1];
            //MediosDePago mediopago1 = new MediosDePago();
            //mediopago1.medioPago = documentoGP._medpag.mediopago;
            //mediopago1.metodoPago = documentoGP._medpag.metodopago;
            //mediopago1.numeroReferencia = documentoGP._medpag.numeroreferencia;
            debug_xml = debug_xml + "<MEDIOS DE PAGO>\r\n";
            debug_xml = debug_xml + "<mediopago>" + mediopago1.medioPago + "\r\n";
            debug_xml = debug_xml + "<metodopago>" + mediopago1.metodoDePago + "\r\n";
            debug_xml = debug_xml + "<numeroreferencia>" + mediopago1.numeroDeReferencia + "\r\n";
            debug_xml = debug_xml + "<FIN MEDIOS DE PAGO>\r\n";
            //FIN MEDIOS DE PAGO

            //DocEnviarWS.estadoPago = documentoGP.DocVenta.medioPagoDetraccion.ToString();
            //DocEnviarWS.estadoPago = "1";
            //DateTime fec;
            //string fes //= "", hos="", fof ="";
            //fes = documentoGP.DocVenta.fechaEmision.ToString("yyyy-MM-dd");
            //hos = documentoGP.DocVenta.horaEmision.ToString();
            //fof = fes + " " + hos;
            //fec = Convert.ToDateTime(documentoGP.DocVenta.fechaEmision);
            //fes = fec.ToString("yyyy-MM-dd HH:mm:ss");
            DocEnviarWS.fechaEmision = Convert.ToDateTime(documentoGP.DocVenta.fechaEmision).ToString("yyyy-MM-dd");
            //DocEnviarWS.fechaEmision = "2019-07-11 16:15:00";
            //DocEnviarWS.fechaEmisionDocumentoModificado = documentoGP.DocVenta.fechaEmision.ToString("yyyy-MM-dd HH:mm:ss");
            //DocEnviarWS.fechaEmisionDocumentoModificado = "2019-07-11 16:15:00";
            //fes = documentoGP.DocVenta.fechaVencimiento.ToString("yyyy-MM-dd"); ;
            DocEnviarWS.fechaVencimiento = Convert.ToDateTime(documentoGP.DocVenta.fechaVencimiento).ToString("yyyy-MM-dd");
            //DocEnviarWS.icoterms = documentoGP.DocVenta.icoterms.ToString();
            //DocEnviarWS.importeTotal = documentoGP.DocVenta.montoTotalVenta.ToString();
            //DocEnviarWS.importeTotal = tot.ToString("00.00");
            //DocEnviarWS.importeTotal = "2235";
            //DocEnviarWS.consecutivoDocumento = "PREF235";
            //DocEnviarWS.tipoDocumento = "01";
            //DocEnviarWS.rangoNumeracion = "G7YF-1";
            //cadena = documentoGP.DocVenta.cliente_numeroDocumento;
            //nrd = cadena.Split(' ');
            //DocEnviarWS.consecutivoDocumento = nrd[1];            
            //DocEnviarWS.consecutivoDocumento = documentoGP.DocVenta.idDocumento.ToString();
            //DocEnviarWS.consecutivoDocumentoModificado ="00000397";          
            //debug_xml = debug_xml + "<fechaemision>" + DocEnviarWS.fechaEmision + "\r\n";
            //debug_xml = debug_xml + "<fechaemisondocumentomodificado>" + DocEnviarWS.fechaEmisionDocumentoModificado + "\r\n";
            //debug_xml = debug_xml + "<fechavencimiento>" + DocEnviarWS.fechaVencimiento + "\r\n";
            //debug_xml = debug_xml + "<icoterms>" + DocEnviarWS.icoterms + "\r\n";
            //debug_xml = debug_xml + "<importetotal>" + DocEnviarWS.importeTotal + "\r\n";
            //debug_xml = debug_xml + "<FIN FACTURA>\r\n";

            //SECCION IMPUESTOS GLOBALES O GENERALES
            DocEnviarWS.impuestosGenerales = new FacturaImpuestos[1];
            FacturaImpuestos impuestosg1 = new FacturaImpuestos();
            impuestosg1.baseImponibleTOTALImp = documentoGP._facimpdet.baseimponibletotalimp.ToString();// ("00000000000000.00");
            //impuestosg1.baseImponibleTOTALImp = "2000";
            impuestosg1.codigoTOTALImp = documentoGP._facimpdet.codigoTotalImp.ToString();
            //impuestosg1.codigoTOTALImp = "01";
            impuestosg1.controlInterno = documentoGP._facimpdet.controlInterno.ToString();
            //pti = double.Parse("10.00");
            //impuestosg1.porcentajeTOTALImp = pti.ToString("00.00");
            impuestosg1.porcentajeTOTALImp = documentoGP._facimpdet.porcentajeTotalImp.ToString();
            //impuestosg1.porcentajeTOTALImp = Convert.ToDecimal((tot / 100)).ToString("00.00");
            //impuestosg1.valorTOTALImp = documentoGP.DocVenta.montoTotalImpuestos.ToString();
            //vti = Math.Round((float)documentoGP.DocVenta.montoSubtotalIvaImponible, 2) * pti;
            //impuestosg1.valorTOTALImp = vti.ToString("00000000000000.00");
            impuestosg1.valorTOTALImp = documentoGP._facimpdet.valorTotalImp.ToString();// ("00000000000000.00");
            //vti = Math.Round((float)(documentoGP._facimpdet.valorTotalImp * )
            impuestosg1.unidadMedida = documentoGP._facimpdet.unidadMedida;
            impuestosg1.unidadMedidaTributo = documentoGP._facimpdet.unidadMedidaTributo.ToString();
            impuestosg1.valorTributoUnidad = documentoGP._facimpdet.valorTributoUnidad.ToString();
            DocEnviarWS.impuestosGenerales[0] = impuestosg1;
            debug_xml = debug_xml + "<IMPUESTOS GENERALES>\n\r";
            debug_xml = debug_xml + "<impuestos generales>" + DocEnviarWS.impuestosGenerales[0] + "\r\n";
            debug_xml = debug_xml + "<FIN IMPUESTOS GENERALES>\n\r";
            //FIN SECCION IMPUESTOS GENERALES

            //SECCION IMPUESTOS TOTALES
            DocEnviarWS.impuestosTotales = new ImpuestosTotales[1];
            ImpuestosTotales impuestototales1 = new ImpuestosTotales();
            impuestototales1.codigoTOTALImp = documentoGP._facimpcab.codigoTotalImp;
            impuestototales1.montoTotal = documentoGP._facimpcab.valorTotalImp.ToString();// es una sumatoria de valorTotalImp
            DocEnviarWS.impuestosTotales[0] = impuestototales1;
            debug_xml = debug_xml + "<IMPUESTOS TOTALES>\n\r";
            debug_xml = debug_xml + "<impuestos totales>" + DocEnviarWS.impuestosTotales[0] + "\r\n";
            debug_xml = debug_xml + "<FIN IMPUESTOS TOTALES>\n\r";
            //FIN SECCION IMPUESTOS TOTALES


            //DocEnviarWS.importeTotal = (tot + vti).ToString("00000000000000.00");

            //SECCION FACTURA FINAL
            //debug_xml = debug_xml + "<FACTURA FINAL \n\r>";
            //DocEnviarWS.informacionAdicional = documentoGP.DocVenta.informacionAdicional;
            //DocEnviarWS.medioPago = documentoGP.DocVenta.medioPagoDetraccion
            //DocEnviarWS.medioPago = "10";
            DocEnviarWS.moneda = documentoGP.DocVenta.moneda.ToString();
            //DocEnviarWS.moneda = "COP";
            //DocEnviarWS.motivoNota = documentoGP.DocVenta.infoRelNotasObservaciones;
            //DocEnviarWS.propina = documentoGP.DocVenta.propina.ToString();
            DocEnviarWS.rangoNumeracion = documentoGP.DocVenta.rangonumeracion;
            DocEnviarWS.redondeoAplicado = documentoGP.DocVenta.redondeoaplicado.ToString();
            //TAZA DE CAMBIO
            DocEnviarWS.tasaDeCambio = new TasaDeCambio();
            TasaDeCambio tasadecambio1 = new TasaDeCambio();
            tasadecambio1.baseMonedaOrigen = "1.00";
            tasadecambio1.baseMonedaDestino = "1.00";
            tasadecambio1.fechaDeTasaDeCambio = "2019-08-19";
            tasadecambio1.monedaOrigen = "COP";
            tasadecambio1.monedaDestino = "COP";
            tasadecambio1.tasaDeCambio = documentoGP.DocVenta.tasaDeCambio.ToString();
            DocEnviarWS.tasaDeCambio = tasadecambio1;
            //FIN TAZA DE CAMBIO
            DocEnviarWS.tipoDocumento = documentoGP.DocVenta.tipoDocumento;
            DocEnviarWS.tipoOperacion = documentoGP.DocVenta.tipoOperacion;
            DocEnviarWS.totalBaseImponible = documentoGP.DocVenta.totalBaseImponible.ToString();// ("0000000000000000.000000");//ver si va este campo en la vista principal vwGenerarDocumentoDeVenta
            DocEnviarWS.totalBrutoConImpuesto = documentoGP.DocVenta.totalBrutoconImpuestos.ToString();// ("0000000000000000.000000");
            DocEnviarWS.totalMonto = Convert.ToDecimal(documentoGP.DocVenta.totalMonto).ToString();// ("0000000000000000.000000");
            DocEnviarWS.totalProductos = documentoGP.DocVenta.totalProductos.ToString();// ("00000");
            DocEnviarWS.totalSinImpuestos = documentoGP.DocVenta.totalSinImpuestos.ToString();// ("0000000000000000.000000");
            //DocEnviarWS.tipoDocumento= documentoGP.DocVenta.tipoDocumento.ToString();
            //DocEnviarWS.totalDescuentos = documentoGP.DocVenta.montoTotalDescuentosPorItem.ToString();
            //DocEnviarWS.totalDescuentos = tde.ToString("00000000000.00");
            //DocEnviarWS.totalSinImpuestos = tsi.ToString("00000000000000.00");
            //DocEnviarWS.uuidDocumentoModificado = documentoGP.DocVenta.uuidDocumentoModificado;
            //debug_xml = debug_xml + "<FACTURA FINAL>\n\r";
            //debug_xml = debug_xml + "<IMPUESTOS GENERALES>\n\r";
            //debug_xml = debug_xml + "<impuestos generales>" + DocEnviarWS.impuestosGenerales[0] + "\r\n";
            //debug_xml = debug_xml + "FIN IMPUESTOS GENERALES>\n\r";
            //debug_xml = debug_xml + "<información adicional>" + DocEnviarWS.informacionAdicional + "\r\n";
            //debug_xml = debug_xml + "<medio pago>" + DocEnviarWS.medioPago + "\r\n";
            //debug_xml = debug_xml + "<motivo nota>" + DocEnviarWS.motivoNota + "\r\n";
            //debug_xml = debug_xml + "<propina>" + DocEnviarWS.propina + "\r\n";
            debug_xml = debug_xml + "<rango numeración>" + DocEnviarWS.rangoNumeracion + "\r\n";
            debug_xml = debug_xml + "<redondeo aplicado>" + DocEnviarWS.redondeoAplicado + "\r\n";
            debug_xml = debug_xml + "<tasa de cambio>" + DocEnviarWS.tasaDeCambio + "\r\n";
            debug_xml = debug_xml + "<tipo documento>" + DocEnviarWS.tipoDocumento + "\r\n";
            debug_xml = debug_xml + "<tipo operacion>" + DocEnviarWS.tipoOperacion + "\r\n";
            debug_xml = debug_xml + "<total Base Imponible>" + DocEnviarWS.totalBaseImponible + "\r\n";
            debug_xml = debug_xml + "<total Bruto con Impuestos>" + DocEnviarWS.totalBrutoConImpuesto + "\r\n";
            debug_xml = debug_xml + "<total Monto>" + DocEnviarWS.totalMonto+ "\r\n";
            debug_xml = debug_xml + "<total Productos>" + DocEnviarWS.totalProductos + "\r\n";
            //debug_xml = debug_xml + "<total descuento>" + DocEnviarWS.totalDescuentos + "\r\n";
            debug_xml = debug_xml + "<total sin impuestos>" + DocEnviarWS.totalSinImpuestos + "\r\n";
            //debug_xml = debug_xml + "<uuid Documento Modificado>" + DocEnviarWS.uuidDocumentoModificado + "\r\n";
            //debug_xml = debug_xml + "<FIN FACTURA FINAL>";
            //FIN SECCION FACTURA FINAL

            debug_xml = debug_xml + "<FIN FACTURA GENERAL>";
            //FIN FACTURA GENERAL

            /*
            if (documentoGP.DocVenta.descuentoGlobalMonto != 0)
            {
                DocEnviarWS.descuentosGlobales = new DescuentosGlobales();
                
                DocEnviarWS.descuentosGlobales.baseImponible = documentoGP.DocVenta.descuentoGlobalImponible.ToString("0.00");
                DocEnviarWS.descuentosGlobales.monto = documentoGP.DocVenta.descuentoGlobalMonto.ToString("0.00");
                if (DescItemIGV)
                {
                    DocEnviarWS.descuentosGlobales.motivo = "02";
                }
                else
                {
                    DocEnviarWS.descuentosGlobales.motivo = "03";
                }
                DocEnviarWS.descuentosGlobales.porcentaje = string.Format("{0,8:0.00000}", documentoGP.DocVenta.descuentoGlobalPorcentaje).Trim();

                {
                    debug_xml = debug_xml + "<DESCUENTOS GLOBALES>" + "\r\n";
                    debug_xml = debug_xml + "    <baseImponible" + DocEnviarWS.descuentosGlobales.baseImponible + "\r\n";
                    debug_xml = debug_xml + "    <porcentaje>" + DocEnviarWS.descuentosGlobales.porcentaje + "\r\n";
                    debug_xml = debug_xml + "    <monto>" + DocEnviarWS.descuentosGlobales.monto + "\r\n";
                    debug_xml = debug_xml + "    <motivo>" + DocEnviarWS.descuentosGlobales.motivo + "\r\n";
                    debug_xml = debug_xml + "<FIN DESCUENTOS GLOBALES>" + "\r\n";
                }
            }
            else
            {
                debug_xml = debug_xml + "<SIN DESCUENTOS GLOBALES>" + "\r\n";
            }

            //SECCION DETRACCIONES
            if (string.IsNullOrEmpty(documentoGP.DocVenta.codigoDetraccion) || documentoGP.DocVenta.codigoDetraccion.Trim() == "00")
            {
                debug_xml = debug_xml + "<SIN DETRACCIONES>" + "\r\n";
            }
            else
            {
                debug_xml = debug_xml + "<DETRACCIONES>" + "\r\n";

                var detracciones = new Detraccion();
                detracciones.codigo = documentoGP.DocVenta.codigoDetraccion.Trim();
                //detracciones.medioPago = documentoGP.DocVenta.medioPagoDetraccion.Trim();
                detracciones.medioPago = "002";
                detracciones.monto = string.Format("{0,14:0.00}", documentoGP.DocVenta.montoDetraccion).Trim();
                detracciones.numCuentaBancodelaNacion = documentoGP.DocVenta.numCuentaBancoNacion.Trim();
                detracciones.porcentaje = string.Format("{0,8:0.00}", documentoGP.DocVenta.porcentajeDetraccion).Trim();

                DocEnviarWS.detraccion = new Detraccion[1];
                DocEnviarWS.detraccion[0] = detracciones;

                {
                    debug_xml = debug_xml + "    <codigo>" + DocEnviarWS.detraccion[0].codigo + "\r\n";
                    debug_xml = debug_xml + "    <medioPago>" + DocEnviarWS.detraccion[0].medioPago + "\r\n";
                    debug_xml = debug_xml + "    <monto>" + DocEnviarWS.detraccion[0].monto + "\r\n";
                    debug_xml = debug_xml + "    <numCuentaBancodelaNacion>" + DocEnviarWS.detraccion[0].numCuentaBancodelaNacion + "\r\n";
                    debug_xml = debug_xml + "    <porcentaje>" + DocEnviarWS.detraccion[0].porcentaje + "\r\n";
                }

                debug_xml = debug_xml + "<FIN DETRACCIONES>" + DocEnviarWS.detraccion[0].monto + "\r\n";
            }


            //SECCION TOTALES
            debug_xml = debug_xml + "<TOTALES>" + "\r\n";
            DocEnviarWS.totales = new Totales();
            DocEnviarWS.totales.importeTotalPagar = documentoGP.DocVenta.montoTotalVenta.ToString("0.00");
            DocEnviarWS.totales.importeTotalVenta = documentoGP.DocVenta.montoTotalVenta.ToString("0.00");
            DocEnviarWS.totales.montoTotalImpuestos = documentoGP.DocVenta.montoTotalImpuestos.ToString("0.00");
            DocEnviarWS.totales.subtotalValorVenta = string.Format("{0,14:0.00}", documentoGP.DocVenta.montoSubtotalValorVenta).Trim();
            DocEnviarWS.totales.sumaTotalDescuentosporItem = string.Format("{0,14:0.00}", documentoGP.DocVenta.montoTotalDescuentosPorItem).Trim();
            DocEnviarWS.totales.sumatoriaImpuestosOG = documentoGP.DocVenta.montoTotalImpuOperGratuitas.ToString("0.00");
            DocEnviarWS.totales.totalIGV = documentoGP.DocVenta.montoTotalIgv.ToString("0.00");

            {
                debug_xml = debug_xml + "   <importeTotalPagar>" + DocEnviarWS.totales.importeTotalPagar + "\r\n";
                debug_xml = debug_xml + "   <importeTotalVenta>" + DocEnviarWS.totales.importeTotalVenta + "\r\n";
                debug_xml = debug_xml + "   <montoTotalImpuestos>" + DocEnviarWS.totales.montoTotalImpuestos + "\r\n";
                debug_xml = debug_xml + "   <subtotalValorVenta>" + DocEnviarWS.totales.subtotalValorVenta + "\r\n";
                debug_xml = debug_xml + "   <sumaTotalDescuentosporItem>" + DocEnviarWS.totales.sumaTotalDescuentosporItem + "\r\n";
                debug_xml = debug_xml + "   <sumatoriaImpuestosOG>" + DocEnviarWS.totales.sumatoriaImpuestosOG + "\r\n";
                debug_xml = debug_xml + "   <totalIGV>" + DocEnviarWS.totales.totalIGV + "\r\n";
                debug_xml = debug_xml + "<FIN TOTALES>" + "\r\n";
            }

            //SECCION SUBTOTALES

            DocEnviarWS.totales.subtotal = new Subtotal();
            DocEnviarWS.totales.subtotal.IGV = documentoGP.DocVenta.montoSubtotalIvaImponible.ToString("0.00");
            DocEnviarWS.totales.subtotal.exoneradas = documentoGP.DocVenta.montoSubtotalExonerado.ToString("0.00");
            DocEnviarWS.totales.subtotal.exportacion = documentoGP.DocVenta.montoSubtotalExportacion.ToString("0.00");
            DocEnviarWS.totales.subtotal.gratuitas = documentoGP.DocVenta.montoSubtotalGratuito.ToString("0.00");
            DocEnviarWS.totales.subtotal.inafectas = documentoGP.DocVenta.montoSubtotalInafecto.ToString("0.00");
            {
                debug_xml = debug_xml + "   <SUBTOTALES>" + "\r\n";
                debug_xml = debug_xml + "       <IGV>" + DocEnviarWS.totales.subtotal.IGV + "\r\n";
                debug_xml = debug_xml + "       <exoneradas>" + DocEnviarWS.totales.subtotal.exoneradas + "\r\n";
                debug_xml = debug_xml + "       <exportacion>" + DocEnviarWS.totales.subtotal.exportacion + "\r\n";
                debug_xml = debug_xml + "       <gratuitas>" + DocEnviarWS.totales.subtotal.gratuitas + "\r\n";
                debug_xml = debug_xml + "       <inafectas>" + DocEnviarWS.totales.subtotal.inafectas + "\r\n";
                debug_xml = debug_xml + "   <FIN SUBTOTALES>" + "\r\n";
            }

            //SECCION PAGO
            DocEnviarWS.pago = new Pago();
            DocEnviarWS.pago.moneda = documentoGP.DocVenta.moneda;
            DocEnviarWS.pago.tipoCambio = documentoGP.DocVenta.xchgrate.ToString("0.00000");
            DocEnviarWS.pago.fechaInicio = DateTime.Now.ToString("yyyy-MM-dd");
            DocEnviarWS.pago.fechaFin = DateTime.Now.ToString("yyyy-MM-dd");

            //SECCION PERSONALIZACION PDF
            if (!string.IsNullOrEmpty(documentoGP.LeyendasXml))
            {
                XElement leyendasX = XElement.Parse(documentoGP.LeyendasXml);
                int numLeyendas = leyendasX.Elements().Count();
                if (!string.IsNullOrEmpty(leyendasX.Elements().FirstOrDefault().Attribute("S").Value) &&
                    !string.IsNullOrEmpty(leyendasX.Elements().FirstOrDefault().Attribute("T").Value) &&
                    !string.IsNullOrEmpty(leyendasX.Elements().FirstOrDefault().Attribute("V").Value)
                    )
                {
                    DocEnviarWS.personalizacionPDF = new PersonalizacionPDF[numLeyendas];
                    int idx = 0;
                    foreach (XElement child in leyendasX.Elements())
                    {
                        DocEnviarWS.personalizacionPDF[idx] = new PersonalizacionPDF();
                        DocEnviarWS.personalizacionPDF[idx].seccion = child.Attribute("S").Value;
                        DocEnviarWS.personalizacionPDF[idx].titulo = child.Attribute("T").Value;
                        DocEnviarWS.personalizacionPDF[idx].valor = child.Attribute("V").Value;
                        idx++;
                    }
                }
            }
            {
                debug_xml = debug_xml + "<PAGO>" + "\r\n";
                debug_xml = debug_xml + "   <moneda>" + DocEnviarWS.pago.moneda + "\r\n";
                debug_xml = debug_xml + "    <tipoCambio>" + DocEnviarWS.pago.tipoCambio + "\r\n";
                debug_xml = debug_xml + "<FIN PAGO>" + "\r\n";
            }

            debug_xml = debug_xml + "<LLAMADA>";
            */
            return DocEnviarWS;
        }



        //public string TimbraYEnviaASunat(string ruc, string usuario, string usuarioPassword, DocumentoVentaGP documentoGP)
        public string TimbraYEnviaServicioDeImpuesto(string ruc, string usuario, string usuarioPassword, DocumentoVentaGP documentoGP)
        {
            var docWs = ArmaDocumentoEnviarWS(documentoGP);

            //var response = ServicioWS.Enviar("a64532c2a3b14050b893e78832e714f160eacdfd", "25cf0e943ce74feaa717b1f5464ea6e4591b3809", docWs, "0");
            //var response = ServicioWS.Enviar("a64532c2a3b14050b893e78832e714f160eacdfd", "25cf0e943ce74feaa717b1f5464ea6e4591b3809", docWs, "0");
            var response = ServicioWS.Enviar("89ab70d025c1cb8c5bac3f5ac319a94728e42e3a", "3cfb75199b5d14cdb706a55555a055488b1fad6c", docWs, "0");
            //DocumentResponse docRespuesta = ServicioWS.Enviar("89ab70d025c1cb8c5bac3f5ac319a94728e42e3a", "3cfb75199b5d14cdb706a55555a055488b1fad6c", docWs, "0");

            if (response.codigo == 0)
            {

                byte[] converbyte = Convert.FromBase64String(response.xml.ToString());
                return System.Text.Encoding.UTF8.GetString(converbyte);
                //return response.xml.ToString();

                /*return "Mensaje XML: " + response.mensaje + Environment.NewLine +
                                                    "Código error: " + response.codigo + Environment.NewLine +
                                                    "Estatus: " + response.estatus + Environment.NewLine +
                                                    "Hora: " + response.hora + Environment.NewLine +
                                                    "Id Transacción: " + response.idtransaccion + Environment.NewLine +
                                                    "Numeración: " + response.numeracion + Environment.NewLine +
                                                    "CRC: " + response.crc + Environment.NewLine +
                                                    "DebugXML: " + debug_xml + Environment.NewLine  + 
                                                    "XML: " + converbyte.ToString();*/

            
            }
            else
            {   /*
                using (StreamWriter sw = File.CreateText(ruc + documentoGP.DocVenta.serie + documentoGP.DocVenta.numero + DateTime.Today.ToLongDateString()))
                {
                    sw.WriteLine(debug_xml);
                }
                */
                if (response.codigo == 202 || response.codigo == 207)
                    throw new ArgumentException(response.codigo.ToString() + " - " + response.mensaje);
                else
                    throw new TimeoutException("Excepción al conectarse con el Web Service de Facturación. " + response.codigo.ToString() + " - " + response.mensaje + " " + debug_xml);

            }

        }

        //public async Task<string> TimbraYEnviaASunatAsync(string ruc, string usuario, string usuarioPassword, DocumentoVentaGP documentoGP)
        public async Task<string> TimbraYEnviaServicioDeImpuestoAsync(string ruc, string usuario, string usuarioPassword, DocumentoVentaGP documentoGP)
        {
            var docWs = ArmaDocumentoEnviarWS(documentoGP);

            //var response = await ServicioWS.EnviarAsync(ruc, usuario, usuarioPassword, docWs);
            //var response = await ServicioWS.EnviarAsync("a64532c2a3b14050b893e78832e714f160eacdfd", "25cf0e943ce74feaa717b1f5464ea6e4591b3809", docWs, "0");
            var response = await ServicioWS.EnviarAsync("89ab70d025c1cb8c5bac3f5ac319a94728e42e3a", "3cfb75199b5d14cdb706a55555a055488b1fad6c", docWs, "0");
            //DocumentResponse docRespuesta  = await ServicioWS.EnviarAsync("89ab70d025c1cb8c5bac3f5ac319a94728e42e3a", "3cfb75199b5d14cdb706a55555a055488b1fad6c", docWs, "0");

            //if (response.codigo == 0)
            if (response.codigo == 0)
            {
                //docRespuesta.resultado.ToString();
                byte[] converbyte = Convert.FromBase64String(response.xml.ToString());
                return System.Text.Encoding.UTF8.GetString(converbyte);
             //DocumentResponse docRespuesta = new DocumentResponse(); //objeto Response del metodo enviar
                
            }
            else
            {
                if (response.codigo == 202 || response.codigo == 207)
                    throw new ArgumentException(response.codigo.ToString() + " - " + response.mensaje);
                else
                    throw new TimeoutException("Excepción al conectarse con el Web Service de Facturación. [TimbraYEnviaASunatAsync] " + response.codigo.ToString() + " - " + response.mensaje + " " + debug_xml + " ruc: " + ruc + " USu: " + usuario + "/" + usuarioPassword);

            }

        }

        public async Task<string> ObtienePDFdelOSEAsync(string ruc, string usuario, string usuarioPassword, string tipoDoc, string serie, string correlativo, string ruta, string nombreArchivo, string extension)
        {
            string rutaYNomArchivoPDF = Path.Combine(ruta, nombreArchivo + extension);

            //var response_descarga = await ServicioWS.DescargaArchivoAsync(usuario, usuarioPassword, ruc + "-" + tipoDoc + "-" + serie + "-" + correlativo, "PDF");
            var response_descarga = await ServicioWS.DescargaPDFAsync("a64532c2a3b14050b893e78832e714f160eacdfd", "25cf0e943ce74feaa717b1f5464ea6e4591b3809", "PRUE980338212");

            if (response_descarga.codigo == 0)
            {

                byte[] converbyte = Convert.FromBase64String(response_descarga.documento.ToString());

                using (FileStream SourceStream = File.Open(rutaYNomArchivoPDF, FileMode.OpenOrCreate))
                {
                    SourceStream.Seek(0, SeekOrigin.End);
                    await SourceStream.WriteAsync(converbyte, 0, converbyte.Length);
                }

                return rutaYNomArchivoPDF;
            }
        
            else
            {
                throw new Exception("Excepción al descargar el PDF del servicio web. " + response_descarga.codigo.ToString() + " - " + response_descarga.mensaje + " " + response_descarga.cufe);
            }
        }
        
        public string ObtienePDFdelOSE(string ruc, string usuario, string usuarioPassword, string tipoDoc, string serie, string correlativo, string ruta, string nombreArchivo, string extension)
        {
            string rutaYNomArchivoPDF = ruta + nombreArchivo + extension;

            try
            {
                //var response_descarga = ServicioWS.DescargaPDFAsync(usuario, usuarioPassword, ruc + "-" + tipoDoc + "-" + serie + "-" + correlativo, "PDF");
                var response_descarga = ServicioWS.DescargaPDF("a64532c2a3b14050b893e78832e714f160eacdfd", "25cf0e943ce74feaa717b1f5464ea6e4591b3809", "PRUE980338212");
                if (response_descarga.codigo == 0)
                {

                    byte[] converbyte = Convert.FromBase64String(response_descarga.documento.ToString());

                    if (!Directory.Exists(ruta))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(ruta);


                    }

                    using (FileStream Writer = new FileStream(rutaYNomArchivoPDF, FileMode.Create, FileAccess.Write))
                        Writer.Write(converbyte, 0, converbyte.Length);

                    return rutaYNomArchivoPDF;

                }
                else
                {
                    //throw new Exception(rutaYNomArchivoPDF+ " || " + response_descarga.mensaje + 2"||" + ruc + "-" + tipoDoc + "-" + serie + "-" + correlativo);
                    return rutaYNomArchivoPDF + "||" + response_descarga.codigo + "-" + response_descarga.mensaje + "/" + response_descarga.cufe + "||" + ruc + "-" + tipoDoc + "-" + serie + "-" + correlativo;
                }
            }
            catch (DirectoryNotFoundException)
            {
                string smsj = "Verifique la existencia de la carpeta indicada en la configuración de Ruta de archivos Xml de GP. La ruta de la carpeta no existe: " + rutaYNomArchivoPDF;
                throw new DirectoryNotFoundException(smsj);
            }
            catch (IOException)
            {
                string smsj = "Verifique permisos de escritura en la carpeta: " + rutaYNomArchivoPDF + ". No se pudo guardar el archivo xml.";
                throw new IOException(smsj);
            }
            catch (Exception eAFE)
            {
                string smsj;
                if (eAFE.Message.Contains("denied"))
                    smsj = "Elimine el archivo pdf antes de volver a generar uno nuevo. Luego vuelva a intentar. " + eAFE.Message;
                else
                    smsj = "Contacte a su administrador. No se pudo guardar el archivo XML. " + eAFE.Message + Environment.NewLine + eAFE.StackTrace;
                throw new IOException(smsj);
            }

            throw new NotImplementedException();
        }        
        
        public async Task<string> ObtieneXMLdelOSEAsync(string ruc, string usuario, string usuarioPassword, string tipoDoc, string serie, string correlativo)
        {
            //var response_descarga = await ServicioWS.DescargaArchivoAsync(usuario, usuarioPassword, ruc + "-" + tipoDoc + "-" + serie + "-" + correlativo, "XML");
            var response_descarga = await ServicioWS.DescargaXMLAsync("a64532c2a3b14050b893e78832e714f160eacdfd", "25cf0e943ce74feaa717b1f5464ea6e4591b3809", "PRUE980338212");
            if (response_descarga.codigo == 0)
            {
                return response_descarga.documento.ToString();
            }
            else
            {
                throw new Exception("Excepción al descargar el XML del servicio web. " + response_descarga.codigo.ToString() + " - " + response_descarga.mensaje + " " + response_descarga.cufe);
            }

        }

        public string ObtieneXMLdelOSE(string ruc, string usuario, string usuarioPassword, string tipoDoc, string serie, string correlativo)
        {

            string MsjError;
            try
            {
                //var response_descarga = ServicioWS.DescargaArchivo(usuario, usuarioPassword, ruc + "-" + tipoDoc + "-" + serie + "-" + correlativo, "XML");
                var response_descarga = ServicioWS.DescargaXML("a64532c2a3b14050b893e78832e714f160eacdfd", "25cf0e943ce74feaa717b1f5464ea6e4591b3809", "PRUE980338212");
                if (response_descarga.codigo == 0)
                {

                    return response_descarga.documento.ToString();

                }
                else
                {
                    MsjError = "Mensaje: " + response_descarga.mensaje + Environment.NewLine +
                               "Código error: " + response_descarga.codigo + Environment.NewLine;

                    throw new NotImplementedException(MsjError);
                }
            }
            catch (Exception)
            {
                throw new NotImplementedException("Mensaje: Error en el servicio");
            }


        }
        /*
        public string TimbraYEnviaASunat(string ruc, string usuario, string usuarioPassword, string texto)
        {
            throw new NotImplementedException();
        }
        */
        public string TimbraYEnviaServicioDeImpuesto(string ruc, string usuario, string usuarioPassword, string texto)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SolicitarBajaAsync(string ruc, string usuario, string usuarioPassword, string nroDocumento, string motivo)
        {
            //var baja = await ServicioWS.DescargaPDFAsync(ruc, usuario, usuarioPassword, nroDocumento, motivo);
            var baja= await ServicioWS.DescargaPDFAsync("a64532c2a3b14050b893e78832e714f160eacdfd", "25cf0e943ce74feaa717b1f5464ea6e4591b3809", "PRUE980338212");
            if (baja.codigo == 0)
            {
                return baja.mensaje;
            }
            else
            {
                throw new Exception("Excepción al solicitar la baja al servicio web. " + baja.codigo.ToString() + " - " + baja.mensaje + " " + baja.cufe);
            }

        }

        //public Task<string> ObtieneCDRdelOSEAsync(string ruc, string tipoDoc, string serie, string correlativo, string rutaYNomArchivoCfdi)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<string> ConsultaStatusAlOSEAsync(string ruc, string usuario, string usuarioPassword, string tipoDoc, string serie, string correlativo)
        {
            //string rutaYNomArchivoPDF = Path.Combine(ruta, nombreArchivo + extension);

            //var response_descarga = await ServicioWS.EstatusDocumentoAsync(usuario, usuarioPassword, ruc + "-" + tipoDoc + "-" + serie + "-" + correlativo);
            var response_descarga = await ServicioWS.EstadoDocumentoAsync("a64532c2a3b14050b893e78832e714f160eacdfd", "25cf0e943ce74feaa717b1f5464ea6e4591b3809", "PRUE980338212");
            return string.Concat(response_descarga.codigo.ToString(), "-", response_descarga.mensaje);
            //if (response_descarga.codigo == 0)
            //{
            //    return response_descarga.codigo.ToString();
            //}
            //else
            //{
            //    return string.Concat(response_descarga.codigo.ToString(), " - ", response_descarga.mensaje);
            //}

        }

        //public string ObtieneCDRdelOSE(string ruc, string tipoDoc, string serie, string correlativo)
        //{
        //    throw new NotImplementedException();
        //}
        //public Tuple<string, string> Baja(string ruc, string usuario, string usuarioPassword, string nroDocumento, string motivo)
        //{
        //        throw new NotImplementedException();
        //}
        //public Tuple<string, string> ResumenDiario(string ruc, string usuario, string usuarioPassword, string texto)
        //{
        //    throw new NotImplementedException();
        //}
    
    }
    
}


