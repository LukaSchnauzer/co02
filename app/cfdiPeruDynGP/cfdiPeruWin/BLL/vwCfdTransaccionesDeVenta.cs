using System;
using System.Collections.Generic;
using System.Text;
using cfdiEntidadesGP;
using Comun;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;

namespace cfdiPeru
{
    class vwCfdTransaccionesDeVenta : vwCfdiTransaccionesDeVenta
    {
        DocumentoVentaGP docGP;
        private const string FormatoFecha = "yyyy-MM-dd";
        private const string FormatoFechaHora = "yyyy-MM-dd";

        public DocumentoVentaGP DocGP { get => docGP; set => docGP = value; }

        public vwCfdTransaccionesDeVenta(string connstr, string nombreVista)
        {
            this.ConnectionString = connstr;
            this.QuerySource = nombreVista;
            this.MappingName = nombreVista;

            //this.QuerySource = "vwCfdiTransaccionesDeVenta";
            //this.MappingName = "vwCfdiTransaccionesDeVenta";
        }

        public static async Task<string> ObtieneLeyendasAsync()
        {
            return await DocumentoVentaGP.GetParametrosTipoLeyendaAsync();

        }

        public void ArmarDocElectronico(string leyendas)
        {
            string leyendaConjunta = leyendas;
            try
            {
                docGP = new DocumentoVentaGP();
                docGP.GetDatosDocumentoVenta(this.Sopnumbe, this.Soptype);

                if (!string.IsNullOrEmpty(leyendas) && !string.IsNullOrEmpty(docGP.DocVenta.leyendaPorFactura))
                {
                    XElement leyendasX = XElement.Parse(leyendas);
                    XElement nuevaSeccion = new XElement("SECCION", new XAttribute("S", 1), new XAttribute("T", "Adicional"), new XAttribute("V", docGP.DocVenta.leyendaPorFactura));
                    leyendasX.Add(nuevaSeccion);
                    leyendaConjunta = leyendasX.ToString();
                }
                docGP.LeyendasXml = leyendaConjunta;

                //_docElectronico = new DocumentoElectronico();
                //_docElectronico.TipoDocumento = docGP.DocVenta.tipoDocumento;
                //_docElectronico.IdDocumento = docGP.DocVenta.idDocumento;
                //_docElectronico.FechaEmision = this.Fechahora.ToString(FormatoFechaHora);
                //_docElectronico.Moneda = docGP.DocVenta.moneda;
                //_docElectronico.Emisor.NroDocumento = docGP.DocVenta.emisorNroDoc;
                //_docElectronico.Emisor.NombreComercial = docGP.DocVenta.emisorNombre;
                //_docElectronico.Emisor.NombreLegal = docGP.DocVenta.emisorNombre;
                //_docElectronico.Emisor.Ubigeo = docGP.DocVenta.emisorUbigeo;
                //_docElectronico.Emisor.Direccion = docGP.DocVenta.emisorDireccion;
                //_docElectronico.Emisor.Urbanizacion = docGP.DocVenta.emisorUrbanizacion;
                //_docElectronico.Emisor.Departamento = docGP.DocVenta.emisorDepartamento;
                //_docElectronico.Emisor.Provincia = docGP.DocVenta.emisorProvincia;
                //_docElectronico.Emisor.Distrito = docGP.DocVenta.emisorDistrito;
                //_docElectronico.Receptor.TipoDocumento = docGP.DocVenta.receptorTipoDoc;
                //_docElectronico.Receptor.NroDocumento = docGP.DocVenta.receptorNroDoc;
                //_docElectronico.Receptor.NombreComercial = docGP.DocVenta.receptorNombre;
                //_docElectronico.Receptor.NombreLegal = docGP.DocVenta.receptorNombre;
                //_docElectronico.Receptor.EMailTo = docGP.DocVenta.emailTo;
                //_docElectronico.TipoOperacion = docGP.DocVenta.tipoOperacion;

                //lDetalleDocumento = new List<DetalleDocumento>();
                //int i = 1;
                //foreach (vwCfdiConceptos d in docGP.LDocVentaConceptos)
                //{
                //    lDetalleDocumento.Add(new DetalleDocumento()
                //    {
                //        CodigoItem = d.ITEMNMBR,
                //        Id = i, 
                //        Descripcion = d.Descripcion,
                //        Cantidad = d.cantidad,
                //        UnidadMedida = d.udemSunat,
                //        PrecioReferencial = Convert.ToDecimal(d.precioUniConIva),
                //        TotalVenta = Convert.ToDecimal(d.importe),
                        
                //    });
                //    i++;
                //}
                //_docElectronico.Items = new List<DetalleDocumento>();
                //_docElectronico.Items = lDetalleDocumento;

                //if (docGP.LDocVentaRelacionados.Count > 0)
                //{
                //    _docElectronico.Relacionados = new List<DocumentoRelacionado>();
                //    _docElectronico.Discrepancias = new List<Discrepancia>();
                //    foreach (vwCfdiRelacionados d in docGP.LDocVentaRelacionados)
                //    {
                //        _docElectronico.Relacionados.Add(new DocumentoRelacionado()
                //        {
                //            NroDocumento = d.sopnumbeTo,
                //            TipoDocumento = d.tipoDocumento
                //        });

                //        _docElectronico.Discrepancias.Add(new Discrepancia()
                //        {
                //            NroReferencia = d.sopnumbeTo
                //        });
                //    }

                //}


            }
            catch (Exception)
            {

                throw;
            }
        }

        public void ArmarBaja(String motivoBaja)
        {
            DocumentoVentaGP docGP = new DocumentoVentaGP();

            docGP.GetDatosDocumentoVenta(this.Sopnumbe, this.Soptype);


        }
    }
}
