using Comun;
using MaquinaDeEstados;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using cfdiColombiaInterfaces;
using System.Xml.Linq;
using cfdiColombia;

namespace cfd.FacturaElectronica
{
    public class ProcesaCfdi
    {
        private Parametros _Param;
        private ConexionAFuenteDatos _Conex;
        private String nroTicket=String.Empty;
        private String _mensajeSunat = String.Empty;

        public string ultimoMensaje = "";
        vwCfdTransaccionesDeVenta trxVenta;

        internal vwCfdTransaccionesDeVenta TrxVenta
        {
            get
            {
                return trxVenta;
            }

            set
            {
                trxVenta = value;
            }
        }
        public delegate void LogHandler(int iAvance, string sMsj);
        public event LogHandler Progreso;

        /// <summary>
        /// Dispara el evento para actualizar la barra de progreso
        /// </summary>
        /// <param name="iProgreso"></param>
        public void OnProgreso(int iAvance, string sMsj)
        {
            if (Progreso != null)
                Progreso(iAvance, sMsj);
        }

        public ProcesaCfdi(ConexionAFuenteDatos Conex, Parametros Param)
        {
            _Param = Param;
            _Conex = Conex;
            //_client = new HttpClient { BaseAddress = new Uri(ConfigurationManager.AppSettings["UrlOpenInvoicePeruApi"]) };

        }

        /// <summary>
        /// Genera documentos xml: factura, boleta, nc, nd
        /// </summary>
        public async Task GeneraDocumentoXmlAsync(ICfdiMetodosWebService servicioTimbre)
        {
            string xmlFactura = string.Empty;
            string rutaYNom = string.Empty;
            try
            {
                String msj = String.Empty;
                trxVenta.Rewind(); //un objeto que la lista de todas las facturas que a marcado el usuario para emitir                                                         //move to first record
                string leyendas = await vwCfdTransaccionesDeVenta.ObtieneLeyendasAsync();
                int errores = 0; int i = 1;
                cfdReglasFacturaXml DocVenta = new cfdReglasFacturaXml(_Conex, _Param);     //log de facturas xml emitidas y anuladas
                ReglasME maquina = new ReglasME(_Param);
                ValidadorXML validadorxml = new ValidadorXML(_Param);
                TransformerXML loader = new TransformerXML();
                OnProgreso(1, "INICIANDO EMISION DE COMPROBANTES DE VENTA...");
                do
                {
                    msj = String.Empty;
                    String accion = "EMITE XML Y PDF";
                    try
                    {
                        if (//trxVenta.Estado.Equals("no emitido") &&
                            //maquina.ValidaTransicion(_Param.tipoDoc, accion, trxVenta.EstadoActual) &&
                            //trxVenta.EstadoContabilizado.Equals("contabilizado")//
                            1==1)                           

                        { 
                            if (trxVenta.Voidstts == 0)  //documento no anulado
                            {
                                trxVenta.ArmarDocElectronico(leyendas); 
                                String[] serieCorrelativo = trxVenta.DocGP.DocVenta.consecutivoDocumento.Split(new char[] { '-' });
                                string nombreArchivo = Utiles.FormatoNombreArchivo(trxVenta.Docid + trxVenta.Sopnumbe + "_" + trxVenta.s_CUSTNMBR, trxVenta.s_NombreCliente, 20) + "_" + accion.Substring(0, 2);
                                //validaciones
                                switch (trxVenta.DocGP.DocVenta.tipoDocumento)
                                {
                                    //case "07":
                                        //if (trxVenta.DocGP.LDocVentaRelacionados.Count() == 0)
                                        //{
                                            //msj = "La nota de crédito no está aplicada.";
                                            //continue;
                                        //}
                                        //else
                                       //// {
                                            //if (trxVenta.DocGP.LDocVentaRelacionados
                                                                        //.Where(f => f.sopnumbeTo.Substring(0, 1) == trxVenta.DocGP.DocVenta.consecutivoDocumento.Substring(0, 1))
                                                                        //.Count() 
                                                //!= trxVenta.DocGP.LDocVentaRelacionados.Count())
                                            //{
                                                //msj = "La serie de la nota de crédito y de la factura aplicada deben empezar con la misma letra: F o B.";
                                                //continue;
                                           // }
                                       // }
                                        /*
                                        if (string.IsNullOrEmpty(trxVenta.DocGP.DocVenta.infoRelNotasCodigoTipoNota))
                                        {
                                            msj = "No ha informado la causa de la discrepancia en la nota de crédito.";
                                            continue;
                                        }
                                        */
                                        //break;
                                    case "08":
                                        msj = "ok";
                                        break;
                                    case "01":
                                        msj = "ok";
                                        break;
                                    case "03":
                                        msj = "ok";
                                        break;
                                    default:
                                        msj = "No se puede emitir porque el tipo de documento: " + trxVenta.DocGP.DocVenta.tipoDocumento + " no está configurado.";
                                        throw new ApplicationException(msj);
                                }

                                string extension = ".xml";
                                rutaYNom = Path.Combine(trxVenta.RutaXml.Trim(), nombreArchivo + extension);
                                try
                                {
                                    // la siguiente linea envia el metodo timbraYEnviaServivio>DeImpuesto envia el objeto armado con la factura al servicio web.
                                    xmlFactura = await servicioTimbre.TimbraYEnviaServicioDeImpuestoAsync(trxVenta.DocGP.DocVenta.consecutivoDocumento, trxVenta.Ruta_certificadoPac, trxVenta.Contrasenia_clavePac, trxVenta.DocGP);
                                    //xmlFactura = servicioTimbre.TimbraYEnviaServicioDeImpuesto(trxVenta.DocGP.DocVenta.consecutivoDocumento, "89ab70d025c1cb8c5bac3f5ac319a94728e42e3a", "3cfb75199b5d14cdb706a55555a055488b1fad6c", trxVenta.DocGP);
                                    DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaYNom, "FAC", _Conex.Usuario, xmlFactura.Replace("encoding=\"utf-8\"", "").Replace("encoding=\"iso-8859-1\"", ""), maquina.DestinoStatusBase, maquina.DestinoEBinario, maquina.DestinoMensaje);

                                }
                                catch (ArgumentException ae)    //202 ó 207
                                {
                                    msj = ae.Message;
                                    xmlFactura = await servicioTimbre.ObtieneXMLdelOSEAsync(trxVenta.DocGP.DocVenta.consecutivoDocumento, trxVenta.Ruta_certificadoPac, trxVenta.Contrasenia_clavePac, trxVenta.DocGP.DocVenta.tipoDocumento, serieCorrelativo[0], serieCorrelativo[1]);
                                    DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaYNom, "FAC", _Conex.Usuario, xmlFactura.Replace("encoding=\"utf-8\"", "").Replace("encoding=\"iso-8859-1\"", ""), maquina.DestinoStatusBase, maquina.DestinoEBinario, maquina.DestinoMensaje);
                                }
                                catch(XmlException xm)
                                {
                                    msj = "Verifique la configuración de leyendas para la impresión PDF. [GeneraDocumentoXmlAsync] " + xm.Message + Environment.NewLine + xm.StackTrace;
                                    DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, "GeneraDocumentoXmlAsync " + xm.Message, "errLeyendas", _Conex.Usuario, string.Empty, "error", maquina.DestinoEBinario, xm.StackTrace);
                                    errores++;
                                }
                                catch (Exception lo)
                                {
                                    msj = "[GeneraDocumentoXmlAsync] " + lo.Message + Environment.NewLine + lo.StackTrace;
                                    DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, "GeneraDocumentoXmlAsync " + lo.Message, "errDFacture", _Conex.Usuario, string.Empty, "error", maquina.DestinoEBinario, lo.StackTrace);
                                    errores++;
                                }

                                if (!string.IsNullOrEmpty(xmlFactura))
                                {
                                    rutaYNom = await DocVenta.GuardaArchivoAsync(trxVenta, xmlFactura, nombreArchivo, extension, false);
                                    string tPdf = await servicioTimbre.ObtienePDFdelOSEAsync(trxVenta.DocGP.DocVenta.consecutivoDocumento,  trxVenta.Ruta_certificadoPac, trxVenta.Contrasenia_clavePac, trxVenta.DocGP.DocVenta.tipoDocumento, serieCorrelativo[0], serieCorrelativo[1], trxVenta.RutaXml.Trim(), nombreArchivo, ".pdf");
                                }
                            }
                            else //si el documento está anulado en gp, agregar al log como emitido
                            {
                                maquina.ValidaTransicion("FACTURA", "ANULA VENTA", trxVenta.EstadoActual, "emitido");
                                msj = "Anulado en GP y marcado como emitido.";
                                OnProgreso(1, msj);
                                DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, "Anulado en GP", "0", _Conex.Usuario, "", "emitido", maquina.eBinarioNuevo, msj.Trim());
                            }
                        }
                    }
                    catch (HttpRequestException he)
                    {
                        msj = string.Concat(he.Message, Environment.NewLine, he.StackTrace);
                        errores++;
                    }
                    catch (ArgumentException ae)
                    {
                        msj = ae.Message + Environment.NewLine + ae.StackTrace;
                        errores++;
                    }
                    catch (TimeoutException ae)
                    {
                        string imsj = ae.InnerException == null ? "" : ae.InnerException.ToString();
                        msj = ae.Message + " " + imsj + Environment.NewLine + ae.StackTrace;
                        errores++;
                    }
                    catch (DirectoryNotFoundException dnf)
                    {
                        msj = "El comprobante fue emitido, pero no se pudo guardar el archivo en: " + trxVenta.Ruta_clave + " Verifique si existe la carpeta." + Environment.NewLine;
                        DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, msj, "errCarpeta", _Conex.Usuario, string.Empty, "error", maquina.DestinoEBinario, dnf.Message);
                        msj += dnf.Message + Environment.NewLine;
                        errores++;
                    }
                    catch (IOException io)
                    {
                        msj = "El comprobante fue emitido, pero no se pudo guardar el archivo en: " + trxVenta.Ruta_clave + " Verifique permisos a la carpeta." + Environment.NewLine;
                        DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, msj, "errIO", _Conex.Usuario, string.Empty, "error", maquina.DestinoEBinario, io.Message);
                        msj += io.Message + Environment.NewLine;
                        errores++;
                    }
                    catch (Exception lo)
                    {
                        string imsj = lo.InnerException == null ? "" : lo.InnerException.ToString();
                        msj = lo.Message + " " + imsj + Environment.NewLine + lo.StackTrace;
                        DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, lo.Message, "errDesconocido", _Conex.Usuario, string.Empty, "error", maquina.DestinoEBinario, lo.StackTrace);
                        errores++;
                    }
                    finally
                    {
                        OnProgreso(i * 100 / trxVenta.RowCount, "Doc:" + trxVenta.Sopnumbe + " " + msj.Trim() + Environment.NewLine);              //Notifica al suscriptor
                        i++;
                    }
                } while (trxVenta.MoveNext() && errores < 10);
            }
            catch (Exception xw)
            {
                string imsj = xw.InnerException == null ? "" : xw.InnerException.ToString();
                this.ultimoMensaje = xw.Message + " " + imsj + Environment.NewLine + xw.StackTrace;
            }
            finally
            {
                OnProgreso(100, ultimoMensaje);
            }
            OnProgreso(100, "Proceso finalizado!");
        }

        public async Task<string> ProcesaConsultaStatusAsync(ICfdiMetodosWebService servicioTimbre)
        {
            string resultadoSunat = string.Empty;
            try
            {
                String msj = String.Empty;
                String eBinario = String.Empty;
                trxVenta.Rewind();                                                          //move to first record

                int errores = 0;
                int i = 1;
                cfdReglasFacturaXml DocVenta = new cfdReglasFacturaXml(_Conex, _Param);     //log de facturas xml emitidas y anuladas
                ReglasME maquina = new ReglasME(_Param);
                String accion = "CONSULTA STATUS";

                OnProgreso(1, "INICIANDO CONSULTA DE STATUS...");              //Notifica al suscriptor
                do
                {
                    msj = String.Empty;
                    String claseDocumento = !trxVenta.Docid.Equals("RESUMEN") ? _Param.tipoDoc : trxVenta.Docid;
                    try
                    {
                        String[] serieCorrelativo = trxVenta.Sopnumbe.Split(new char[] { '-' });

                        if (maquina.ValidaTransicion(claseDocumento, accion, trxVenta.EstadoActual))
                            if (trxVenta.Voidstts == 0 && trxVenta.EstadoContabilizado.Equals("contabilizado"))  //documento no anulado
                            {
                                string tipoDoc = string.Empty;
                                string serie = string.Empty;
                                string correlativo = string.Empty;

                                trxVenta.ArmarDocElectronico(string.Empty);
                                tipoDoc = trxVenta.DocGP.DocVenta.tipoDocumento;
                                serie = serieCorrelativo[0];
                                correlativo = serieCorrelativo[1];

                                resultadoSunat = await servicioTimbre.ConsultaStatusAlOSEAsync(trxVenta.DocGP.DocVenta.consecutivoDocumento, trxVenta.Ruta_certificadoPac, trxVenta.Contrasenia_clavePac, tipoDoc, serie, correlativo);
                                String[] codigoYMensaje = resultadoSunat.Split(new char[] { '-' });
                                maquina.DestinoAceptado = codigoYMensaje[0] == "0" ? true : false;
                                maquina.ActualizarNodoDestinoStatusBase();
                                DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, codigoYMensaje[1], codigoYMensaje[0], _Conex.Usuario, accion, maquina.DestinoStatusBase, maquina.DestinoEBinario, accion + ":" + codigoYMensaje[0]);

                                if (codigoYMensaje[0].Equals("0") || int.Parse(codigoYMensaje[0])>1000)
                                {
                                    DocVenta.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, "emitido", "emitido", maquina.DestinoEBinario, maquina.DestinoMensaje, codigoYMensaje[0]);
                                }
                                msj = "Mensaje del OCE: " + resultadoSunat;

                            }
                    }
                    catch (Exception lo)
                    {
                        string imsj = lo.InnerException == null ? "" : lo.InnerException.ToString();
                        msj = lo.Message + " " + imsj + Environment.NewLine + lo.StackTrace;
                        errores++;
                    }
                    finally
                    {
                        OnProgreso(i * 100 / trxVenta.RowCount, "Doc:" + trxVenta.Sopnumbe + " " + msj.Trim() + " " + maquina.ultimoMensaje + Environment.NewLine);              //Notifica al suscriptor
                        i++;
                    }
                } while (trxVenta.MoveNext() && errores < 10);
            }
            catch (Exception xw)
            {
                string imsj = xw.InnerException == null ? "" : xw.InnerException.ToString();
                this.ultimoMensaje = xw.Message + " " + imsj + Environment.NewLine + xw.StackTrace;
            }
            finally
            {
                OnProgreso(100, ultimoMensaje);
            }
            OnProgreso(100, "PROCESO FINALIZADO!");
            return resultadoSunat;
        }

        private Tuple<bool, string, string> ResultadoCDR(string tipoDocumento, string cdr)
        {
            var xmlCdr = XElement.Parse(cdr);
            XNamespace nscac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
            XNamespace nscbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
            var responseCode = xmlCdr?.Elements(nscac + "DocumentResponse")?.Elements(nscac + "Response")?.Elements(nscbc + "ResponseCode")?.First().Value;
            var description = xmlCdr?.Elements(nscac + "DocumentResponse")?.Elements(nscac + "Response")?.Elements(nscbc + "Description")?.First().Value;
            //var refId = xmlCdr?.Elements(nscac + "DocumentResponse")?.Elements(nscac + "Response")?.Elements(nscbc + "ReferenceID")?.First().Value;

            if (string.IsNullOrEmpty(responseCode))
            {
                throw new ArgumentException("El archivo de respuesta no corresponde a un cdr. ");
            }

            return Tuple.Create(responseCode.Equals("0"), responseCode, description);
        }

        public async Task ProcesaObtienePDFAsync(ICfdiMetodosWebService servicioTimbre)
        {
            try
            {
                String msj = String.Empty;
                String eBinario = String.Empty;
                trxVenta.Rewind();                                                          //move to first record

                int errores = 0;
                int i = 1;
                cfdReglasFacturaXml DocVenta = new cfdReglasFacturaXml(_Conex, _Param);     //log de facturas xml emitidas y anuladas
                ReglasME maquina = new ReglasME(_Param);
                String accion = "IMPRIME PDF";

                OnProgreso(1, "INICIANDO CONSULTA DE PDFs...");              //Notifica al suscriptor
                do
                {
                    msj = String.Empty;
                    String rutaNombrePDF = String.Empty;
                    String ticket = trxVenta.Regimen;
                    try
                    {
                        String[] serieCorrelativo = trxVenta.Sopnumbe.Split(new char[] { '-' });
                        string nombreArchivo = Utiles.FormatoNombreArchivo(trxVenta.Docid + trxVenta.Sopnumbe + "_" + trxVenta.s_CUSTNMBR, trxVenta.s_NombreCliente, 20) + "_CDR_" + accion.Substring(0, 2);

                        if (maquina.ValidaTransicion(_Param.tipoDoc, accion, trxVenta.EstadoActual))
                            if (trxVenta.Voidstts == 0 && trxVenta.EstadoContabilizado.Equals("contabilizado"))  //no anulado y contabilizado
                            {
                                trxVenta.ArmarDocElectronico(string.Empty);
                                rutaNombrePDF = await servicioTimbre.ObtienePDFdelOSEAsync(trxVenta.Rfc, trxVenta.Ruta_certificadoPac, trxVenta.Contrasenia_clavePac, trxVenta.DocGP.DocVenta.tipoDocumento, serieCorrelativo[0], serieCorrelativo[1], trxVenta.RutaXml.Trim(), nombreArchivo, ".pdf");
                                DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaNombrePDF, ticket, _Conex.Usuario, accion, maquina.DestinoStatusBase, maquina.DestinoEBinario, maquina.DestinoMensaje);

                                DocVenta.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, "emitido", "emitido", maquina.DestinoEBinario, maquina.DestinoMensaje, ticket);
                            }
                            else
                                msj = "No se puede generar porque no está Contabilizado o está Anulado.";

                    }
                    catch (ArgumentException ae)
                    {
                        msj = ae.Message + Environment.NewLine ;
                        //DocVenta.LogDocumento(trxVenta, msj, maquina, ticket, _Param.tipoDoc, accion, false, rutaNombrePDF);
                        //DocVenta.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, "emitido", "emitido", maquina.eBinActualConError, maquina.EnLetras(maquina.eBinActualConError, _Param.tipoDoc), ticket);
                        errores++;
                    }
                    catch (IOException io)
                    {
                        msj = "Excepción al revisar la carpeta/archivo: " + trxVenta.Ruta_clave + " Verifique su existencia y privilegios." + Environment.NewLine + io.Message + Environment.NewLine;
                        errores++;
                    }
                    catch (Exception lo)
                    {
                        string imsj = lo.InnerException == null ? "" : lo.InnerException.ToString();
                        msj = lo.Message + " " + imsj + Environment.NewLine + lo.StackTrace;
                        errores++;
                    }
                    finally
                    {
                        OnProgreso(i * 100 / trxVenta.RowCount, "Doc:" + trxVenta.Sopnumbe + " " + msj.Trim() + " " + maquina.ultimoMensaje + Environment.NewLine);              //Notifica al suscriptor
                        i++;
                    }
                } while (trxVenta.MoveNext() && errores < 10);
            }
            catch (Exception xw)
            {
                string imsj = xw.InnerException == null ? "" : xw.InnerException.ToString();
                this.ultimoMensaje = xw.Message + " " + imsj + Environment.NewLine + xw.StackTrace;
            }
            finally
            {
                OnProgreso(100, ultimoMensaje);
            }
            OnProgreso(100, "PROCESO FINALIZADO!");
        }

        public async Task ProcesaBajaComprobanteAsync(String motivoBaja, ICfdiMetodosWebService servicioTimbre)
        {
            try
            {
                String msj = String.Empty;
                String eBinario = String.Empty;
                trxVenta.Rewind();                                                          //move to first record

                int errores = 0; int i = 1;
                cfdReglasFacturaXml DocVenta = new cfdReglasFacturaXml(_Conex, _Param);     //log de facturas xml emitidas y anuladas
                ReglasME maquina = new ReglasME(_Param);

                OnProgreso(1, "INICIANDO BAJA DE DOCUMENTO...");              //Notifica al suscriptor
                do
                {
                    msj = String.Empty;
                    try
                    {
                        String accion = "BAJA";
                        if (maquina.ValidaTransicion(_Param.tipoDoc, accion, trxVenta.EstadoActual))
                        {
                            eBinario = maquina.eBinarioNuevo;

                            trxVenta.ArmarBaja(motivoBaja);
                            String[] serieCorrelativo = trxVenta.Sopnumbe.Split(new char[] { '-' });
                            string numeroSunat = serieCorrelativo[0] + "-" + serieCorrelativo[1];
                            //string numeroSunat = serieCorrelativo[0] + "-" + long.Parse(serieCorrelativo[1]).ToString();

                            //validaciones
                            switch (trxVenta.DocGP.DocVenta.tipoDocumento)
                            {
                                case "01":
                                    if (!trxVenta.Sopnumbe.Substring(0, 1).Equals("F"))
                                    {
                                        msj = "El folio de la Factura debe empezar con la letra F. ";
                                        throw new ApplicationException(msj);
                                    }
                                    break;
                                case "03":
                                    if (!trxVenta.Sopnumbe.Substring(0, 1).Equals("B"))
                                    {
                                        msj = "El folio de la Boleta debe empezar con la letra B. ";
                                        throw new ApplicationException(msj);
                                    }
                                    break;
                                default:
                                    msj = "ok";
                                    break;
                            }
                            string nombreArchivo = Utiles.FormatoNombreArchivo(trxVenta.Docid + trxVenta.Sopnumbe + "_" + trxVenta.s_CUSTNMBR, trxVenta.s_NombreCliente, 20) + "_" + accion.Substring(0, 4);

                            string resultadoBaja = await servicioTimbre.SolicitarBajaAsync(trxVenta.DocGP.DocVenta.cliente_numeroDocumento, trxVenta.Ruta_certificadoPac, trxVenta.Contrasenia_clavePac, string.Concat(trxVenta.DocGP.DocVenta.tipoDocumento, "-", numeroSunat), Utiles.Izquierda(motivoBaja, 100));

                            DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, resultadoBaja, "baja ok", _Conex.Usuario, string.Empty, maquina.DestinoStatusBase, maquina.DestinoEBinario, maquina.DestinoMensaje);

                            DocVenta.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, "emitido", "emitido", maquina.DestinoEBinario, maquina.DestinoMensaje, "baja ok");

                        }
                    }
                    catch (HttpRequestException he)
                    {
                        msj = string.Concat(he.Message, Environment.NewLine, he.StackTrace);
                        errores++;
                    }
                    catch (ApplicationException ae)
                    {
                        msj = ae.Message + Environment.NewLine + ae.StackTrace;
                        errores++;
                    }
                    catch (IOException io)
                    {
                        msj = "Excepción al revisar la carpeta/archivo: " + trxVenta.Ruta_clave + " Verifique su existencia y privilegios." + Environment.NewLine + io.Message + Environment.NewLine;
                        errores++;
                    }
                    catch (Exception lo)
                    {
                        string imsj = lo.InnerException == null ? "" : lo.InnerException.ToString();
                        msj = lo.Message + " " + imsj + Environment.NewLine + lo.StackTrace;
                        errores++;
                    }
                    finally
                    {
                        OnProgreso(i * 100 / trxVenta.RowCount, "Doc:" + trxVenta.Sopnumbe + " " + msj.Trim() + " " + maquina.ultimoMensaje + Environment.NewLine);              //Notifica al suscriptor
                        i++;
                    }
                } while (trxVenta.MoveNext() && errores < 10 && i < 2); //Dar de baja uno por uno
            }
            catch (Exception xw)
            {
                string imsj = xw.InnerException == null ? "" : xw.InnerException.ToString();
                this.ultimoMensaje = xw.Message + " " + imsj + Environment.NewLine + xw.StackTrace;
            }
            finally
            {
                OnProgreso(100, ultimoMensaje);
            }
            OnProgreso(100, "Proceso finalizado!");
        }


    }
}

