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
        vwCfdiGeneraResumenDiario _resumenCab;
        public vwCfdiFacturaImpuestosCabecera _facimpcab;
        List<vwCfdiGeneraResumenDiario> _lDocResumenLineas;
        List <vwCfdiConceptos> _LDocVentaConceptos;
        public List<vwCfdiRelacionados> _LDocVentaRelacionados;
        public vwCfdiFacturaImpuestosDetalles _facimpdet;
        public vwCfdiClienteDestinatario _clides;
        public vwCfdiClienteObligaciones _cliobl;
        public vwCfdiMediosDePago _medpag;
        private string _leyendasXml = string.Empty;
        
        public DocumentoVentaGP()
        {
            _LDocVentaConceptos = new List<vwCfdiConceptos>();
            _DocVenta = new vwCfdiGeneraDocumentoDeVenta();
            _facimpcab = new vwCfdiFacturaImpuestosCabecera();
            _LDocVentaRelacionados = new List<vwCfdiRelacionados>();
            _resumenCab = new vwCfdiGeneraResumenDiario();
            _lDocResumenLineas = new List<vwCfdiGeneraResumenDiario>();
            _facimpdet = new vwCfdiFacturaImpuestosDetalles();
            _clides = new vwCfdiClienteDestinatario();
            _cliobl = new vwCfdiClienteObligaciones();
            _medpag = new vwCfdiMediosDePago();
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
        public vwCfdiFacturaImpuestosCabecera facimpcab
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

        public vwCfdiGeneraResumenDiario ResumenCab
        {
            get
            {
                return _resumenCab;
            }

            set
            {
                _resumenCab = value;
            }
        }

        public List<vwCfdiGeneraResumenDiario> LDocResumenLineas
        {
            get
            {
                return _lDocResumenLineas;
            }

            set
            {
                _lDocResumenLineas = value;
            }
        }
        public vwCfdiFacturaImpuestosDetalles facimpdet
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
        public vwCfdiClienteDestinatario clides
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
        public vwCfdiClienteObligaciones cliobl
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
        public vwCfdiMediosDePago medpag
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
            using (var ctx = new PER10Entities())
            {
                var leyendas = await ctx.fCfdiParametrosTipoLeyenda("LEYENDASFE", "CMP").AsQueryable().ToListAsync();
                return leyendas.FirstOrDefault().inetinfo;
            }
        }

        public void GetDatosDocumentoVenta(String Sopnumbe, short Soptype)
        {            
            using (PER10Entities dv = new PER10Entities())
            {
                _DocVenta = dv.vwCfdiGeneraDocumentoDeVenta
                                    .Where(v => v.sopnumbe == Sopnumbe && v.soptype == Soptype)
                                    .First();                
                _LDocVentaConceptos = dv.vwCfdiConceptos
                                    .Where(v => v.sopnumbe == Sopnumbe && v.soptype == Soptype)
                                    .ToList();                                     
                //_LDocVentaRelacionados = dv.vwCfdiRelacionados
                                   //.Where(v => v.sopnumbeFrom == Sopnumbe && v.soptypeFrom == Soptype)
                                   //.ToList();
                _facimpcab = dv.vwCfdiFacturaImpuestosCabecera
                                   .Where(v => v.sopnumbe == Sopnumbe && v.soptype == Soptype)
                                   .First();
                _facimpdet = dv.vwCfdiFacturaImpuestosDetalles
                                    .Where(v => v.sopnumbe == Sopnumbe && v.soptype == Soptype)
                                    .First();
                _medpag = dv.vwCfdiMediosDePago
                                    .Where(v => v.sopnumbe == Sopnumbe && v.soptype == Soptype)
                                    .First();
                _clides = dv.vwCfdiClienteDestinatario
                                    .Where(v => v.CUSTNMBR == _DocVenta.CUSTNMBR)
                                    .First();
                _cliobl = dv.vwCfdiClienteObligaciones
                                   .Where(v => v.CUSTNMBR == _DocVenta.CUSTNMBR)
                                   .First();
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
        public void GetDatosResumenBoletas(String Sopnumbe, short Soptype)
        {
            using (PER10Entities dv = new PER10Entities())
            {
                //System.Diagnostics.Debugger.NotifyOfCrossThreadDependency();
                _lDocResumenLineas = dv.vwCfdiGeneraResumenDiario
                                        .Where(v => v.numResumenDiario == Sopnumbe && v.tipoResumenDiario == Soptype)
                                        .ToList();

                _resumenCab = _lDocResumenLineas.First();
            }
        }
    }
}
