using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfdiEntidadesGP
{
    public class DocumentoVentaGP
    {
        vwCfdiGeneraDocumentoDeVenta _DocVenta;
        public List<vwCfdiFacturaImpuestosCabecera> _facimpcab;
        List <vwCfdiConceptos> _LDocVentaConceptos;
        public List<vwCfdiRelacionados> _LDocVentaRelacionados;
        public List<vwCfdiFacturaImpuestosDetalles> _facimpdet;
        public List<vwCfdiClienteDestinatario> _clides;
        public List<vwCfdiClienteObligaciones> _cliobl;
        public List<vwCfdiMediosDePago> _medpag;
        private string _leyendasXml = string.Empty;
        
        public DocumentoVentaGP()
        {
            _LDocVentaConceptos = new List<vwCfdiConceptos>();
            _DocVenta = new vwCfdiGeneraDocumentoDeVenta();
            _facimpcab = new List<vwCfdiFacturaImpuestosCabecera>();
            _LDocVentaRelacionados = new List<vwCfdiRelacionados>();
            _facimpdet = new List<vwCfdiFacturaImpuestosDetalles>();
            _clides = new List<vwCfdiClienteDestinatario>();
            _cliobl = new List<vwCfdiClienteObligaciones>();
            _medpag = new List<vwCfdiMediosDePago>();
        }

        public vwCfdiGeneraDocumentoDeVenta DocVenta
        {
            get
            {
                return _DocVenta;
            }

            set
            {
                _DocVenta = value;
            }
        }
        public List<vwCfdiFacturaImpuestosCabecera> facimpcab
        {
            get
            {
                return _facimpcab;
            }

            set
            {
                _facimpcab = value;
            }
        }

        public List<vwCfdiConceptos> LDocVentaConceptos
        {
            get
            {
                return _LDocVentaConceptos;
            }

            set
            {
                _LDocVentaConceptos = value;
            }
        }

        public List<vwCfdiRelacionados> LDocVentaRelacionados
        {
            get
            {
                return _LDocVentaRelacionados;
            }

            set
            {
                _LDocVentaRelacionados = value;
            }
        }

        public List<vwCfdiFacturaImpuestosDetalles> facimpdet
        {
            get
            {
                return _facimpdet;
            }

            set
            {
                _facimpdet = value;
            }
        }
        public List<vwCfdiClienteDestinatario> clides
        {
            get
            {
                return _clides;
            }

            set
            {
                _clides = value;
            }
        }
        public List<vwCfdiClienteObligaciones> cliobl
        {
            get
            {
                return _cliobl;
            }

            set
            {
                _cliobl = value;
            }
        }
        public List<vwCfdiMediosDePago> medpag
        {
            get
            {
                return _medpag;
            }

            set
            {
                _medpag = value;
            }
        }
        public string LeyendasXml { get => _leyendasXml; set => _leyendasXml = value; }

        public static async Task<string> GetParametrosTipoLeyendaAsync()
        {
            using (var ctx = new COLEntities())
            {
                var leyendas = await ctx.fCfdiParametrosTipoLeyenda("LEYENDASFE", "CMP").AsQueryable().ToListAsync();
                return leyendas.FirstOrDefault().inetinfo;
            }
        }

        public void GetDatosDocumentoVenta(String Sopnumbe, short Soptype)
        {            
            using (COLEntities dv = new COLEntities())
            {
                _DocVenta = dv.vwCfdiGeneraDocumentoDeVenta
                                    .Where(v => v.sopnumbe == Sopnumbe && v.soptype == Soptype)
                                    .First();                
                _LDocVentaConceptos = dv.vwCfdiConceptos
                                    .Where(v => v.sopnumbe == Sopnumbe && v.soptype == Soptype)
                                    .ToList();
                _LDocVentaRelacionados = dv.vwCfdiRelacionados
                                    .Where(v => v.sopnumbeFrom == Sopnumbe && v.soptypeFrom == Soptype)
                                    .ToList();
                _facimpcab = dv.vwCfdiFacturaImpuestosCabecera
                                   .Where(v => v.sopnumbe == Sopnumbe && v.soptype == Soptype)
                                   .ToList();
                _facimpdet = dv.vwCfdiFacturaImpuestosDetalles
                                    .Where(v => v.sopnumbe == Sopnumbe && v.soptype == Soptype)
                                    .ToList();
                _medpag = dv.vwCfdiMediosDePago
                                    .Where(v => v.sopnumbe == Sopnumbe && v.soptype == Soptype)
                                    ?.ToList();
                _clides = dv.vwCfdiClienteDestinatario
                                    .Where(v => v.custnmbr == _DocVenta.custnmbr.Trim())
                                    ?.ToList();

                _cliobl = dv.vwCfdiClienteObligaciones
                                   .Where(v => v.custnmbr == _DocVenta.custnmbr.Trim())
                                   ?.ToList();

                //var resDoc = dv.vwCfdiGeneraDocumentoDeVenta.Where(v => v.sopnumbe == Sopnumbe && v.soptype == Soptype);
                //foreach (vwCfdiGeneraDocumentoDeVenta doc in resDoc)
                //{
                //    _DocVenta = doc;
                //    break;
                //}
                //var resCon = dv.vwCfdiConceptos.Where(v => v.sopnumbe == Sopnumbe && v.soptype == Soptype);
                //foreach (vwCfdiConceptos c in resCon)
                //{
                //    _LDocVentaConceptos.Add(c);
                //}

                //var resRelacionados = dv.vwCfdiRelacionados.Where(v => v.sopnumbeFrom == Sopnumbe && v.soptypeFrom == Soptype);
                //foreach (vwCfdiRelacionados c in resRelacionados)
                //{
                //    _LDocVentaRelacionados.Add(c);
                //}

            }

        }
    }
}
