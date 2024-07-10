using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Usuario
    {
        public int ID_Usuario { get; set; }
        public string Nombre { get; set; }
        public string Contraseña { get; set; }
        public Rol oRol { get; set; }
        public bool Estado { get; set; }
        public string FechaRegistro { get; set; }


    }
}
