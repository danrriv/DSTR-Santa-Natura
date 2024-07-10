using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public struct RevenueByDate
    {
        public string Date { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class DashboardNuevo
    {
        //Fields & Properties
        public DateTime startDate;
        public DateTime endDate;
        public int numberDays;

        public string NumCustomers { get; set; }
        public string NumSuppliers { get; set; }
        public string NumProducts { get; set; }
        
        public List<RevenueByDate> GrossRevenueList { get; set; }
        public string NumOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public string TotalProfit { get; set; }


        

        public List<KeyValuePair<string, int>> UnderstockList = new List<KeyValuePair<string, int>>();
        public List<KeyValuePair<string, int>> TopProductsList { get; set; }
    }
    }
