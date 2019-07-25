/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            DocEnviarWS = new FacturaGeneral();
            int i = 0; // Variable para loopear
            double tot = 0;
            double pti = 0, vti = 0;
            double tde = 0, tsi = 0;
            int correlativo = 1; // Variable para corre de productos;
            Boolean DescItemIGV = true;
            string cadena = "";
            string[] nrd;
            debug_xml = "";

            //Seccion emisor
            DocEnviarWS.cliente = new Cliente();
            //DocEnviarWS.cliente.apellido = documentoGP.DocVenta.receptorNombre;            
            DocEnviarWS.cliente.nombreRazonSocial = documentoGP.DocVenta.receptorNombre;
            //DocEnviarWS.cliente.departamento = "no aplica  no hay en la base de datos";
            DocEnviarWS.cliente.direccion = documentoGP.DocVenta.receptorDireccion;
            //DocEnviarWS.cliente.pais = documentoGP.DocVenta.receptorPais;
            DocEnviarWS.cliente.email = documentoGP.DocVenta.emailTo;
            DocEnviarWS.cliente.numeroDocumento = documentoGP.DocVenta.receptorNroDoc;
            //DocEnviarWS.cliente.numeroDocumento = "23456783456734567832";

            //DocEnviarWS.cliente.tipoIdentificacion = documentoGP.DocVenta.idDocumento;
            //DocEnviarWS.cliente.tipoPersona = documentoGP.DocVenta.tipoPersona;
            //DocEnviarWS.cliente.notificar = documentoGP.DocVenta.notificar;
            //DocEnviarWS.cliente.regimen = documentoGP.DocVenta.regimen;            
            DocEnviarWS.cliente.tipoIdentificacion = "31";
            DocEnviarWS.cliente.tipoPersona = "1";
            DocEnviarWS.cliente.notificar = "NO";
            DocEnviarWS.cliente.regimen = "2";
            //DocEnviarWS.cliente.telefono = documentoGP.DocVenta.emisorTelefono;
            //DocEnviarWS.cliente.nombreRazonSocial = documentoGP.DocVenta.emisorRazonSocial;
            //DocEnviarWS.cliente.referencia2 = documentoGP.DocVenta.emisorReferencia2;
            //DocEnviarWS.cliente.subDivision = documentoGP.DocVenta.emisorSubdivision;
            //DocEnviarWS.cliente.segundoNombre = documentoGP.DocVenta.emisorSegundoNombre;
            
            //DocEnviarWS.emisor.nombreComercial = documentoGP.DocVenta.emisorNombre;
            //DocEnviarWS.emisor.lugarExpedicion = documentoGP.DocVenta.emmisor**
            //DocEnviarWS.emisor.domicilioFiscal = documentoGP.DocVenta.emisorDireccion;
            //.cliente. = documentoGP.DocVenta.emisorUrbanizacion;
            //DocEnviarWS.cliente.distrito = documentoGP.DocVenta.emisorDistrito;
            //DocEnviarWS.cliente.provincia = documentoGP.DocVenta.emisorProvincia;
            //DocEnviarWS.cliente.departamento = documentoGP.DocVenta.emisorDepartamento;
            //DocEnviarWS.cliente.codigoPais = documentoGP.DocVenta.emisorCodPais;
            //DocEnviarWS.cliente.ubigeo = documentoGP.DocVenta.emisorUbigeo;*/

            //debug_xml = "<EMISOR>" + DocEnviarWS.emisor.ruc + "\r\n";
            debug_xml = "<EMISOR>" + DocEnviarWS.cliente + "\r\n";

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
            //     DocEnviarWS.receptor.telefono = documentoGP.DocVenta.
            //DocEnviarWS.receptor.tipoDocumento = documentoGP.DocVenta.receptorTipoDoc;
            //    DocEnviarWS.receptor.ubigeo = documentoGP.DocVenta.rece

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
            
            //SECCION DETALLE DE LA FACTURA
            DocEnviarWS.detalleDeFactura = new FacturaDetalle[documentoGP.LDocVentaConceptos.Count()];
            debug_xml = debug_xml + "<CANT PROD>" + DocEnviarWS.detalleDeFactura.Count() + "\r\n";
            i = 0; correlativo = 1;
            foreach(vwCfdiConceptos detalleDeFactura_gp in documentoGP.LDocVentaConceptos)
            {
                //detalle de la factura
                var detalle1 = new FacturaDetalle();                 
                detalle1.codigoProducto = detalleDeFactura_gp.ITEMNMBR.ToString();
                detalle1.descripcion = detalleDeFactura_gp.Descripcion;
                detalle1.unidadMedida = "UN.";
                detalle1.cantidadUnidades = Math.Round((double)detalleDeFactura_gp.cantidad,2).ToString("00000000000.00");
                detalle1.descuento = Math.Round((double)detalleDeFactura_gp.descuento,2).ToString("0000000000000.00");
                detalle1.precioVentaUnitario = Math.Round((double)detalleDeFactura_gp.valorUni, 2).ToString("00000000000000.00");
                detalle1.precioTotalSinImpuestos = (Math.Round((double)detalleDeFactura_gp.importe, 2) - Math.Round((double)detalleDeFactura_gp.descuento, 2)).ToString("00000000000000.00");
                //producto.detalleAdicionalNombre = detalleDeFactura_gp
                //producto.detalleAdiiconalValor = detalleDeFactura_gp
                //impuestos detalle
                detalle1.impuestosDetalles = new FacturaImpuestos[1];
                FacturaImpuestos impuestodetalles1 = new FacturaImpuestos();
                impuestodetalles1.baseImponibleTOTALImp = Math.Round((double)detalleDeFactura_gp.montoImponibleIva,2).ToString("00000000000000.00");
                impuestodetalles1.codigoTOTALImp = "01";
                //impuestodetalles1.codigoTOTALImp = detalleDeFactura_gp.tipoTributo.ToString();
                impuestodetalles1.controlInterno = "056";
                //impuestodetalles1.controlInterno = detalleDeFactura_gp.controlInterno.ToString();
                impuestodetalles1.porcentajeTOTALImp = Convert.ToDecimal(detalleDeFactura_gp.porcentajeIva).ToString("00.00");
                impuestodetalles1.valorTOTALImp = Math.Round((double)detalleDeFactura_gp.montoIva,2).ToString("00000000000000.00");
                detalle1.impuestosDetalles[0] = impuestodetalles1;
                //fin impuestos detalles1
                //calculo del precio total
                tde = tde + Math.Round((double)detalleDeFactura_gp.descuento, 2);
                tsi = tsi + (Math.Round((double)detalleDeFactura_gp.importe, 2));
                detalle1.precioTotal = (Math.Round((double)detalleDeFactura_gp.importe, 2)- Math.Round((double)detalleDeFactura_gp.descuento, 2)+ Math.Round((double)detalleDeFactura_gp.montoIva, 2)).ToString("00000000000000.00");
                tot = tot + float.Parse(detalle1.precioTotal);                
                //detalle1.precioVentaUnitario = "2000";
                //detalle1.seriales = detalleDeFactura_gp.SERLTNUM;
                {
                    debug_xml = debug_xml + "<PRODUCTO>" + correlativo + "\r\n";
                    debug_xml = debug_xml + "<cantidad>" + detalle1.cantidadUnidades + "\r\n";
                    debug_xml = debug_xml + "   <codigoPLU>" + detalle1.codigoProducto + "\r\n";
                    debug_xml = debug_xml + "   <descripcion>" + detalle1.descripcion + "\r\n";
                    debug_xml = debug_xml + "<descuento>" + detalle1.descuento + "\r\n" ;
                    debug_xml = debug_xml + "<impuestosdetalles>" + detalle1.impuestosDetalles[0] + "\r\n ";
                    debug_xml = debug_xml + "<preciototal>" + detalle1.precioTotal + "\r\n";
                    debug_xml = debug_xml + "<preciototalsinimpuestos>" + detalle1.precioTotalSinImpuestos + "\r\n";
                    debug_xml = debug_xml + "<precioventaunitario>" + detalle1.precioVentaUnitario + "\r\n";
                    debug_xml = debug_xml + "<seriales>" + detalle1.seriales + "\r\n";
                    debug_xml = debug_xml + "<unidadmedida>" + detalle1.unidadMedida + "\r\n";
                }

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

            //SECCION FACTURA
            //corregir lo de abajo
            //DocEnviarWS.estadoPago = documentoGP.DocVenta.medioPagoDetraccion.ToString();
            DocEnviarWS.estadoPago = "1";
            //DateTime fec;
            //string fes = "", hos="", fof ="";
            //fes = documentoGP.DocVenta.fechaEmision.ToString("yyyy-MM-dd");
            //hos = documentoGP.DocVenta.horaEmision.ToString();
            //fof = fes + " " + hos;
            //fec = Convert.ToDateTime(documentoGP.DocVenta.fechaEmision);
            //fes = fec.ToString("yyyy-MM-dd HH:mm:ss");
            //DocEnviarWS.fechaEmision = documentoGP.DocVenta.fechaEmision.ToString(fof);
            DocEnviarWS.fechaEmision = "2019-07-11 16:15:00";
            //DocEnviarWS.fechaEmisionDocumentoModificado = documentoGP.DocVenta.fechaEmision.ToString("yyyy-MM-dd HH:mm:ss");
            DocEnviarWS.fechaEmisionDocumentoModificado = "2019-07-11 16:15:00";
            //DocEnviarWS.fechaVencimiento = documentoGP.DocVenta.fechaVencimiento.ToString();
            //DocEnviarWS.icoterms = documentoGP.DocVenta.icoterms.ToString();
            //DocEnviarWS.importeTotal = documentoGP.DocVenta.montoTotalVenta.ToString();
            //DocEnviarWS.importeTotal = tot.ToString("00.00");
            //DocEnviarWS.importeTotal = "2235";
            //DocEnviarWS.consecutivoDocumento = "PREF235";
            DocEnviarWS.tipoDocumento = "01";
            DocEnviarWS.rangoNumeracion = "G7YF-1";
            cadena = documentoGP.DocVenta.idDocumento.ToString();
            nrd = cadena.Split(' ');
            DocEnviarWS.consecutivoDocumento = nrd[1];
            //DocEnviarWS.consecutivoDocumento = documentoGP.DocVenta.idDocumento.ToString();
            //DocEnviarWS.consecutivoDocumentoModificado ="00000397";            
            debug_xml = debug_xml + "<FACTURA>\r\n";
            debug_xml = debug_xml + "<estadopago>" + DocEnviarWS.estadoPago + "\r\n";
            debug_xml = debug_xml + "<fechaemision>" + DocEnviarWS.fechaEmision + "\r\n";
            debug_xml = debug_xml + "<fechaemisondocumentomodificado>" + DocEnviarWS.fechaEmisionDocumentoModificado + "\r\n";
            debug_xml = debug_xml + "<fechavencimiento>" + DocEnviarWS.fechaVencimiento + "\r\n";
            debug_xml = debug_xml + "<icoterms>" + DocEnviarWS.icoterms + "\r\n";
            debug_xml = debug_xml + "<importetotal>" + DocEnviarWS.importeTotal + "\r\n";
            debug_xml = debug_xml + "<FIN FACTURA>\r\n";

            //SECCION IMPUESTOS GLOBALES
            DocEnviarWS.impuestosGenerales = new FacturaImpuestos[1];
            FacturaImpuestos impuestosg1 = new FacturaImpuestos();
            impuestosg1.baseImponibleTOTALImp = documentoGP.DocVenta.montoSubtotalIvaImponible.ToString("00000000000000.00");
            //impuestosg1.baseImponibleTOTALImp = "2000";
            //impuestosg1.codigoTOTALImp = documentoGP.DocVenta.codigoTotalImp.ToString();
            impuestosg1.codigoTOTALImp = "01";
            impuestosg1.controlInterno = "056";
            pti = double.Parse("10.00");
            impuestosg1.porcentajeTOTALImp = pti.ToString("00.00");
            //impuestosg1.porcentajeTOTALImp = Convert.ToDecimal((tot / 100)).ToString("00.00");
            //impuestosg1.valorTOTALImp = documentoGP.DocVenta.montoTotalImpuestos.ToString();
            vti = Math.Round((float)documentoGP.DocVenta.montoSubtotalIvaImponible, 2) * pti;
            impuestosg1.valorTOTALImp = vti.ToString("00000000000000.00");
            DocEnviarWS.impuestosGenerales[0] = impuestosg1;
            DocEnviarWS.importeTotal = (tot + vti).ToString("00000000000000.00");

            //SECCION FACTURA CONTINUACION
            //DocEnviarWS.informacionAdicional = documentoGP.DocVenta.informacionAdicional;
            //DocEnviarWS.medioPago = documentoGP.DocVenta.medioPagoDetraccion
            DocEnviarWS.medioPago = "10";
            //DocEnviarWS.moneda = documentoGP.DocVenta.moneda.ToString();
            DocEnviarWS.moneda = "COP";
            DocEnviarWS.motivoNota = documentoGP.DocVenta.infoRelNotasObservaciones;
            //DocEnviarWS.propina = documentoGP.DocVenta.propina.ToString();
            //DocEnviarWS.rangoNumeracion = documentoGP.DocVenta.numero;//-->ver si esta bien
            //DocEnviarWS.tipoDocumento= documentoGP.DocVenta.tipoDocumento.ToString();
            //DocEnviarWS.totalDescuentos = documentoGP.DocVenta.montoTotalDescuentosPorItem.ToString();
            //DocEnviarWS.totalSinImpuestos = documentoGP.DocVenta.totalsinImpuestos.ToString();
            DocEnviarWS.totalDescuentos = tde.ToString("00000000000.00");
            DocEnviarWS.totalSinImpuestos = tsi.ToString("00000000000000.00");
            //DocEnviarWS.uuidDocumentoModificado = documentoGP.DocVenta.uuidDocumentoModificado;
            debug_xml = debug_xml + "<FACTURA>\n\r";
            debug_xml = debug_xml + "<IMPUESTOS GENERALES>\n\r";
            debug_xml = debug_xml + "<impuestos generales>" + DocEnviarWS.impuestosGenerales[0] + "\r\n";
            debug_xml = debug_xml + "FIN IMPUESTOS GENERALES>\n\r";
            debug_xml = debug_xml + "<información adicional>" + DocEnviarWS.informacionAdicional + "\r\n";
            debug_xml = debug_xml + "<medio pago>" + DocEnviarWS.medioPago + "\r\n";
            debug_xml = debug_xml + "<motivo nota>" + DocEnviarWS.motivoNota + "\r\n";
            debug_xml = debug_xml + "<propina>" + DocEnviarWS.propina + "\r\n";
            debug_xml = debug_xml + "<rango numeración>" + DocEnviarWS.rangoNumeracion + "\r\n";
            debug_xml = debug_xml + "<tipo documento>" + DocEnviarWS.tipoDocumento + "\r\n";
            debug_xml = debug_xml + "<total descuento>" + DocEnviarWS.totalDescuentos + "\r\n";
            debug_xml = debug_xml + "<total sin impuestos>" + DocEnviarWS.impuestosGenerales + "\r\n";
            debug_xml = debug_xml + "<uuid Documento Modificado>" + DocEnviarWS.uuidDocumentoModificado + "\r\n";
            debug_xml = debug_xml + "<FIN FACTURA>";           
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
        
        
        
        public string TimbraYEnviaASunat(string ruc, string usuario, string usuarioPassword, DocumentoVentaGP documentoGP)
        {
            var docWs = ArmaDocumentoEnviarWS(documentoGP);

            var response = ServicioWS.Enviar("a64532c2a3b14050b893e78832e714f160eacdfd", "25cf0e943ce74feaa717b1f5464ea6e4591b3809", docWs, "0");

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
            {
                using (StreamWriter sw = File.CreateText(ruc + documentoGP.DocVenta.serie + documentoGP.DocVenta.numero + DateTime.Today.ToLongDateString()))
                {
                    sw.WriteLine(debug_xml);
                }
                if (response.codigo == 202 || response.codigo == 207)
                    throw new ArgumentException(response.codigo.ToString() + " - " + response.mensaje);
                else
                    throw new TimeoutException("Excepción al conectarse con el Web Service de Facturación. " + response.codigo.ToString() + " - " + response.mensaje + " " + debug_xml);

            }

        }

        public async Task<string> TimbraYEnviaASunatAsync(string ruc, string usuario, string usuarioPassword, DocumentoVentaGP documentoGP)
        {

            var docWs = ArmaDocumentoEnviarWS(documentoGP);

            //var response = await ServicioWS.EnviarAsync(ruc, usuario, usuarioPassword, docWs);
            var response = await ServicioWS.EnviarAsync("a64532c2a3b14050b893e78832e714f160eacdfd", "25cf0e943ce74feaa717b1f5464ea6e4591b3809", docWs, "0");

            if (response.codigo == 0)
            {
                byte[] converbyte = Convert.FromBase64String(response.xml.ToString());
                return System.Text.Encoding.UTF8.GetString(converbyte);

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

        public string TimbraYEnviaASunat(string ruc, string usuario, string usuarioPassword, string texto)
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


