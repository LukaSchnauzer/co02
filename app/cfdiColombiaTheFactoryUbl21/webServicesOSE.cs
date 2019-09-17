using cfdiEntidadesGP;
using cfdiColombiaInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using cfdiColombiaOperadorServiciosElectronicos.Colombia;
using System.Xml.Serialization;

namespace cfdiColombiaOperadorServiciosElectronicos
{
    public class WebServicesOSE : ICfdiMetodosWebService
    {
        private FacturaGeneral DocEnviarWS = new FacturaGeneral();
        //instancia un objeto que tiene la direccion del servicio web
        private ServiceClient ServicioWS = new ServiceClient();

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
            int correlativo = 1; // Variable para corre de productos;            

            //FACTURA GENERAL
            DocEnviarWS.cantidadDecimales = documentoGP.DocVenta.cantidadDecimales.ToString();
            DocEnviarWS.consecutivoDocumento = documentoGP.DocVenta.consecutivoDocumento;           
            DocEnviarWS.cliente = new Cliente();
            DocEnviarWS.cliente.nombreRazonSocial = documentoGP.DocVenta.cliente_nombreRazonSocial;            
            DocEnviarWS.cliente.numeroDocumento = documentoGP.DocVenta.cliente_numeroDocumento;
            DocEnviarWS.cliente.tipoIdentificacion = documentoGP.DocVenta.cliente_tipoIdentificacion.ToString();
            DocEnviarWS.cliente.tipoPersona = documentoGP.DocVenta.cliente_tipoPersona;
            DocEnviarWS.cliente.notificar = documentoGP.DocVenta.cliente_notificar;
            //DocEnviarWS.cliente.telefono = documentoGP.DocVenta.cliente_telefono;
            DocEnviarWS.cliente.email = documentoGP.DocVenta.cliente_email;
            //DIRECCION DEL CLIENTE
            DocEnviarWS.cliente.direccionCliente = new Direccion();
            Direccion direccion1 = new Direccion();
            direccion1.ciudad = documentoGP.DocVenta.cliente_difCiudad;
            direccion1.codigoDepartamento = documentoGP.DocVenta.cliente_difcodigoDepartamento;//"11";
            direccion1.departamento = documentoGP.DocVenta.cliente_difdepartamento;
            direccion1.direccion = documentoGP.DocVenta.cliente_difdireccion;//"Direccion";
            direccion1.lenguaje = documentoGP.DocVenta.cliente_diflenguaje;//"es";
            direccion1.municipio = documentoGP.DocVenta.cliente_difmunicipio;// "11001";
            direccion1.pais = documentoGP.DocVenta.cliente_difpais;// "CO";
            direccion1.zonaPostal = documentoGP.DocVenta.cliente_difzonapostal;//"110211";
            DocEnviarWS.cliente.direccionFiscal = direccion1;
            //FIN DIRECCION DEL CLIENTE
            DocEnviarWS.cliente.numeroIdentificacionDV = documentoGP.DocVenta.cliente_numeroIdentificacionDV;
            if (!string.IsNullOrEmpty( documentoGP.DocVenta.cliente_nombreComercial))
            {
                DocEnviarWS.cliente.nombreComercial = documentoGP.DocVenta.cliente_nombreComercial;
            }
            if(!string.IsNullOrEmpty(documentoGP.DocVenta.cliente_actividadEconomicaCIIU ))
            {
                DocEnviarWS.cliente.actividadEconomicaCIIU = documentoGP.DocVenta.cliente_actividadEconomicaCIIU;
            }

            //DESTINATARIOS
            int des = documentoGP._clides.Count();
            DocEnviarWS.cliente.destinatario = new Destinatario[des];
            int p = 0;
            foreach (vwCfdiClienteDestinatario destinatarios_gp in documentoGP._clides)           
            {
                var destinatario = new Destinatario();
                destinatario.email = new string[] { destinatarios_gp.cliente_email };
                destinatario.canalDeEntrega = destinatarios_gp.cliente_canalEntrega;
                DocEnviarWS.cliente.destinatario[p] = destinatario;

                p++;
            }
            //FIN DE DATOS DE CLIENTES VISTA DESTINATARIOS
            
            //SECCION DETALLES TRIBUTOS
            DocEnviarWS.cliente.detallesTributarios = new Tributos[1];
            Tributos tributos1 = new Tributos();
            tributos1.codigoImpuesto = documentoGP._facimpcab[0].codigoTotalImp; //"01";
            DocEnviarWS.cliente.detallesTributarios[0] = tributos1;
            //FIN DETALLES TRIBUTOS

            // SECCION DIRECCION FISCAL,
            Direccion direccionFiscal = new Direccion();            
            direccionFiscal.ciudad = documentoGP.DocVenta.cliente_difCiudad;
            direccionFiscal.codigoDepartamento = documentoGP.DocVenta.cliente_difcodigoDepartamento;//"11";
            direccionFiscal.departamento = documentoGP.DocVenta.cliente_difdepartamento;
            direccionFiscal.direccion = documentoGP.DocVenta.cliente_difdireccion;//"Direccion";
            direccionFiscal.lenguaje = documentoGP.DocVenta.cliente_diflenguaje;//"es";
            direccionFiscal.municipio = documentoGP.DocVenta.cliente_difmunicipio;// "11001";
            direccionFiscal.pais = documentoGP.DocVenta.cliente_difpais;// "CO";
            direccionFiscal.zonaPostal = documentoGP.DocVenta.cliente_difzonapostal;//"110211";
            DocEnviarWS.cliente.direccionFiscal = direccionFiscal;
            //FIN DIRECCION FISCAL

            //INFORMACION LEGAL DEL CLIENTE
            DocEnviarWS.cliente.informacionLegalCliente = new InformacionLegal();
            InformacionLegal informacionlegal1 = new InformacionLegal();
            informacionlegal1.nombreRegistroRUT = documentoGP.DocVenta.cliente_nombreRegistroRUT;
            informacionlegal1.numeroIdentificacion = documentoGP.DocVenta.cliente_numeroIdentificacion;
            informacionlegal1.numeroIdentificacionDV = documentoGP.DocVenta.cliente_numeroIdentificacionDV;
            informacionlegal1.tipoIdentificacion = documentoGP.DocVenta.cliente_tipoIdentificacion.ToString();
            DocEnviarWS.cliente.informacionLegalCliente = informacionlegal1;
            //FIN INFORMACION LEGAL DEL CLIENTE

            //OBLIGACIONES VIENE DE LA VISTA vwCfdiClienteObligacioS
            int cob = documentoGP._cliobl.Count();
            DocEnviarWS.cliente.responsabilidadesRut = new Obligaciones[cob];
            int obl = 0;
            foreach (vwCfdiClienteObligaciones obligaciones_gp in documentoGP._cliobl)
            {
                Obligaciones obligaciones1 = new Obligaciones();
                obligaciones1.obligaciones = obligaciones_gp.cliente_obligaciones; 
                obligaciones1.regimen = obligaciones_gp.cliente_regimen;
                DocEnviarWS.cliente.responsabilidadesRut[obl] = obligaciones1;
                obl++;
                //FIN OBLIGACIONES DE LA VISTA vwCfdiClienteObligaciones            
            }
            // FIN SECCION COMPROBANTE   

            //CARGOS DESCUENTOS
            DocEnviarWS.cargosDescuentos = new CargosDescuentos[1];
            CargosDescuentos cargosdescuentos1 = new CargosDescuentos();
            cargosdescuentos1.codigo = documentoGP.DocVenta.cargosdescuentos_codigo;
            cargosdescuentos1.descripcion = documentoGP.DocVenta.cargosdescuentos_descripcion;
            cargosdescuentos1.indicador = documentoGP.DocVenta.cargosdescuentos_indicador;
            cargosdescuentos1.monto = documentoGP.DocVenta.cargosdescuentos_monto.ToString();
            cargosdescuentos1.montoBase = documentoGP.DocVenta.cargosdescuentos_montobase.ToString();
            cargosdescuentos1.porcentaje = documentoGP.DocVenta.cargodescuentos_porcentaje.ToString();
            cargosdescuentos1.secuencia = documentoGP.DocVenta.cargosdescuentos_secuencia;
            DocEnviarWS.cargosDescuentos[0] = cargosdescuentos1;

            //SECCION CONCEPTOS O DETALLES DE LA FACTURA
            DocEnviarWS.detalleDeFactura = new FacturaDetalle[documentoGP.LDocVentaConceptos.Count()];
            i = 0; correlativo = 1;
            int toi = 0;
            decimal tsi = 0;
            foreach(vwCfdiConceptos detalleDeFactura_gp in documentoGP.LDocVentaConceptos)
            {
                //detalles basicos de la factura
                var detalle1 = new FacturaDetalle();
                detalle1.codigoProducto = detalleDeFactura_gp.facturadetalle_codigoproducto;
                detalle1.descripcion = detalleDeFactura_gp.facturadetalle_descripcion;
                detalle1.estandarCodigo = detalleDeFactura_gp.facturadetalle_estandarcodigo;
                detalle1.estandarCodigoProducto = detalleDeFactura_gp.facturadetalle_estandarcodigoproducto;
                detalle1.unidadMedida = detalleDeFactura_gp.facturadetalle_unidadMedida;
                detalle1.cantidadReal = Math.Round(detalleDeFactura_gp.facturadetalle_cantidadreal,0).ToString();
                detalle1.cantidadRealUnidadMedida = detalleDeFactura_gp.facturadetalle_cantidadrealunidadmedida;
                detalle1.cantidadUnidades = Math.Round(detalleDeFactura_gp.facturadetalle_cantidadunidades,0).ToString();
                detalle1.secuencia = detalleDeFactura_gp.facturadetalle_secuencia.ToString();                
                detalle1.cantidadPorEmpaque = detalleDeFactura_gp.facturadetalle_cantidadporempaque.ToString();
                detalle1.precioVentaUnitario = Math.Round(detalleDeFactura_gp.facturadetalle_precioVentaUnitario,2).ToString();// "00000000000000.00");
                detalle1.precioTotalSinImpuestos = Math.Round(detalleDeFactura_gp.facturadetalle_precioTotalSinImpuestos,2).ToString();//("00000000000000.00");
                detalle1.precioTotal = Math.Round(Convert.ToDecimal(detalleDeFactura_gp.facturadetalle_precioTotal),2).ToString();
                tsi = tsi + detalleDeFactura_gp.facturadetalle_precioTotalSinImpuestos;
                //FIN DETALLES BASICOS

                var impuestosDelConcepto = documentoGP.facimpdet.Where(x => x.LNITMSEQ == detalleDeFactura_gp.LNITMSEQ);

                //IMPUESTOS DETALLES DE FACTURA DETALLES 
                int imp3 = impuestosDelConcepto.Count();
                detalle1.impuestosDetalles = new FacturaImpuestos[imp3];
                int j = 0;
                foreach (var regImpuestosDelConcepto in impuestosDelConcepto)
                {
                    FacturaImpuestos facturaImpuestosDet = new FacturaImpuestos();
                    facturaImpuestosDet.baseImponibleTOTALImp = Math.Round(Convert.ToDecimal(regImpuestosDelConcepto.baseImponibleTotalImp), 2).ToString();//("00000000000000.00");
                    facturaImpuestosDet.codigoTOTALImp = regImpuestosDelConcepto.codigoTotalImp;
                    //facturaImpuestosDet.controlInterno = regImpuestosDelConcepto.controlInterno;
                    facturaImpuestosDet.porcentajeTOTALImp = Math.Abs(Math.Round(regImpuestosDelConcepto.porcentajeTotalImp, 2)).ToString(); //("00.00");
                    facturaImpuestosDet.valorTOTALImp = Math.Round(Convert.ToDecimal(regImpuestosDelConcepto.valorTotalImp), 2).ToString();// ("00000000000000.00");
                    if (!string.IsNullOrEmpty( regImpuestosDelConcepto.unidadMedida ))
                    {
                        facturaImpuestosDet.unidadMedida = regImpuestosDelConcepto.unidadMedida;
                    }
                    detalle1.impuestosDetalles[j] = facturaImpuestosDet;
                    j++;
                }

                //IMPUESTOS TOTALES DE FACTURA DETALLES
                var impTotalesDelConcepto = impuestosDelConcepto.GroupBy(ic => new { ic.codigoTotalImp })
                                            .Select(impuTotales => new
                                                {
                                                    CodigoTotalImp = impuTotales.Key.codigoTotalImp,
                                                    SumMontoTotal = impuTotales.Sum(s=>s.valorTotalImp),
                                                });

                int imp4 = impTotalesDelConcepto.Count();
                detalle1.impuestosTotales = new ImpuestosTotales[imp4];
                j = 0;
                foreach (var regImpTotalesConcepto in impTotalesDelConcepto)
                {
                    ImpuestosTotales impuestosTotales1 = new ImpuestosTotales();
                    impuestosTotales1.codigoTOTALImp = regImpTotalesConcepto.CodigoTotalImp;
                    impuestosTotales1.montoTotal = Math.Round(Convert.ToDecimal(regImpTotalesConcepto.SumMontoTotal), 2).ToString();
                    detalle1.impuestosTotales[j] = impuestosTotales1;
                    j++;
                }
                //FIN IMPUESTOS TOTALES DE FACTURA DETALLE  
                
                DocEnviarWS.detalleDeFactura[i] = detalle1;           
                //Aumenta contadoresDocEnviarWS.producto[i].
                i++;
                correlativo++;
                toi = toi + 1;
            }
            //FIN SECCION DETALLES DE LA FACTURA            

            //SECCION FACTURA MEDIOS DE PAGO. MEDIOS DE PAGO VIENE DE LA VISTA vwCfdiMediosDePago]
            int mep = documentoGP._medpag.Count();
            DocEnviarWS.mediosDePago = new MediosDePago[mep];
            for (int j = 0; j < mep; j++)
            {
                MediosDePago mediopago1 = new MediosDePago();
                mediopago1.medioPago = documentoGP._medpag[j].mediopago;
                mediopago1.numeroDeReferencia = documentoGP._medpag[j].numeroreferencia;
                mediopago1.metodoDePago = documentoGP._medpag[j].metodopago;
                DocEnviarWS.mediosDePago[j] = mediopago1;
            }
            //FIN MEDIOS DE PAGO            
            DocEnviarWS.fechaEmision = Convert.ToDateTime(documentoGP.DocVenta.fechaEmision).ToString("yyyy-MM-dd 00:00:00");
            
            DocEnviarWS.fechaVencimiento = Convert.ToDateTime(documentoGP.DocVenta.fechaVencimiento).ToString("yyyy-MM-dd");
            
            //SECCION IMPUESTOS GLOBALES O GENERALES
            int imp = documentoGP.facimpcab.Count();
            DocEnviarWS.impuestosGenerales = new FacturaImpuestos[imp];
            for (int j = 0; j < imp; j++)
            {
                FacturaImpuestos impuestosg1 = new FacturaImpuestos();
                impuestosg1.baseImponibleTOTALImp = Math.Round(Convert.ToDecimal(documentoGP.facimpcab[j].baseImponibleTotalImp),2).ToString();// ("00000000000000.00");
                impuestosg1.codigoTOTALImp = documentoGP.facimpcab[j].codigoTotalImp.ToString();
                //impuestosg1.controlInterno = documentoGP.facimpcab[j].controlInterno.ToString();
                impuestosg1.porcentajeTOTALImp = Math.Abs(Math.Round(documentoGP.facimpcab[j].porcentajeTotalImp,2)).ToString();
                impuestosg1.valorTOTALImp = Math.Round(Convert.ToDecimal(documentoGP.facimpcab[j].valorTotalImp),2).ToString();// ("00000000000000.00");
                impuestosg1.unidadMedida = documentoGP.facimpcab[j].unidadMedida;
                DocEnviarWS.impuestosGenerales[j] = impuestosg1;
                //FIN SECCION IMPUESTOS GENERALES
            }

            //SECCION IMPUESTOS TOTALES
            var impuestosTotalesCab = documentoGP._facimpcab.GroupBy(ic => new { ic.codigoTotalImp })
                            .Select(impuTotales => new
                            {
                                CodigoTotalImp = impuTotales.Key.codigoTotalImp,
                                SumMontoTotal = impuTotales.Sum(s => s.valorTotalImp),
                            });
            int imp2 = documentoGP._facimpcab.Count();
            DocEnviarWS.impuestosTotales = new ImpuestosTotales[imp2];
            int m = 0;
            foreach (var regImpuTotalesCab in impuestosTotalesCab)
            {
                ImpuestosTotales impuestototales1 = new ImpuestosTotales();
                impuestototales1.codigoTOTALImp = regImpuTotalesCab.CodigoTotalImp;
                impuestototales1.montoTotal = Math.Round(Convert.ToDecimal(regImpuTotalesCab.SumMontoTotal), 2).ToString();// es una sumatoria de valorTotalImp
                DocEnviarWS.impuestosTotales[m] = impuestototales1;
                m++;
            }
            //FIN SECCION IMPUESTOS TOTALES

            DocEnviarWS.moneda = documentoGP.DocVenta.moneda.ToString();
            DocEnviarWS.rangoNumeracion = documentoGP.DocVenta.rangonumeracion;
            DocEnviarWS.redondeoAplicado = documentoGP.DocVenta.redondeoaplicado.ToString();
            //TASA DE CAMBIO
            //tasaDeCambio = new TasaDeCambio();
            //DocEnviarWS.tasaDeCambio = new TasaDeCambio();
            //TasaDeCambio tasadecambio1 = new TasaDeCambio();            
            //tasadecambio1.baseMonedaDestino = documentoGP.DocVenta.tc_baseMonedaDestino.ToString(); ;
            //tasadecambio1.fechaDeTasaDeCambio = documentoGP.DocVenta.tc_fechaDeTasaDeCambio;
            //tasadecambio1.monedaOrigen = documentoGP.DocVenta.tc_monedaOrigen;
            //tasadecambio1.monedaDestino = documentoGP.DocVenta.tc_monedaDestino;
            //tasadecambio1.tasaDeCambio = documentoGP.DocVenta.tasaDeCambio.ToString();
            //DocEnviarWS.tasaDeCambio = tasadecambio1;
            //FIN TASA DE CAMBIO
            DocEnviarWS.tipoDocumento = documentoGP.DocVenta.tipoDocumento;
            DocEnviarWS.tipoOperacion = documentoGP.DocVenta.tipoOperacion;
            DocEnviarWS.totalBaseImponible = Math.Round(Convert.ToDecimal(documentoGP.DocVenta.totalBaseImponible),2).ToString();// ("0000000000000000.000000");
            DocEnviarWS.totalBrutoConImpuesto = Math.Round(Convert.ToDecimal(documentoGP.DocVenta.totalBrutoconImpuestos),2).ToString();
            DocEnviarWS.totalMonto = Math.Round(Convert.ToDecimal(documentoGP.DocVenta.totalMonto),2).ToString();// ("0000000000000000.000000");
            DocEnviarWS.totalProductos = toi.ToString();// ("00000");
            DocEnviarWS.totalSinImpuestos = Math.Round(Convert.ToDecimal(documentoGP.DocVenta.totalSinImpuestos),2).ToString();// ("0000000000000000.000000");
            //DocEnviarWS.totalSinImpuestos = Math.Round(tsi,2).ToString();
            //FIN SECCION FACTURA FINAL
            //FIN FACTURA GENERAL
            return DocEnviarWS;
        }

        public string TimbraYEnviaServicioDeImpuesto(string ruc, string usuario, string usuarioPassword, DocumentoVentaGP documentoGP)
        {
            var docWs = ArmaDocumentoEnviarWS(documentoGP);

            DocumentResponse response = ServicioWS.Enviar("89ab70d025c1cb8c5bac3f5ac319a94728e42e3a", "3cfb75199b5d14cdb706a55555a055488b1fad6c", docWs, "0");
            
            if (response.codigo == 200)
            {
                byte[] converbyte = Convert.FromBase64String(response.xml.ToString());
                return System.Text.Encoding.UTF8.GetString(converbyte);            
            }
            else
            {
                string docSerializado = string.Empty;
                XmlSerializer xml = new XmlSerializer(typeof(FacturaGeneral));
                using (StringWriter sw = new StringWriter() )
                {           
                    xml.Serialize(sw, docWs);
                    docSerializado = sw.ToString();
                }

                //if (response.codigo == 202 || response.codigo == 207)
                //    throw new ArgumentException(response.codigo.ToString() + " - " + response.mensaje + " - " + response.mensajesValidacion.ToString());
                //else
                throw new TimeoutException("Excepción al conectarse con el Web Service de Facturación [TimbraYEnviaServicioDeImpuesto] " + response.codigo.ToString() + " - " + response.mensaje + Environment.NewLine + response.mensajesValidacion.ToString() + Environment.NewLine + docSerializado);

            }

        }

        public async Task<string> TimbraYEnviaServicioDeImpuestoAsync(string ruc, string tokenEmpresa, string tokenPassword, DocumentoVentaGP documentoGP)
        {
            var docWs = ArmaDocumentoEnviarWS(documentoGP);
            var response = await ServicioWS.EnviarAsync(tokenEmpresa, tokenPassword, docWs, "0");
            if (response.codigo == 200)
            {
                byte[] converbyte = Convert.FromBase64String(response.xml.ToString());
                return System.Text.Encoding.UTF8.GetString(converbyte);
            }
            else
            {
                string docSerializado = string.Empty;
                XmlSerializer xml = new XmlSerializer(typeof(FacturaGeneral));
                using (StringWriter sw = new StringWriter())
                {
                    xml.Serialize(sw, docWs);
                    docSerializado = sw.ToString();
                }

                string msjErr=Environment.NewLine;
                if (response.mensajesValidacion.Count() > 0)
                    msjErr += string.Join(Environment.NewLine, response.mensajesValidacion);

                msjErr += Environment.NewLine + docSerializado;
                throw new InvalidOperationException(response.codigo.ToString() + " - " + response.mensaje + msjErr + "Excepción del Web Service de Facturación. [TimbraYEnviaServicioDeImpuestoAsync]");

            }

        }

        public async Task<string> ObtienePDFdelOSEAsync(string ruc, string usuario, string usuarioPassword, string tipoDoc, string serie, string correlativo, string ruta, string nombreArchivo, string extension)
        {
            string rutaYNomArchivoPDF = Path.Combine(ruta, nombreArchivo + extension);

            var response_descarga = await ServicioWS.DescargaPDFAsync(usuario, usuarioPassword, serie+correlativo);

            if (response_descarga.codigo == 200)
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
                throw new InvalidOperationException(response_descarga.codigo.ToString() + " - " + response_descarga.mensaje + " " + response_descarga.cufe + " Excepción al descargar el PDF del servicio web. [ObtienePDFdelOSEAsync] " );
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
            var response_descarga = await ServicioWS.DescargaXMLAsync(usuario, usuarioPassword, serie + correlativo);
            if (response_descarga.codigo == 200)
            {
                byte[] converbyte = Convert.FromBase64String(response_descarga.documento.ToString());
                return System.Text.Encoding.UTF8.GetString(converbyte);
            }
            else
            {
                throw new InvalidOperationException(response_descarga.codigo.ToString() + " - " + response_descarga.mensaje + " " + response_descarga.cufe + " Excepción al descargar el XML del servicio web. " );
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
            var response_descarga = await ServicioWS.EstadoDocumentoAsync(usuario, usuarioPassword, serie+correlativo);
            return string.Concat(response_descarga.codigo.ToString(), "-", response_descarga.mensaje);

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


