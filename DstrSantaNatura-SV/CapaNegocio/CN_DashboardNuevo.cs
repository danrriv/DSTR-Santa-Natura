using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_DashboardNuevo
    {
        private CD_DashboardNuevo accesDB = new CD_DashboardNuevo();
        
        public void cnLowProductos(DashboardNuevo obj)
        {
            accesDB.getLowProductos(obj);
        }
        
        public void cnTopProductos(DashboardNuevo obj, string fromDate, string toDate)
        {
            accesDB.getTopProductos(obj, fromDate, toDate);
        }

        public void cnTotalVentas(DashboardNuevo obj, string fromDate, string toDate)
        {
            accesDB.getTotalVentas(obj, fromDate, toDate);
        }

        public void cnItemsNumeros(DashboardNuevo obj)
        {
            accesDB.getItemsNumeros(obj);
        }

        public void cnNumeroVentas(DashboardNuevo obj, string fromDate, string toDate)
        {
            accesDB.getNumeroVentas(obj, fromDate, toDate);
        }

        public void cnTotalCompras(DashboardNuevo obj, string fromDate, string toDate)
        {
            accesDB.getTotComprasPorFecha(obj, fromDate, toDate);
        }
    }
}
