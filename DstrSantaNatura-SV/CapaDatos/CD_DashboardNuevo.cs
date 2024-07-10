using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{ 

    public class CD_DashboardNuevo
    {
        SqlConnection oconexion = new SqlConnection(Conexion.cadena);
        SqlCommand cmd;
        SqlDataReader dr;

        public void getItemsNumeros(DashboardNuevo obj)
        {
            cmd = new SqlCommand("spc_ItemsNumeros", oconexion);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter ncliente = new SqlParameter("@nclient", 0); ncliente.Direction = ParameterDirection.Output;
            SqlParameter nprod = new SqlParameter("@nprod", 0); nprod.Direction = ParameterDirection.Output;
            SqlParameter ncategora = new SqlParameter("@ncateg", 0); ncategora.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(nprod);
            cmd.Parameters.Add(ncategora);
            cmd.Parameters.Add(ncliente);
            oconexion.Open();
            cmd.ExecuteNonQuery();
            obj.NumCustomers = cmd.Parameters["@nclient"].Value.ToString();
            obj.NumSuppliers = cmd.Parameters["@nprod"].Value.ToString();
            obj.NumProducts = cmd.Parameters["@ncateg"].Value.ToString();
            oconexion.Close();
        }

        public void getNumeroVentas(DashboardNuevo obj, string fromDate, string toDate)
        {
            cmd = new SqlCommand("spc_NumVentasPorFecha", oconexion);
            cmd.Parameters.AddWithValue("fromDate", fromDate);
            cmd.Parameters.AddWithValue("toDate", toDate);
            cmd.CommandType= CommandType.StoredProcedure;
            SqlParameter nventas = new SqlParameter("@nventas", 0); nventas.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(nventas);
            oconexion.Open();
            cmd.ExecuteNonQuery();
            obj.NumOrders = cmd.Parameters["@nventas"].Value.ToString();
            oconexion.Close();
        }

        public void getTotComprasPorFecha(DashboardNuevo obj, string fromDate, string toDate)
        {
            cmd = new SqlCommand("spc_TotalComprasPorFecha", oconexion);
            cmd.Parameters.AddWithValue("fromDate", fromDate);
            cmd.Parameters.AddWithValue("toDate", toDate);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter totcompras = new SqlParameter("@totcompras", 0); totcompras.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(totcompras);
            oconexion.Open();
            cmd.ExecuteNonQuery();
            obj.TotalProfit = cmd.Parameters["@totcompras"].Value.ToString();
            oconexion.Close();
        }

        public void getTopProductos(DashboardNuevo obj, string fromDate, string toDate)
        {
            obj.TopProductsList = new List<KeyValuePair<string, int>>();
            SqlCommand cmd = new SqlCommand("spc_TopProductosPorFecha", oconexion);
            cmd.Parameters.AddWithValue("fromDate", fromDate);
            cmd.Parameters.AddWithValue("toDate", toDate);
            cmd.CommandType = CommandType.StoredProcedure;
            oconexion.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                obj.TopProductsList.Add(new KeyValuePair<string, int>(dr.GetString(0), (int)dr[1]));
            }
            dr.Close();
            oconexion.Close();
        }

        public void getLowProductos(DashboardNuevo obj)
        {
            obj.UnderstockList = new List<KeyValuePair<string, int>>();
            cmd = new SqlCommand("ProdBajoStock", oconexion);
            cmd.CommandType = CommandType.StoredProcedure;
            oconexion.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                obj.UnderstockList.Add(new KeyValuePair<string, int>(dr.GetString(0), (int)dr[1]));
            }
            dr.Close();
            oconexion.Close();
        }

        public void getTotalVentas(DashboardNuevo obj, string fromDate, string toDate)
        {
            obj.GrossRevenueList = new List<RevenueByDate>();
            //obj.TotalProfit = 0;
            obj.TotalRevenue = 0;
            cmd = new SqlCommand("spc_TotalVentasPorFecha", oconexion);
            cmd.Parameters.AddWithValue("fromDate", fromDate);
            cmd.Parameters.AddWithValue("toDate", toDate);
            cmd.CommandType = CommandType.StoredProcedure;
            oconexion.Open();
            var reader = cmd.ExecuteReader();
            var resultTable = new List<KeyValuePair<DateTime, decimal>>();
            while (reader.Read())
            {
                resultTable.Add(
                    new KeyValuePair<DateTime, decimal>((DateTime)reader[0], (decimal)reader[1])
                    );
                obj.TotalRevenue += (decimal)reader[1];
            }
            //obj.TotalProfit = obj.TotalRevenue * 0.2m;//20%
            reader.Close();

            obj.numberDays = (Convert.ToDateTime(toDate) - Convert.ToDateTime(fromDate)).Days;
            //Group by Hours
            if (obj.numberDays <= 1)
            {
                obj.GrossRevenueList = (from orderList in resultTable
                                    group orderList by orderList.Key.ToString("hh tt")
                                   into order
                                    select new RevenueByDate
                                    {
                                        Date = order.Key,
                                        TotalAmount = order.Sum(amount => amount.Value)
                                    }).ToList();
            }   
            //Group by Days
            else if (obj.numberDays <= 30)
            {   
                obj.GrossRevenueList = (from orderList in resultTable
                                    group orderList by orderList.Key.ToString("dd MMM")
                                   into order
                                    select new RevenueByDate
                                    {
                                        Date = order.Key,
                                        TotalAmount = order.Sum(amount => amount.Value)
                                    }).ToList();
            }   
                
            //Group by Weeks
            else if (obj.numberDays <= 92)
            {   
                obj.GrossRevenueList = (from orderList in resultTable
                                    group orderList by CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                        orderList.Key, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                                   into order
                                    select new RevenueByDate
                                    {
                                        Date = "Week " + order.Key.ToString(),
                                        TotalAmount = order.Sum(amount => amount.Value)
                                    }).ToList();
            }   
                
            //Group by Months
            else if (obj.numberDays <= (365 * 2))
            {   
                bool isYear = obj.numberDays <= 365 ? true : false;
                obj.GrossRevenueList = (from orderList in resultTable
                                    group orderList by orderList.Key.ToString("MMM yyyy")
                                   into order
                                    select new RevenueByDate
                                    {
                                        Date = isYear ? order.Key.Substring(0, order.Key.IndexOf(" ")) : order.Key,
                                        TotalAmount = order.Sum(amount => amount.Value)
                                    }).ToList();
            }

            //Group by Years
            else
            {
                obj.GrossRevenueList = (from orderList in resultTable
                                    group orderList by orderList.Key.ToString("yyyy")
                                   into order
                                    select new RevenueByDate
                                    {
                                        Date = order.Key,
                                        TotalAmount = order.Sum(amount => amount.Value)
                                    }).ToList();
            }
            oconexion.Close();
        }

    }
}
