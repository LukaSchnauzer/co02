using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaquinaDeEstados
{
    public class Estado
    {
        private String _nombre;
        private int _idx;
        private int _idxSuperEstado;

        public Estado(String nombre, int idx, int idxSuperEstado)
        {
            _nombre = nombre;
            _idx = idx;
            _idxSuperEstado = idxSuperEstado;
        }

        public string Nombre { get => _nombre; set => _nombre = value; }
        public int Idx { get => _idx; set => _idx = value; }
        public int IdxSuperEstado { get => _idxSuperEstado; set => _idxSuperEstado = value; }
    }
}
