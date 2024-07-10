using CapaEntidad;
using CapaNegocio;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmDashboardNuevo : Form
    {
        //Fields
        private Button currentButton;
        
        public frmDashboardNuevo()
        {
            InitializeComponent();
            //Default - Last 7 days
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            SetDateMenuButtonsUI(btnUlt7dias);
            btnUlt7dias.Select();

            LoadData();
        }

        //Private methods
        private void LoadData()
        {
            string fechainicio = dtpStartDate.Value.ToString();
            string fechafin = dtpEndDate.Value.ToString();

            CN_DashboardNuevo neg = new CN_DashboardNuevo();
            DashboardNuevo model = new DashboardNuevo();
            neg.cnLowProductos(model);
            neg.cnTopProductos(model,fechainicio, fechafin);
            neg.cnTotalVentas(model, fechainicio, fechafin);
            neg.cnItemsNumeros(model);
            neg.cnNumeroVentas(model, fechainicio, fechafin);
            neg.cnTotalCompras(model, fechainicio, fechafin);

            lblNumOrders.Text = model.NumOrders.ToString();
            lblTotalRevenue.Text = "S/." + model.TotalRevenue.ToString();
            lblTotalCompras.Text = "S/." + model.TotalProfit.ToString();

            lblNumCustomers.Text = model.NumCustomers.ToString();
            lblNumSuppliers.Text = model.NumSuppliers.ToString();
            lblNumProducts.Text = model.NumProducts.ToString();

            chartGrossRevenue.DataSource = model.GrossRevenueList;
            chartGrossRevenue.Series[0].XValueMember = "Date";
            chartGrossRevenue.Series[0].YValueMembers = "TotalAmount";
            chartGrossRevenue.DataBind();

            chartTopProducts.DataSource = model.TopProductsList;
            chartTopProducts.Series[0].XValueMember = "Key";
            chartTopProducts.Series[0].YValueMembers = "Value";
            chartTopProducts.DataBind();

            dgvUnderstock.DataSource = model.UnderstockList;
            dgvUnderstock.Columns[0].HeaderText = "Producto";
            dgvUnderstock.Columns[1].HeaderText = "Unidad";
            Console.WriteLine("Loaded view :)");

        }
        public void SetDateMenuButtonsUI(object button)
        {
            var btn = (Button)button;
            btn.BackColor = btnUlt30dias.FlatAppearance.BorderColor;
            btn.ForeColor = Color.White;

            if (currentButton != null && currentButton != btn)
            { 
                currentButton.BackColor = this.BackColor;
                currentButton.ForeColor = Color.FromArgb(158, 163, 167);
            }
            currentButton = btn;
            //dtpStartDate.Enabled = false;
            //dtpEndDate.Enabled = false;
            //btnOkCustomDate.Visible = false;
        }

        private void btnHoy_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today;
            dtpEndDate.Value = DateTime.Now;
            btnOkCustomDate.Visible = false;
            LoadData();
            SetDateMenuButtonsUI(sender);
        }

        private void btnUlt7dias_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            btnOkCustomDate.Visible = false;
            LoadData();
            SetDateMenuButtonsUI(sender);
        }

        private void btnUlt30dias_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;
            btnOkCustomDate.Visible = false;
            LoadData();
            SetDateMenuButtonsUI(sender);
        }

        private void btnEstemes_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpEndDate.Value = DateTime.Now;
            btnOkCustomDate.Visible = false;
            LoadData();
            SetDateMenuButtonsUI(sender);
        }

        private void btnPersonalizado_Click(object sender, EventArgs e)
        {
            dtpStartDate.Enabled = true;
            dtpEndDate.Enabled = true;
            btnOkCustomDate.Visible = true;
            SetDateMenuButtonsUI(sender);
        }

        private void btnOkCustomDate_Click(object sender, EventArgs e)
        {
            LoadData();
            
        }

        private void frmDashboardNuevo_Load(object sender, EventArgs e)
        {
            dgvUnderstock.Columns[1].Width = 50;
            lblStartDate.Text = dtpStartDate.Text;
            lblEndDate.Text = dtpEndDate.Text;
        }

        private void lblStartDate_Click(object sender, EventArgs e)
        {
            if (currentButton == btnPersonalizado)
            {
                dtpStartDate.Select();
                SendKeys.Send("%{DOWN}");
            }
        }

        private void lblEndDate_Click(object sender, EventArgs e)
        {
            if (currentButton == btnPersonalizado)
            {
                dtpEndDate.Select();
                SendKeys.Send("%{DOWN}");
            }
        }

        private void dtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            lblStartDate.Text = dtpStartDate.Text;
        }

        private void dtpEndDate_ValueChanged(object sender, EventArgs e)
        {
            lblEndDate.Text = dtpEndDate.Text;
        }
    }
}
