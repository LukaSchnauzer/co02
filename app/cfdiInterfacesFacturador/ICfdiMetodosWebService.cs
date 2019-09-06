using cfdiEntidadesGP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfdiColombiaInterfaces
{
    public interface ICfdiMetodosWebService
    {
        string TimbraYEnviaServicioDeImpuesto(string ruc, string usuario, string usuarioPassword, string texto);
        Task<string> TimbraYEnviaServicioDeImpuestoAsync(string ruc, string usuario, string usuarioPassword, DocumentoVentaGP documentoGP);

        string TimbraYEnviaServicioDeImpuesto(string ruc, string usuario, string usuarioPassword, DocumentoVentaGP documentoGP);

        Task<string> ObtienePDFdelOSEAsync(string ruc, string usuario, string usuarioPassword, string tipoDoc, string serie, string correlativo, string ruta, string nombreArchivo, string extension);
        string ObtienePDFdelOSE(string ruc, string usuario, string usuarioPassword, string tipoDoc, string serie, string correlativo, string ruta, string nombreArchivo, string extension);

        Task<string> ObtieneXMLdelOSEAsync(string ruc, string usuario, string usuarioPassword, string tipoDoc, string serie, string correlativo);
        string ObtieneXMLdelOSE(string ruc, string usuario, string usuarioPassword, string tipoDoc, string serie, string correlativo);
        Task<string> SolicitarBajaAsync(string ruc, string usuario, string usuarioPassword, string nroDocumento, string motivo);

        //Task<string> ObtieneCDRdelOSEAsync(string ruc, string tipoDoc, string serie, string correlativo, string rutaYNomArchivoCfdi);
        //string ObtieneCDRdelOSE(string ruc, string tipoDoc, string serie, string correlativo);
        //Tuple<string, string> Baja(string ruc, string usuario, string usuarioPassword, string nroDocumento);
        Task<string> ConsultaStatusAlOSEAsync(string ruc, string usuario, string usuarioPassword, string tipoDoc, string serie, string correlativo);

        //Tuple<string, string> ResumenDiario(string ruc, string usuario, string usuarioPassword, string texto);

    }

}
