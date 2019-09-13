using System;
using System.Collections.Generic;
using System.Text;
using cfdiEntidadesGP;
using Comun;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using cfdiColombia;
using MaquinaDeEstados;

//namespace cfdiPeru
namespace cfdiColombia
{
    class vwCfdTransaccionesDeVenta : vwCfdiTransaccionesDeVenta
    {
        DocumentoVentaGP docGP;
        private const string FormatoFecha = "yyyy-MM-dd";
        private const string FormatoFechaHora = "yyyy-MM-dd";
        public Maquina CicloDeVida { get; set; }

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

                //if (!string.IsNullOrEmpty(leyendas) && !string.IsNullOrEmpty(docGP.DocVenta.leyendaPorFactura))
                if (!string.IsNullOrEmpty(leyendas))
                {
                    XElement leyendasX = XElement.Parse(leyendas);
                    //XElement nuevaSeccion = new XElement("SECCION", new XAttribute("S", 1), new XAttribute("T", "Adicional"), new XAttribute("V", docGP.DocVenta.leyendaPorFactura));
                    XElement nuevaSeccion = new XElement("SECCION", new XAttribute("S", 1), new XAttribute("T", "Adicional"));
                    leyendasX.Add(nuevaSeccion);
                    leyendaConjunta = leyendasX.ToString();
                }
                docGP.LeyendasXml = leyendaConjunta;


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
