using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Ganancia
    {
        private CD_Ganancia objcd_ganancia = new CD_Ganancia();
        public List<Ganancia> Ganancias(string fechainicio, string fechafin)
        {
            return objcd_ganancia.ganancias(fechainicio, fechafin);
        }
    }
}
