using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Ganancia
    {
        public string Producto { get; set; }
        public string PrecioCompra { get; set; }
        public string PrecioVenta { get; set; }
        public string CantidadComprada { get; set; }
        public string CantidadVendida { get; set; }
        public string Inversion { get; set; }
        public string GananciaBruta { get; set; }
        public string GananciaReal { get; set; }
    }
}
