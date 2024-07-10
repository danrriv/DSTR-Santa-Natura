using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Devolucion
    {
        public int ID_Devolucion { get; set; }
        public Producto oProducto { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public bool Estado { get; set; }
        public string FechaRegistro { get; set; }
    }
}
