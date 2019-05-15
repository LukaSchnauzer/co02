using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using System.IO;
using DemoGenFactura_int_tfhka_fel.ServiceDemoAdjuntos;
using DemoGenFactura_int_tfhka_fel.ServiceDemoEnviarArchivo;

namespace DemoGenFactura_int_tfhka_fel
{
    public partial class Form1 : Form
    {
        public string tokenEmpresa = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";// los accesos deben ser solicitados al iniciar el proceso de integración
        public string tokenAuthorizacion = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";// los accesos deben ser solicitados al iniciar el proceso de integración
        FacturaGeneral factura;
        ServiceDemoAdjuntos.ServiceClient serviceClient;
        ServiceDemoEnviarArchivo.ServiceClient serviceArchivos;
        DocumentResponse docRespuesta;
        FileUploadResponse fileRespuesta;
        uploadAttachment uploadAttachment;
        private byte[] anexByte;

        public Form1()
        {
            InitializeComponent();
            configurarServicios();
            activarEdicion(false);          
            this.tbxFechaEmision.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.tbxFechaVencimiento.Text = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void activarEdicion(bool pactivar)
        {
            // datos cliente 
            this.tbxDepartamento.Enabled = pactivar;
            this.tbxCiudad.Enabled = pactivar;
            this.tbxDireccion.Enabled = pactivar;
            this.tbxApellido.Enabled = pactivar;
            //this.tbxEmail.Enabled = pactivar;
            this.tbxNomRazon.Enabled = pactivar;
            this.tbxNumeroIdentidad.Enabled = pactivar;
            this.tbxRegimen.Enabled = pactivar;
            this.tbxSegundoNom.Enabled = pactivar;
            this.tbxTelefono.Enabled = pactivar;
            this.tbxTipoIdentificacion.Enabled = pactivar;
            this.tbxTipoPersona.Enabled = pactivar;

            // datos factura
            this.tbxFechaEmision.Enabled = pactivar;
            this.tbxFechaVencimiento.Enabled = pactivar;
            this.tbxTipoPago.Enabled = pactivar;
            this.tbxMoneda.Enabled = pactivar;
            this.tbxTotalImpuestos.Enabled = pactivar;
            this.tbxImporteTotal.Enabled = pactivar;
            this.tbxBaseImponible.Enabled = pactivar; 
            // datos producto 
            this.tbxCodProducto.Enabled = pactivar;
            this.tbxDescripcion.Enabled = pactivar;
            this.tbxCantidadUnidades.Enabled = pactivar;
            this.tbxtipoUnidad.Enabled = pactivar;
            this.tbxPrecioUnitario.Enabled = pactivar;
            this.tbxTipoImpuesto.Enabled = pactivar;
            this.tbxPorcentaje.Enabled = pactivar; 

        }

        private FacturaGeneral buildFactura()
        {
            //armo el objeto factura
            FacturaGeneral facturaDemo = new FacturaGeneral();
            Cliente cliente = new Cliente();
            cliente.apellido = this.tbxApellido.Text; 
            cliente.departamento = this.tbxDepartamento.Text;
            cliente.ciudad = this.tbxCiudad.Text; 
            cliente.direccion = this.tbxDireccion.Text;
            cliente.email = this.tbxEmail.Text;
            cliente.nombreRazonSocial = this.tbxNomRazon.Text; 
            cliente.notificar = "SI";
            cliente.numeroDocumento = this.tbxNumeroIdentidad.Text; 
            cliente.pais = "CO";
            cliente.regimen = this.tbxRegimen.Text;
            cliente.segundoNombre = this.tbxSegundoNom.Text; 
            cliente.subDivision = "";
            cliente.telefono = this.tbxTelefono.Text;
            cliente.tipoIdentificacion = this.tbxTipoIdentificacion.Text;
            cliente.tipoPersona = this.tbxTipoPersona.Text;

            facturaDemo.cliente = cliente;
            facturaDemo.rangoNumeracion = "PREFIJO-DESDE"; // rango desde asignado
            facturaDemo.consecutivoDocumento = "CONSECUTIVO"; // Este valor debe aumentar cada vez que envie una factura 
            facturaDemo.detalleDeFactura = new FacturaDetalle[1];
            // detalle de la factura 
            FacturaDetalle detalle1 = new FacturaDetalle();

            detalle1.cantidadUnidades = this.tbxCantidadUnidades.Text;
            detalle1.descripcion = this.tbxDescripcion.Text;
            detalle1.codigoProducto = "001";
            detalle1.descuento = "0.00";
            detalle1.seriales = ""; 
            detalle1.impuestosDetalles = new FacturaImpuestos[1];
            // impuestos por producto 
            FacturaImpuestos impuestodetalles1 = new FacturaImpuestos();
            impuestodetalles1.baseImponibleTOTALImp = this.tbxBaseImponible.Text; 
            impuestodetalles1.codigoTOTALImp = this.tbxTipoImpuesto.Text;
            impuestodetalles1.controlInterno = "";
            impuestodetalles1.porcentajeTOTALImp = this.tbxPorcentaje.Text;
            impuestodetalles1.valorTOTALImp = this.tbxTotalImpuestos.Text;
            //fin impuestoprod 

            detalle1.impuestosDetalles[0] = impuestodetalles1;
            detalle1.precioTotal = this.tbxImporteTotal.Text;
            detalle1.precioTotalSinImpuestos = this.tbxBaseImponible.Text;
            detalle1.precioVentaUnitario = this.tbxPrecioUnitario.Text;
            detalle1.unidadMedida = this.tbxtipoUnidad.Text;
            detalle1.detalleAdicionalNombre = "";
            detalle1.detalleAdicionalValor = "";


            // fin detalle
            facturaDemo.detalleDeFactura[0] = detalle1;
            facturaDemo.informacionAdicional = "CAMPO DE INFORMACION ADICIONAL QUE SIRVE PARA AGREGAR INFORMACION REALCIONADA CON LA FACTURA QUE SE EMITE Y TIENE UNA CAPACIDAD AMPLIA";
            
            facturaDemo.estadoPago = "3"; //pagada totalmente 
            
            facturaDemo.fechaEmision = this.tbxFechaEmision.Text; //"2017-05-09 22:24:00";
            facturaDemo.fechaVencimiento = this.tbxFechaVencimiento.Text;
            facturaDemo.icoterms = "";
            facturaDemo.importeTotal = this.tbxImporteTotal.Text;
            facturaDemo.impuestosGenerales = new FacturaImpuestos[1];

            // factura impuestos generales 
            FacturaImpuestos impuestosg1 = new FacturaImpuestos();
            impuestosg1.baseImponibleTOTALImp = this.tbxBaseImponible.Text;
            impuestosg1.codigoTOTALImp = "01";
            impuestosg1.porcentajeTOTALImp = this.tbxPorcentaje.Text;
            impuestosg1.valorTOTALImp = this.tbxTotalImpuestos.Text;
            
            facturaDemo.impuestosGenerales[0] = impuestosg1;
            
            facturaDemo.medioPago = this.tbxTipoPago.Text;
            facturaDemo.moneda = this.tbxMoneda.Text;
            facturaDemo.propina = "0.00";
            facturaDemo.tipoDocumento = "01";
            facturaDemo.totalDescuentos = "0.00";
            facturaDemo.totalSinImpuestos = this.tbxBaseImponible.Text;

            facturaDemo.extras = new Extras[5]; //Uso de campos Extensibles
            Extras comentarioHeader = new Extras();
            comentarioHeader.nombre = "100100";
            comentarioHeader.valor = "MENSAJES PERSONALIZADO 1<br>MENSAJES PERSONALIZADO 2<br>MENSAJES PERSONALIZADO 3<br>MENSAJES PERSONALIZADO 4<br>MENSAJES PERSONALIZADO 5";
            comentarioHeader.xml = "1";
            comentarioHeader.pdf = "1";
            comentarioHeader.controlInterno1 = "encabezado";
            Extras comentarioFooter = new Extras();
            comentarioFooter.nombre = "100200";
            comentarioFooter.valor = "MENSAJES PERSONALIZADO 1<br>MENSAJES PERSONALIZADO 2<br>MENSAJES PERSONALIZADO 3<br>MENSAJES PERSONALIZADO 4<br>MENSAJES PERSONALIZADO 5<br>MENSAJES PERSONALIZADO 6";
            comentarioFooter.xml = "1";
            comentarioFooter.pdf = "1";
            comentarioFooter.controlInterno1 = "pie de pagina";
            Extras vendedor = new Extras();
            vendedor.nombre = "443";
            vendedor.valor = "Pablo Marmol";
            vendedor.xml = "1";
            vendedor.pdf = "1";
            vendedor.controlInterno1 = "";
            Extras pedido0 = new Extras();
            pedido0.nombre = "140";
            pedido0.valor = "PED";
            pedido0.xml = "1";
            pedido0.pdf = "1";
            pedido0.controlInterno1 = "";
            Extras pedido1 = new Extras();
            pedido1.nombre = "141";
            pedido1.valor = "01234";
            pedido1.xml = "1";
            pedido1.pdf = "1";
            pedido1.controlInterno1 = "";

            facturaDemo.extras[0] = comentarioHeader;
            facturaDemo.extras[1] = comentarioFooter;
            facturaDemo.extras[2] = vendedor;
            facturaDemo.extras[3] = pedido0;
            facturaDemo.extras[4] = pedido1;

            return facturaDemo;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            // habilito los controles 
            activarEdicion(true);
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        { 
            factura = buildFactura();
            rtxInformacion.Clear();
            this.Cursor = Cursors.WaitCursor;
            rtxInformacion.Text = "Envio de Factura:" + Environment.NewLine;
            int cantidadAnexos = leerAnexos(lbxAnexos.Items);

            //envio factura 
            if (cantidadAnexos < 1)
            {
                DialogResult dRes = MessageBox.Show("No hay anexos para enviar como adjuntos, ¿Desea continuar?", "No se encontraron archivos anexos", MessageBoxButtons.YesNo);
                if (dRes == DialogResult.Yes)
                {
                    docRespuesta = serviceClient.Enviar(tokenEmpresa, tokenAuthorizacion, factura, "0");

                    if (docRespuesta.codigo == 200)
                    {
                        rtxInformacion.Text += "Codigo: " + docRespuesta.codigo.ToString() + Environment.NewLine + 
                                               "Consecutivo Documento: " + docRespuesta.consecutivoDocumento + Environment.NewLine +
                                               "Cufe: " + docRespuesta.cufe + Environment.NewLine + 
                                               "Mensaje: " + docRespuesta.mensaje + Environment.NewLine + 
                                               "Resultado: " + docRespuesta.resultado;
                    }
                    else
                    {
                        rtxInformacion.Text += "Codigo: " + docRespuesta.codigo.ToString() + Environment.NewLine +
                                               "Mensaje: " + docRespuesta.mensaje + Environment.NewLine +
                                               "Resultado: " + docRespuesta.resultado;
                    }
                }
                else
                {
                    rtxInformacion.Text = "Proceso cancelado";
                }
            }
            else
            {
                docRespuesta = serviceClient.Enviar(tokenEmpresa, tokenAuthorizacion, factura, "1");

                if (docRespuesta.codigo == 200)
                {
                    rtxInformacion.Text += "Codigo: " + docRespuesta.codigo.ToString() + Environment.NewLine +
                                               "Consecutivo Documento: " + docRespuesta.consecutivoDocumento + Environment.NewLine +
                                               "Cufe: " + docRespuesta.cufe + Environment.NewLine +
                                               "Mensaje: " + docRespuesta.mensaje + Environment.NewLine +
                                               "Resultado: " + docRespuesta.resultado + Environment.NewLine + Environment.NewLine;

                    rtxInformacion.Text += "--------------------------------------------------" + Environment.NewLine;
                    rtxInformacion.Text += "Envio de adjuntos:" + Environment.NewLine;
                    int resultado = EnviarArchivosAdjuntos(cantidadAnexos, docRespuesta);
                    if (resultado > 0)
                    {
                        rtxInformacion.Text += resultado.ToString() + "PROCESO EXITOSO: Archivos adjuntos procesados correctamente!!!";
                    }
                    else
                    {
                        rtxInformacion.Text += Environment.NewLine + "ERROR: procesando archivos adjuntos!!!";
                    }
                }
                else
                {
                    rtxInformacion.Text += docRespuesta.codigo.ToString() + Environment.NewLine + docRespuesta.mensaje + Environment.NewLine + docRespuesta.resultado;
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void btnEstadoDocumento_Click(object sender, EventArgs e)
        {
            DocumentStatusResponse resp = serviceClient.EstadoDocumento(tokenEmpresa, tokenAuthorizacion, tbxEstadoDocumento.Text.Trim());
            MessageBox.Show(resp.codigo + Environment.NewLine + resp.estatusDocumento + Environment.NewLine + resp.mensaje, "Estado de Documento");
        }

        private void btnDescarga_Click(object sender, EventArgs e)
        {
            DownloadPDFResponse pdfResponse;
            DownloadXMLResponse xmlResponse;
            if (cbxTipoArchivo.Text.Equals("PDF"))
            {
                pdfResponse = serviceClient.DescargaPDF(tokenEmpresa, tokenAuthorizacion, tbxDescarga.Text.Trim());
                MessageBox.Show(pdfResponse.codigo + Environment.NewLine + pdfResponse.mensaje, "DEscarga de PDF");
                if (pdfResponse.codigo == 200)
                {
                    File.WriteAllBytes(tbxDescarga.Text.Trim() + ".pdf", Convert.FromBase64String(pdfResponse.documento));
                }
            }
            else
            {
                xmlResponse = serviceClient.DescargaXML(tokenEmpresa, tokenAuthorizacion, tbxDescarga.Text.Trim());
                MessageBox.Show(xmlResponse.codigo + Environment.NewLine + xmlResponse.mensaje, "Descarga de XML");
                if (xmlResponse.codigo == 200)
                {
                    File.WriteAllBytes(tbxDescarga.Text.Trim() + ".xml", Convert.FromBase64String(xmlResponse.documento));
                }
            }
        }

        private void btnFoliosRestantes_Click(object sender, EventArgs e)
        {
            rtxInformacion.Clear();
            FoliosRemainingResponse folios = serviceClient.FoliosRestantes(tokenEmpresa, tokenAuthorizacion);
            MessageBox.Show(folios.codigo + Environment.NewLine + folios.foliosRestantes + Environment.NewLine + folios.mensaje,"Folios Restantes");
        }

        private void btnEnvioCorreo_Click(object sender, EventArgs e)
        {
            SendEmailResponse eMail;
            eMail = serviceClient.EnvioCorreo(tokenEmpresa, tokenAuthorizacion, tbxDocCorreo.Text.Trim(), tbxEmailCorreo.Text.Trim());
            MessageBox.Show(eMail.codigo + Environment.NewLine + eMail.mensaje,"Reenvio de Correo");
        }



        private void btnAgregarArchivo_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                lbxAnexos.Items.Add(openFileDialog1.FileName);
            }
        }

        private void btnLimpiarArchivos_Click(object sender, EventArgs e)
        {
            lbxAnexos.Items.Clear();
        }

        private void configurarServicios()
        {
            serviceClient = new ServiceDemoAdjuntos.ServiceClient();
            serviceArchivos = new ServiceDemoEnviarArchivo.ServiceClient();
        }

        private int leerAnexos(ListBox.ObjectCollection items)
        {
            int cantidad = 0;
            foreach (string ruta in lbxAnexos.Items)
            {
                FileInfo file = new FileInfo(ruta);
                if (file.Exists)
                {
                    cantidad++;
                }
                else
                {
                    lbxAnexos.Items.RemoveAt(lbxAnexos.Items.IndexOf(ruta));
                }
            }

            return cantidad;
        }

        private int EnviarArchivosAdjuntos(int numeroDeArchivos, DocumentResponse docInfo)
        {
            int procesados = 0;
            for (int i = 0; i < numeroDeArchivos; i++)
            {
                FileInfo file = new FileInfo(lbxAnexos.Items[i].ToString());
                if (file.Exists)
                {
                    BinaryReader bReader = new BinaryReader(file.OpenRead());
                    anexByte = bReader.ReadBytes((int)file.Length);
                    //anexB64 = Convert.ToBase64String(anexByte);
                    uploadAttachment = new uploadAttachment();
                    uploadAttachment.archivo = anexByte;
                    uploadAttachment.numeroDocumento = docInfo.consecutivoDocumento;
                    uploadAttachment.nombre = file.Name.Substring(0, file.Name.Length - 4);
                    uploadAttachment.formato = file.Extension.Substring(1);
                    uploadAttachment.tokenEmpresa = tokenEmpresa;
                    uploadAttachment.tokenPassword = tokenAuthorizacion;
                    uploadAttachment.tipo = "2";
                    if (i + 1 == lbxAnexos.Items.Count)
                    {
                        uploadAttachment.enviar = "1";
                    }
                    else
                    {
                        uploadAttachment.enviar = "0";
                    }
                    fileRespuesta = serviceArchivos.EnviarArchivo(uploadAttachment);
                    if (fileRespuesta.codigo == 200)
                    {
                        rtxInformacion.Text += "Archivo: " + file.Name + " procesado correctamente" + Environment.NewLine;
                        procesados++;
                    }
                    else
                    {
                        rtxInformacion.Text += "Archivo: " + file.Name + " no fue transmitido" + Environment.NewLine;
                    }
                }
                else
                {
                    //no debería entrar a este ciclo
                }

            }
            return procesados;
        }

    }      
}

