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
            docGP = new DocumentoVentaGP();
            docGP.GetDatosDocumentoVenta(this.Sopnumbe, this.Soptype);
            docGP.LeyendasXml = ObtieneLeyendaConjunta(leyendas, string.Empty, docGP.DocVenta.leyendaPorFactura2);
        }

        public void EliminaStatusBaseDelLog()
        {
            docGP = new DocumentoVentaGP();
            docGP.EliminaStatusDelLog(this.Soptype, this.Sopnumbe, "emitido");
        }

        private string ObtieneLeyendaConjunta(string leyendas, string leyendaPorFactura, string leyendaPorFactura2)
        {
            string leyendaConjunta = leyendas;
            if (!string.IsNullOrEmpty(leyendas))
            {
                XElement leyendasX = XElement.Parse(leyendas);
                XElement nuevaSeccion;
                if (!string.IsNullOrEmpty(leyendaPorFactura))
                {
                    nuevaSeccion = new XElement("SECCION", new XAttribute("S", 1), new XAttribute("T", "Adicional"), new XAttribute("V", leyendaPorFactura));
                    leyendasX.Add(nuevaSeccion);
                }
                if (!string.IsNullOrEmpty(leyendaPorFactura2))
                {
                    nuevaSeccion = new XElement("SECCION", new XAttribute("S", 2), new XAttribute("T", "Adicionales"), new XAttribute("V", leyendaPorFactura2));
                    leyendasX.Add(nuevaSeccion);
                }

                leyendaConjunta = leyendasX.ToString();
            }

            return leyendaConjunta;
        }

        public void ArmarBaja(String motivoBaja)
        {
            DocumentoVentaGP docGP = new DocumentoVentaGP();

            docGP.GetDatosDocumentoVenta(this.Sopnumbe, this.Soptype);


        }
    }
}
