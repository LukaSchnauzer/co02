using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace cfd.FacturaElectronica
{
    public class ServicioOpenInvoice
    {
        private string _rutaArchivo;

        public string RutaArchivo
        {
            get
            {
                return _rutaArchivo;
            }

            set
            {
                _rutaArchivo = value;
            }
        }

    }
}
