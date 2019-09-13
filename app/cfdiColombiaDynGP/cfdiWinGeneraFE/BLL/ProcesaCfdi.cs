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
                trxVenta.Rewind(); //move to first record
                string leyendas = await vwCfdTransaccionesDeVenta.ObtieneLeyendasAsync();
                int errores = 0;
                int i = 1;
                cfdReglasFacturaXml LogComprobante = new cfdReglasFacturaXml(_Conex, _Param);     //log de facturas xml emitidas y anuladas
                string tipoMEstados = "DOCVENTA-" + trxVenta.EstadoContabilizado;
                trxVenta.CicloDeVida = new Maquina(trxVenta.EstadoActual, trxVenta.Regimen, trxVenta.Voidstts, "emisor", tipoMEstados);

                OnProgreso(1, "INICIANDO EMISION DE COMPROBANTES DE VENTA...");
                do
                {
                    msj = String.Empty;
                    try
                    {
                        if (trxVenta.CicloDeVida.Transiciona(Maquina.eventoGeneraYEnviaXml, 1))
                        {
                            trxVenta.ArmarDocElectronico(leyendas);
                            string nombreArchivo = Utiles.FormatoNombreArchivo(trxVenta.Sopnumbe + "_" + trxVenta.s_CUSTNMBR, trxVenta.s_NombreCliente, 20) + "_" + Maquina.eventoGeneraYEnviaXml.ToString();
                            msj = ValidaDatosComprobante();

                            string extension = ".xml";
                            rutaYNom = Path.Combine(trxVenta.RutaXml.Trim(), nombreArchivo + extension);
                            try
                            {
                                xmlFactura = await servicioTimbre.TimbraYEnviaServicioDeImpuestoAsync(trxVenta.DocGP.DocVenta.cliente_numeroIdentificacion, trxVenta.Ruta_certificadoPac, trxVenta.Ruta_clavePac, trxVenta.DocGP);
                                rutaYNom = await LogEmiteAceptaImprimeAsync(servicioTimbre, xmlFactura, rutaYNom, LogComprobante, nombreArchivo, extension);

                            }
                            catch (XmlException xm)
                            {
                                msj = "Verifique la configuración de leyendas para la impresión PDF. [GeneraDocumentoXmlAsync] " + xm.Message + Environment.NewLine + xm.StackTrace;
                                LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, "GeneraDocumentoXmlAsync " + xm.Message, "errLeyendas", _Conex.Usuario, string.Empty, Maquina.estadoBaseError, trxVenta.CicloDeVida.binStatus, xm.StackTrace);
                                errores++;
                            }
                            catch (Exception lo)
                            {
                                msj = "[GeneraDocumentoXmlAsync] " + lo.Message + Environment.NewLine + lo.StackTrace;
                                String[] mensajeWs = lo.Message.Split(new char[] { '-' });
                                switch (mensajeWs[0])
                                {
                                    case "Z99": //Caso de error interno de Ws
                                        xmlFactura = await servicioTimbre.ObtieneXMLdelOSEAsync(trxVenta.DocGP.DocVenta.cliente_numeroIdentificacion, trxVenta.Ruta_certificadoPac, trxVenta.Ruta_clavePac, trxVenta.DocGP.DocVenta.tipoDocumento, trxVenta.DocGP.DocVenta.prefijo, trxVenta.DocGP.DocVenta.consecutivoDocumento);
                                        rutaYNom = await LogEmiteAceptaImprimeAsync(servicioTimbre, xmlFactura, rutaYNom, LogComprobante, nombreArchivo, extension);
                                        break;
                                    case "Z98": //Rechazo de la DIAN.
                                        LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaYNom, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _Conex.Usuario, string.Empty,
                                                                               trxVenta.CicloDeVida.targetSingleStatus, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));
                                        if (trxVenta.CicloDeVida.Transiciona(Maquina.eventoDIANRechaza, 1))
                                        {
                                            LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaYNom, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _Conex.Usuario, string.Empty,
                                                                                trxVenta.CicloDeVida.targetSingleStatus, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));
                                            LogComprobante.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, Maquina.estadoBaseEmisor, Maquina.estadoBaseEmisor, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus), trxVenta.CicloDeVida.idxTargetSingleStatus.ToString());
                                        }
                                        break;
                                    default:
                                        LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, "GeneraDocumentoXmlAsync " + lo.Message, "errDFacture", _Conex.Usuario, string.Empty, Maquina.estadoBaseError, trxVenta.CicloDeVida.binStatus, lo.StackTrace);
                                        break;
                                }
                                errores++;
                            }
                        }
                        //if (trxVenta.Voidstts == 1)  //documento anulado
                        //{
                        //     //si el documento está anulado en gp, agregar al log como emitido
                        //        maquina.ValidaTransicion("FACTURA", "ANULA VENTA", trxVenta.EstadoActual, "emitido");
                        //        msj = "Anulado en GP y marcado como emitido.";
                        //        OnProgreso(1, msj);
                        //        LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, "Anulado en GP", "0", _Conex.Usuario, "", "emitido", maquina.eBinarioNuevo, msj.Trim());
                        //}
                    }
                    catch (DirectoryNotFoundException dnf)
                    {
                        msj = "El comprobante fue emitido, pero no se pudo guardar el archivo en: " + trxVenta.Ruta_clave + " Verifique si existe la carpeta." + Environment.NewLine;
                        LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, msj, "errCarpeta", _Conex.Usuario, string.Empty, Maquina.estadoBaseError, trxVenta.CicloDeVida.binStatus, dnf.Message);
                        msj += dnf.Message + Environment.NewLine;
                        errores++;
                    }
                    catch (IOException io)
                    {
                        msj = "El comprobante fue emitido, pero no se pudo guardar el archivo en: " + trxVenta.Ruta_clave + " Verifique permisos a la carpeta." + Environment.NewLine;
                        LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, msj, "errIO", _Conex.Usuario, string.Empty, Maquina.estadoBaseError, trxVenta.CicloDeVida.binStatus, io.Message);
                        msj += io.Message + Environment.NewLine;
                        errores++;
                    }
                    catch (Exception lo)
                    {
                        string imsj = lo.InnerException == null ? "" : lo.InnerException.ToString();
                        msj = lo.Message + " " + imsj + Environment.NewLine + lo.StackTrace;
                        LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, lo.Message, "errDesconocido", _Conex.Usuario, string.Empty, Maquina.estadoBaseError, trxVenta.CicloDeVida.binStatus, lo.StackTrace);
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

        private string ValidaDatosComprobante()
        {
            string msj;
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

            return msj;
        }

        private async Task<string> LogEmiteAceptaImprimeAsync(ICfdiMetodosWebService servicioTimbre, string xmlFactura, string rutaYNomXml, cfdReglasFacturaXml LogComprobante, string nombreArchivo, string extension)
        {
            LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaYNomXml, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _Conex.Usuario, xmlFactura.Replace("encoding=\"utf-8\"", "").Replace("encoding=\"iso-8859-1\"", ""),
                                                    trxVenta.CicloDeVida.targetSingleStatus, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));

            if (!string.IsNullOrEmpty(xmlFactura))
                rutaYNomXml = await LogComprobante.GuardaArchivoAsync(trxVenta, xmlFactura, nombreArchivo, extension, false);

            string rutaYNomPdf = await LogAceptaYObtienePdfAsync(servicioTimbre, rutaYNomXml, LogComprobante, nombreArchivo);

            return rutaYNomXml;
        }

        private async Task<string> LogAceptaYObtienePdfAsync(ICfdiMetodosWebService servicioTimbre, string rutaYNom, cfdReglasFacturaXml LogComprobante, string nombreArchivo)
        {
            string rutaYNomPdf = string.Empty;
            if (trxVenta.CicloDeVida.Transiciona(Maquina.eventoDIANAcepta, 1))
            {
                LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaYNom, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _Conex.Usuario, string.Empty,
                                                    trxVenta.CicloDeVida.targetSingleStatus, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));
                LogComprobante.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, Maquina.estadoBaseEmisor, Maquina.estadoBaseEmisor, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus), trxVenta.CicloDeVida.idxTargetSingleStatus.ToString());
            }

            if (trxVenta.CicloDeVida.Transiciona(Maquina.eventoObtienePDF, 1))
            {
                rutaYNomPdf = await servicioTimbre.ObtienePDFdelOSEAsync(trxVenta.DocGP.DocVenta.cliente_numeroIdentificacion, trxVenta.Ruta_certificadoPac, trxVenta.Ruta_clavePac, trxVenta.DocGP.DocVenta.tipoDocumento, trxVenta.DocGP.DocVenta.prefijo, trxVenta.DocGP.DocVenta.consecutivoDocumento, trxVenta.RutaXml.Trim(), nombreArchivo, ".pdf");
                LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaYNomPdf, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _Conex.Usuario, string.Empty,
                                                    trxVenta.CicloDeVida.targetSingleStatus, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));
                LogComprobante.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, Maquina.estadoBaseEmisor, Maquina.estadoBaseEmisor, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus), trxVenta.CicloDeVida.idxTargetSingleStatus.ToString());
            }
            return rutaYNomPdf;
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
                string tipoMEstados = "DOCVENTA-" + trxVenta.EstadoContabilizado;
                trxVenta.CicloDeVida = new Maquina(trxVenta.EstadoActual, trxVenta.Regimen, trxVenta.Voidstts, "emisor", tipoMEstados);

                String accion = "CONSULTA STATUS";

                OnProgreso(1, "INICIANDO CONSULTA DE STATUS...");              //Notifica al suscriptor
                do
                {
                    msj = String.Empty;
                    String claseDocumento = !trxVenta.Docid.Equals("RESUMEN") ? _Param.tipoDoc : trxVenta.Docid;
                    try
                    {
                        String[] serieCorrelativo = trxVenta.Sopnumbe.Split(new char[] { '-' });

                        //Consulta ws
                        string tipoDoc = string.Empty;
                        string serie = string.Empty;
                        string correlativo = string.Empty;

                        trxVenta.ArmarDocElectronico(string.Empty);
                        tipoDoc = trxVenta.DocGP.DocVenta.tipoDocumento;
                        serie = serieCorrelativo[0];
                        correlativo = serieCorrelativo[1];
                        resultadoSunat = await servicioTimbre.ConsultaStatusAlOSEAsync(trxVenta.DocGP.DocVenta.consecutivoDocumento, trxVenta.Ruta_certificadoPac, trxVenta.Contrasenia_clavePac, tipoDoc, serie, correlativo);
                        //Determina evento en base al resultado del ws
                        int evento = Maquina.eventoAcuseAceptado;

                        if (trxVenta.CicloDeVida.Transiciona(evento, 1))
                        {
                            String[] codigoYMensaje = resultadoSunat.Split(new char[] { '-' });
                            //maquina.DestinoAceptado = codigoYMensaje[0] == "0" ? true : false;
                            //maquina.ActualizarNodoDestinoStatusBase();
                            //DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, codigoYMensaje[1], codigoYMensaje[0], _Conex.Usuario, accion, maquina.DestinoStatusBase, maquina.DestinoEBinario, accion + ":" + codigoYMensaje[0]);

                            if (codigoYMensaje[0].Equals("0") || int.Parse(codigoYMensaje[0]) > 1000)
                            {
                                //DocVenta.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, "emitido", "emitido", maquina.DestinoEBinario, maquina.DestinoMensaje, codigoYMensaje[0]);
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
                        //OnProgreso(i * 100 / trxVenta.RowCount, "Doc:" + trxVenta.Sopnumbe + " " + msj.Trim() + " " + maquina.ultimoMensaje + Environment.NewLine);              //Notifica al suscriptor
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
                cfdReglasFacturaXml LogComprobante = new cfdReglasFacturaXml(_Conex, _Param);     //log de facturas xml emitidas y anuladas
                string tipoMEstados = "DOCVENTA-" + trxVenta.EstadoContabilizado;
                trxVenta.CicloDeVida = new Maquina(trxVenta.EstadoActual, trxVenta.Regimen, trxVenta.Voidstts, "emisor", tipoMEstados);

                OnProgreso(1, "INICIANDO CONSULTA DE PDFs...");              //Notifica al suscriptor
                do
                {
                    msj = String.Empty;
                    String rutaNombrePDF = String.Empty;
                    try
                    {
                        string nombreArchivo = Utiles.FormatoNombreArchivo(trxVenta.Sopnumbe + "_" + trxVenta.s_CUSTNMBR, trxVenta.s_NombreCliente, 20)+ "_" + Maquina.eventoObtienePDF.ToString();
                        string rutaYNom = Path.Combine(trxVenta.RutaXml.Trim(), nombreArchivo);
                        trxVenta.ArmarDocElectronico(string.Empty);

                        rutaNombrePDF = await LogAceptaYObtienePdfAsync(servicioTimbre, rutaYNom, LogComprobante, nombreArchivo);

                    }
                    catch (IOException io)
                    {
                        msj = "Excepción al verificar acceso a la carpeta/archivo: " + trxVenta.Ruta_clave + " Verifique su existencia y privilegios." + Environment.NewLine + io.Message + Environment.NewLine;
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
                string tipoMEstados = "DOCVENTA-" + trxVenta.EstadoContabilizado;
                trxVenta.CicloDeVida = new Maquina(trxVenta.EstadoActual, trxVenta.Regimen, trxVenta.Voidstts, "emisor", tipoMEstados);

                OnProgreso(1, "INICIANDO BAJA DE DOCUMENTO...");              //Notifica al suscriptor
                do
                {
                    msj = String.Empty;
                    try
                    {
                        String accion = "BAJA";
                        if (trxVenta.CicloDeVida.Transiciona(Maquina.eventoDarDeBaja, 1))
                        {
                            //eBinario = maquina.eBinarioNuevo;

                            trxVenta.ArmarBaja(motivoBaja);
                            String[] serieCorrelativo = trxVenta.Sopnumbe.Split(new char[] { '-' });
                            string numeroSunat = serieCorrelativo[0] + "-" + serieCorrelativo[1];

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

                            //DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, resultadoBaja, "baja ok", _Conex.Usuario, string.Empty, maquina.DestinoStatusBase, maquina.DestinoEBinario, maquina.DestinoMensaje);

                            //DocVenta.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, "emitido", "emitido", maquina.DestinoEBinario, maquina.DestinoMensaje, "baja ok");

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
                        //OnProgreso(i * 100 / trxVenta.RowCount, "Doc:" + trxVenta.Sopnumbe + " " + msj.Trim() + " " + maquina.ultimoMensaje + Environment.NewLine);              //Notifica al suscriptor
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

