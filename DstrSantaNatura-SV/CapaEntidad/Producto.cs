﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Producto
    {
        public int ID_Producto { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public Categoria oCategoria { get; set; }
        public decimal DescuentoCompra { get; set; }
        public decimal PrecioCatalogo { get; set; }
        public decimal PrecioCompra { get; set; }
        public int Stock { get; set; }
        public bool Estado { get; set; }
        public string FechaRegistro { get; set; }
    }
}
