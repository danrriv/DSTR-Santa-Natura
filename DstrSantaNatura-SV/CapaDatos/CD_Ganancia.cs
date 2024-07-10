using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Ganancia
    {
        public List<Ganancia> ganancias(string fechainicio, string fechafin)
        {
            List<Ganancia> lista = new List<Ganancia>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    SqlCommand cmd = new SqlCommand("sp_Ganancias", oconexion);
                    cmd.Parameters.AddWithValue("fechainicio", fechainicio);
                    cmd.Parameters.AddWithValue("fechafin", fechafin);
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Ganancia()
                            {
                                Producto = dr["Producto"].ToString(),
                                PrecioCompra = dr["PrecioCompra"].ToString(),
                                PrecioVenta = dr["PrecioVenta"].ToString(),
                                CantidadComprada = dr["CantidadComprada"].ToString(),
                                CantidadVendida = dr["CantidadVendida"].ToString(),
                                Inversion = dr["Inversion"].ToString(),
                                GananciaBruta = dr["GBruta"].ToString(),
                                GananciaReal = dr["GReal"].ToString(),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<Ganancia>();
                }
            }

            return lista;

        }
    }
}
