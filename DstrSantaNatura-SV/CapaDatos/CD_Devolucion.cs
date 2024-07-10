using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Devolucion
    {
        
        public List<Devolucion> Listar()
        {
            List<Devolucion> lista = new List<Devolucion>();
            
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select d.ID_Devolucion,p.ID_Producto,p.Nombre,d.Descripcion,d.Cantidad,convert(char(10),d.FechaRegistro,103)[Fecha],d.Estado from DEVOLUCION d");
                    query.AppendLine("inner join Producto p on p.ID_Producto = d.ID_Producto");
                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    //SqlCommand cmd = new SqlCommand("sp_LISTARDEVOLUCION", oconexion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();
                    using(SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Devolucion()
                            {
                                ID_Devolucion = Convert.ToInt32(dr["ID_Devolucion"]),
                                oProducto = new Producto() { ID_Producto = Convert.ToInt32(dr["ID_Producto"]), Nombre =dr["Nombre"].ToString()},
                                Descripcion = dr["Descripcion"].ToString(),
                                Cantidad = Convert.ToInt32(dr["Cantidad"]),
                                Estado = Convert.ToBoolean(dr["Estado"]),
                                FechaRegistro = dr["Fecha"].ToString()
                            });
                        }
                    }
                    
                }
                catch(Exception ex)
                {
                    lista = new List<Devolucion>();
                }
            }
            return lista;
        }

        public int Registrar(Devolucion obj, out string Mensaje)
        {
            int resultado = 0;
            Mensaje = string.Empty;
            try
            {
                using(SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_REGISTRARDEVOLUCION", oconexion);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("Cantidad", obj.Cantidad);
                    cmd.Parameters.AddWithValue("ID_Producto", obj.oProducto.ID_Producto);
                    cmd.Parameters.AddWithValue("Estado", obj.Estado);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch(Exception ex)
            {
                resultado = 0;
                Mensaje = ex.Message;

            }
            return resultado;
        }
        public bool Editar(Devolucion obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;

            try
            {

                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {

                    SqlCommand cmd = new SqlCommand("SP_EDITARDEVOLUCION", oconexion);
                    cmd.Parameters.AddWithValue("ID_Devolucion", obj.ID_Devolucion);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("Cantidad", obj.Cantidad);
                    cmd.Parameters.AddWithValue("ID_Producto", obj.oProducto.ID_Producto);
                    cmd.Parameters.AddWithValue("Estado", obj.Estado);
                    cmd.Parameters.Add("Respuesta", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Respuesta"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                }

            }
            catch (Exception ex)
            {
                respuesta = false;
                Mensaje = ex.Message;
            }

            return respuesta;
        }
        public bool Eliminar(Devolucion obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;


            try
            {

                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {


                    SqlCommand cmd = new SqlCommand("SP_ELIMINARDEVOLUCION", oconexion);
                    cmd.Parameters.AddWithValue("ID_Devolucion", obj.ID_Devolucion);
                    cmd.Parameters.Add("Respuesta", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Respuesta"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                }

            }
            catch (Exception ex)
            {
                respuesta = false;
                Mensaje = ex.Message;
            }

            return respuesta;
        }
        public bool Eliminarcompleto(Devolucion obj, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;
            try
            {

                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {


                    SqlCommand cmd = new SqlCommand("SP_LIMPIARDEVOLUCION", oconexion);
                    cmd.Parameters.Add("Respuesta", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Respuesta"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                }

            }
            catch (Exception ex)
            {
                respuesta = false;
                Mensaje = ex.Message;
            }

            return respuesta;

        }

    }
}
