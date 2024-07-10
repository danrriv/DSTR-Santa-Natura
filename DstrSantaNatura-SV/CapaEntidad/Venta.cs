using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Venta
    {
        public int ID_Venta { get; set; }
        public Usuario oUsuario { get; set; }
        public string TipoDocumento { get; set; }
        public string Serie { get; set; }
        public string NDocumento { get; set; }
        public string DocumentoCliente { get; set; }
        public string NombreCliente { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
        public List<Detalle_Venta> oDetalleVenta { get; set; }
        public string FechaRegistro { get; set; }

    }
}
