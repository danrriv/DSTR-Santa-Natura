using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Compra
    {
        public int ID_Compra { get; set; }
        public Usuario oUsuario { get; set; }
        public string Serie { get; set; }
        public string NDocumento { get; set; }
        public decimal Total { get; set; }
        public decimal Igv { get; set; }
        public List<Detalle_Compra> oDetalleCompra { get; set; }
        public string FechaRegistro { get; set; }

    }
}
