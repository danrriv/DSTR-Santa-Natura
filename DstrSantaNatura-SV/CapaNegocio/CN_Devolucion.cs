using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Devolucion
    {
        private CD_Devolucion objcd_devolucion = new CD_Devolucion();

        public List<Devolucion> Listar()
        {
            return objcd_devolucion.Listar();
        }
        public int Registrar(Devolucion obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            if (Mensaje != string.Empty)
            {
                return 0;
            }
            else
            {
                return objcd_devolucion.Registrar(obj, out Mensaje);
            }
        }
        public bool Editar(Devolucion obj, out string Mensaje)
        {

            Mensaje = string.Empty;
            if (Mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objcd_devolucion.Editar(obj, out Mensaje);
            }
        }
        public bool Eliminar(Devolucion obj, out string Mensaje)
        {
            return objcd_devolucion.Eliminar(obj, out Mensaje);
        }

        public bool Eliminarcompleto(Devolucion obj, out string Mensaje)
        {
            return objcd_devolucion.Eliminarcompleto(obj, out Mensaje);
        }
    }
}
