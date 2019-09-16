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

        private string ultimoMensaje = "";
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
            string rutaYNombreArchivo = string.Empty;
            try
            {
                String msj = String.Empty;
                trxVenta.Rewind(); //move to first record
                string leyendas = await vwCfdTransaccionesDeVenta.ObtieneLeyendasAsync();
                int errores = 0;
                int i = 0;
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

                            rutaYNombreArchivo = await EjecutaEventoEmiteAsync(servicioTimbre, LogComprobante, nombreArchivo, extension, 1);

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
                    catch (XmlException xm)
                    {
                        msj = "Verifique la configuración de leyendas para la impresión PDF. [GeneraDocumentoXmlAsync] " + xm.Message + Environment.NewLine + xm.StackTrace;
                        LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, "GeneraDocumentoXmlAsync " + xm.Message, "errLeyendas", _Conex.Usuario, string.Empty, Maquina.estadoBaseError, trxVenta.CicloDeVida.binStatus, xm.StackTrace);
                        errores++;
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

        private async Task<string> EjecutaEventoEmiteAsync(ICfdiMetodosWebService servicioTimbre, cfdReglasFacturaXml LogComprobante, string nombreArchivo, string extension, int usuarioConAcceso)
        {
            string xmlFactura = string.Empty;
            string rutaYNombreArchivo = string.Empty;
            try
            {
                xmlFactura = await servicioTimbre.TimbraYEnviaServicioDeImpuestoAsync(trxVenta.DocGP.DocVenta.cliente_numeroIdentificacion, trxVenta.Ruta_certificadoPac, trxVenta.Ruta_clavePac, trxVenta.DocGP);

                LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaYNombreArchivo, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _Conex.Usuario, xmlFactura.Replace("encoding=\"utf-8\"", "").Replace("encoding=\"iso-8859-1\"", ""),
                                                    trxVenta.CicloDeVida.targetSingleStatus, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));

                if (!string.IsNullOrEmpty(xmlFactura))
                    rutaYNombreArchivo = await LogComprobante.GuardaArchivoAsync(trxVenta, xmlFactura, nombreArchivo, extension, false);

                EjecutaEventoServicioImpuestosAcepta(servicioTimbre, LogComprobante, rutaYNombreArchivo, nombreArchivo, usuarioConAcceso);

                rutaYNombreArchivo = await EjecutaEventoObtienePDFAsync(servicioTimbre, LogComprobante, nombreArchivo, usuarioConAcceso);

                return (rutaYNombreArchivo);
            }
            catch (Exception lo)
            {
                string msj = "[EjecutaEventoEmiteAsync] " + lo.Message + Environment.NewLine + lo.StackTrace;
                String[] mensajeWs = lo.Message.Split(new char[] { '-' });
                switch (mensajeWs[0])
                {
                    case "Z99": //Caso de error interno de Ws
                        xmlFactura = await servicioTimbre.ObtieneXMLdelOSEAsync(trxVenta.DocGP.DocVenta.cliente_numeroIdentificacion, trxVenta.Ruta_certificadoPac, trxVenta.Ruta_clavePac, trxVenta.DocGP.DocVenta.tipoDocumento, trxVenta.DocGP.DocVenta.prefijo, trxVenta.DocGP.DocVenta.consecutivoDocumento);

                        LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaYNombreArchivo, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _Conex.Usuario, xmlFactura.Replace("encoding=\"utf-8\"", "").Replace("encoding=\"iso-8859-1\"", ""),
                                                            trxVenta.CicloDeVida.targetSingleStatus, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));

                        if (!string.IsNullOrEmpty(xmlFactura))
                            rutaYNombreArchivo = await LogComprobante.GuardaArchivoAsync(trxVenta, xmlFactura, nombreArchivo, extension, false);

                        EjecutaEventoServicioImpuestosAcepta(servicioTimbre, LogComprobante, rutaYNombreArchivo, nombreArchivo, usuarioConAcceso);

                        rutaYNombreArchivo = await EjecutaEventoObtienePDFAsync(servicioTimbre, LogComprobante, nombreArchivo, usuarioConAcceso);
                        break;
                    case "Z98": //Rechazo de la DIAN.
                        LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaYNombreArchivo, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _Conex.Usuario, string.Empty,
                                                               trxVenta.CicloDeVida.targetSingleStatus, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));
                        EjecutaEventoServicioImpuestosRechaza(LogComprobante);
                        break;
                    default:
                        LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, "EjecutaEventoEmiteAsync " + lo.Message, "errTheFactory", _Conex.Usuario, string.Empty, Maquina.estadoBaseError, trxVenta.CicloDeVida.binStatus, lo.StackTrace);
                        throw new InvalidOperationException(msj);
                }
                return (rutaYNombreArchivo);
            }

        }

        private void EjecutaEventoServicioImpuestosRechaza(cfdReglasFacturaXml LogComprobante)
        {
            if (trxVenta.CicloDeVida.Transiciona(Maquina.eventoDIANRechaza, 1))
            {
                LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, string.Empty, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _Conex.Usuario, string.Empty,
                                                    trxVenta.CicloDeVida.targetSingleStatus, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));
                LogComprobante.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, Maquina.estadoBaseEmisor, Maquina.estadoBaseEmisor, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus), trxVenta.CicloDeVida.idxTargetSingleStatus.ToString());
            }
        }

        private void EjecutaEventoServicioImpuestosAcepta(ICfdiMetodosWebService servicioTimbre, cfdReglasFacturaXml LogComprobante, string rutaYNombreArchivo, string nombreArchivo, int usuarioConAcceso)
        {
            if (trxVenta.CicloDeVida.Transiciona(Maquina.eventoDIANAcepta, usuarioConAcceso))
            {
                LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaYNombreArchivo, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _Conex.Usuario, string.Empty,
                                                    trxVenta.CicloDeVida.targetSingleStatus, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));
                LogComprobante.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, Maquina.estadoBaseEmisor, Maquina.estadoBaseEmisor, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus), trxVenta.CicloDeVida.idxTargetSingleStatus.ToString());
            }
        }

        private async Task<string> EjecutaEventoObtienePDFAsync(ICfdiMetodosWebService servicioTimbre, cfdReglasFacturaXml LogComprobante, string nombreArchivo, int usuarioConAcceso)
        {
            string rutaYNombreArchivo = string.Empty;
            if (trxVenta.CicloDeVida.Transiciona(Maquina.eventoObtienePDF, usuarioConAcceso))
            {
                rutaYNombreArchivo = await servicioTimbre.ObtienePDFdelOSEAsync(trxVenta.DocGP.DocVenta.cliente_numeroIdentificacion, trxVenta.Ruta_certificadoPac, trxVenta.Ruta_clavePac, trxVenta.DocGP.DocVenta.tipoDocumento, trxVenta.DocGP.DocVenta.prefijo, trxVenta.DocGP.DocVenta.consecutivoDocumento, trxVenta.RutaXml.Trim(), nombreArchivo, ".pdf");
                LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaYNombreArchivo, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _Conex.Usuario, string.Empty,
                                                    trxVenta.CicloDeVida.targetSingleStatus, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));
                LogComprobante.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, Maquina.estadoBaseEmisor, Maquina.estadoBaseEmisor, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus), trxVenta.CicloDeVida.idxTargetSingleStatus.ToString());
            }
            return rutaYNombreArchivo;
        }

        private void EjecutaEvento(int Evento, ICfdiMetodosWebService servicioTimbre, cfdReglasFacturaXml LogComprobante, int usuarioConAcceso)
        {
            if (trxVenta.CicloDeVida.Transiciona(Evento, usuarioConAcceso))
            {
                LogComprobante.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, string.Empty, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _Conex.Usuario, string.Empty,
                                                    trxVenta.CicloDeVida.targetSingleStatus, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));
                LogComprobante.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, Maquina.estadoBaseEmisor, Maquina.estadoBaseEmisor, trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus), trxVenta.CicloDeVida.idxTargetSingleStatus.ToString());
            }
        }

        public async Task<string> ProcesaConsultaStatusAsync(ICfdiMetodosWebService servicioTimbre)
        {
            string statusActual = string.Empty;
            try
            {
                String msj = String.Empty;
                trxVenta.Rewind();                                                          //move to first record

                int errores = 0;
                int i = 0;
                cfdReglasFacturaXml LogComprobante = new cfdReglasFacturaXml(_Conex, _Param);     //log de facturas xml emitidas y anuladas
                string tipoMEstados = "DOCVENTA-" + trxVenta.EstadoContabilizado;
                trxVenta.CicloDeVida = new Maquina(trxVenta.EstadoActual, trxVenta.Regimen, trxVenta.Voidstts, "emisor", tipoMEstados);

                OnProgreso(1, "INICIANDO CONSULTA DE STATUS...");              //Notifica al suscriptor
                do
                {
                    msj = String.Empty;
                    try
                    {
                        trxVenta.ArmarDocElectronico(string.Empty);
                        statusActual = await servicioTimbre.ConsultaStatusAlOSEAsync(trxVenta.DocGP.DocVenta.consecutivoDocumento, trxVenta.Ruta_certificadoPac, trxVenta.Ruta_clavePac, trxVenta.DocGP.DocVenta.tipoDocumento, trxVenta.DocGP.DocVenta.prefijo, trxVenta.DocGP.DocVenta.consecutivoDocumento);
                        String[] codigoYMensaje = statusActual.Split(new char[] { '-' });
                        int evento = Maquina.eventoNoHaceNada;
                        //Determina evento en base al resultado del ws
                        switch (codigoYMensaje[0])
                        {
                            case "z01":
                                evento = Maquina.eventoAcuseAceptado;
                                break;
                            case "z02":
                                evento = Maquina.eventoAcuseRechazado;
                                break;
                            default:
                                evento = Maquina.eventoNoHaceNada;
                                break;
                        }
                        EjecutaEvento(evento, servicioTimbre, LogComprobante, 1);

                        msj = "Status: " + statusActual;
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
            return statusActual;
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
                        trxVenta.ArmarDocElectronico(string.Empty);

                        EjecutaEventoServicioImpuestosAcepta(servicioTimbre, LogComprobante, string.Empty, nombreArchivo, 1);

                        rutaNombrePDF = await EjecutaEventoObtienePDFAsync(servicioTimbre, LogComprobante, nombreArchivo, 1);

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

