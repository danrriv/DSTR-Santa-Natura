using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ReporteCompra
    {
        public string Fecha { get; set; }
        public string NDocumento { get; set; }
        public string Serie { get; set; }
        public string Total { get; set; }
        public string Usuario { get; set; }
        public string CodigoProducto { get; set; }
        public string Producto { get; set; }
        public string Categoria { get; set; }
        public string PrecioCompra { get; set; }
        public string Cantidad { get; set; }
        public string Subtotal { get; set; }
    }
}
