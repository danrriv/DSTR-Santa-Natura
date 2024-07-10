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
    public class CD_Compra
    {
        public bool Registrar(Compra obj, DataTable DetalleCompra, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = String.Empty;
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {

                try
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarCompra", oconexion);
                    cmd.Parameters.AddWithValue("ID_Usuario", obj.oUsuario.ID_Usuario);
                    cmd.Parameters.AddWithValue("Serie", obj.Serie);
                    cmd.Parameters.AddWithValue("NDocumento", obj.NDocumento);
                    cmd.Parameters.AddWithValue("Total", obj.Total);
                    cmd.Parameters.AddWithValue("Igv", obj.Igv);
                    cmd.Parameters.AddWithValue("DetalleCompra", DetalleCompra);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();



                    Respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
                catch (Exception ex)
                {

                    Respuesta = false;
                    Mensaje = ex.Message;
                }
                return Respuesta;
            }
        }

        public Compra ObtenerCompra(string numero)
        {
            Compra obj = new Compra();
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();

                    query.AppendLine("SELECT c.ID_Compra,");
                    query.AppendLine("u.Nombre,");
                    query.AppendLine("c.Serie,c.NDocumento,c.Total,c.Igv,convert(char(10),c.FechaRegistro,103)[Fecha]");
                    query.AppendLine("FROM COMPRA c");
                    query.AppendLine("inner join USUARIO u on u.ID_Usuario = c.ID_Usuario");
                    query.AppendLine("where c.NDocumento = @numero");

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            obj = new Compra()
                            {
                                ID_Compra = Convert.ToInt32(dr["ID_Compra"]),
                                oUsuario = new Usuario() { Nombre = dr["Nombre"].ToString() },
                                Serie = dr["Serie"].ToString(),
                                NDocumento = dr["NDocumento"].ToString(),
                                Total = Convert.ToDecimal(dr["Total"]),
                                Igv = Convert.ToDecimal(dr["Igv"]),
                                FechaRegistro = dr["Fecha"].ToString()
                            };
                        }
                    }

                }
                catch (Exception ex)
                {
                    obj = new Compra();
                }
            }
            return obj;
        }

        public List<Detalle_Compra> ObtenerDetalleCompra(int idcompra)
        {
            List<Detalle_Compra> oLista = new List<Detalle_Compra>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();

                    query.AppendLine("Select p.Nombre,dc.PrecioCompra,dc.Cantidad,dc.Subtotal from DETALLE_COMPRA dc");
                    query.AppendLine("inner join PRODUCTO p on p.ID_Producto = dc.ID_Producto");
                    query.AppendLine("where dc.ID_Compra = @idcompra");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@idcompra", idcompra);
                    cmd.CommandType = System.Data.CommandType.Text;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            oLista.Add(new Detalle_Compra()
                            {
                                oProducto = new Producto() { Nombre = dr["Nombre"].ToString() },
                                PrecioCompra = Convert.ToDecimal(dr["PrecioCompra"].ToString()),
                                Cantidad = Convert.ToInt32(dr["Cantidad"].ToString()),
                                MontoTotal = Convert.ToDecimal(dr["Subtotal"].ToString()),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oLista = new List<Detalle_Compra>();
            }
            return oLista;
        }
    }
}
