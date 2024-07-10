using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
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
    public partial class frmProducto : Form
    {
        public frmProducto()
        {
            InitializeComponent();
        }

        private void cargarDatos()
        {
            dgvdata.Rows.Clear();
            //MOSTRAR TODOS LOS USUARIOS
            List<Producto> lista = new CN_Producto().Listar();

            foreach (Producto item in lista)
            {

                dgvdata.Rows.Add(new object[] {
                    "",
                    item.ID_Producto,
                    item.Codigo,
                    item.Nombre,
                    item.Descripcion,
                    item.oCategoria.ID_Categoria,
                    item.oCategoria.Descripcion,
                    item.Stock,
                    item.PrecioCatalogo,
                    item.DescuentoCompra,
                    item.PrecioCompra,
                    item.Estado == true ? 1 : 0 ,
                    item.Estado == true ? "Activo" : "No Activo"
                });
            }
        }

        private void frmProducto_Load(object sender, EventArgs e)
        {
            cboestado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboestado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboestado.DisplayMember = "Texto";
            cboestado.ValueMember = "Valor";
            cboestado.SelectedIndex = 0;


            List<Categoria> listacategoria = new CN_Categoria().Listar();

            foreach (Categoria item in listacategoria)
            {
                cbocategoria.Items.Add(new OpcionCombo() { Valor = item.ID_Categoria, Texto = item.Descripcion });
            }
            cbocategoria.DisplayMember = "Texto";
            cbocategoria.ValueMember = "Valor";
            cbocategoria.SelectedIndex = 0;


            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {

                if (columna.Visible == true && columna.Name != "btnseleccionar")
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;

            cargarDatos();
            txtcodigo.Select();
        }

        private void Limpiar()
        {

            txtindice.Text = "-1";
            txtid.Text = "0";
            txtcodigo.Text = "";
            txtnombre.Text = "";
            txtdescripcion.Text = "";
            txtstock.Text = "";
            txtpreciocatalogo.Text = "";
            txtdescuentocompra.Text = "";
            txtpreciocompra.Text = "";
            cbocategoria.SelectedIndex = 0;
            cboestado.SelectedIndex = 0;

            txtcodigo.Select();

        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            if (!string.IsNullOrEmpty(txtnombre.Text) && !string.IsNullOrEmpty(txtdescripcion.Text) && !string.IsNullOrEmpty(txtstock.Text) && !string.IsNullOrEmpty(txtpreciocatalogo.Text) && !string.IsNullOrEmpty(txtdescuentocompra.Text))
            {
                Producto obj = new Producto()
                {
                    ID_Producto = Convert.ToInt32(txtid.Text),
                    Codigo = txtcodigo.Text,
                    Nombre = txtnombre.Text,
                    Descripcion = txtdescripcion.Text,
                    Stock = Convert.ToInt32(txtstock.Text),
                    PrecioCatalogo = Convert.ToDecimal(txtpreciocatalogo.Text),
                    DescuentoCompra = Convert.ToDecimal(txtdescuentocompra.Text),
                    PrecioCompra = Convert.ToDecimal(txtpreciocompra.Text),
                    oCategoria = new Categoria() { ID_Categoria = Convert.ToInt32(((OpcionCombo)cbocategoria.SelectedItem).Valor) },
                    Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1 ? true : false
                };

                if (obj.ID_Producto == 0)
                {
                    int idgenerado = new CN_Producto().Registrar(obj, out mensaje);

                    if (idgenerado != 0)
                    {

                        dgvdata.Rows.Add(new object[] {
                        "",
                       idgenerado,
                       txtcodigo.Text,
                       txtnombre.Text,
                       txtdescripcion.Text,
                       txtstock.Text,
                       txtpreciocatalogo,
                       txtdescuentocompra.Text,
                       txtpreciocompra.Text,
                       ((OpcionCombo)cbocategoria.SelectedItem).Valor.ToString(),
                       ((OpcionCombo)cbocategoria.SelectedItem).Texto.ToString(),
                       "0",
                       "0.00",
                       "0.00",
                       ((OpcionCombo)cboestado.SelectedItem).Valor.ToString(),
                       ((OpcionCombo)cboestado.SelectedItem).Texto.ToString()
                    });

                        Limpiar();
                        MessageBox.Show("Producto registrado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cargarDatos();
                    }
                    else
                    {
                        MessageBox.Show(mensaje);
                    }


                }
                else
                {
                    bool resultado = new CN_Producto().Editar(obj, out mensaje);

                    if (resultado)
                    {
                        DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                        row.Cells["ID_Producto"].Value = txtid.Text;
                        row.Cells["Codigo"].Value = txtcodigo.Text;
                        row.Cells["Nombre"].Value = txtnombre.Text;
                        row.Cells["Descripcion"].Value = txtdescripcion.Text;
                        row.Cells["Stock"].Value = txtstock.Text;
                        row.Cells["PrecioCatalogo"].Value = txtpreciocatalogo.Text;
                        row.Cells["DescuentoCompra"].Value = txtdescuentocompra.Text;
                        row.Cells["PrecioCompra"].Value = txtpreciocompra.Text;
                        row.Cells["ID_Categoria"].Value = ((OpcionCombo)cbocategoria.SelectedItem).Valor.ToString();
                        row.Cells["DescripcionCategoria"].Value = ((OpcionCombo)cbocategoria.SelectedItem).Texto.ToString();
                        row.Cells["EstadoValor"].Value = ((OpcionCombo)cboestado.SelectedItem).Valor.ToString();
                        row.Cells["Estado"].Value = ((OpcionCombo)cboestado.SelectedItem).Texto.ToString();

                        Limpiar();
                        MessageBox.Show("Producto editado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cargarDatos();
                    }
                    else
                    {
                        MessageBox.Show(mensaje);
                    }
                }
            }
            else
            {
                MessageBox.Show("Se deben ingresar los campos obligatorios (*)", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtid.Text) != 0)
            {
                if (MessageBox.Show("¿Desea eliminar el producto?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    string mensaje = string.Empty;
                    Producto obj = new Producto()
                    {
                        ID_Producto = Convert.ToInt32(txtid.Text)
                    };

                    bool respuesta = new CN_Producto().Eliminar(obj, out mensaje);

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtindice.Text));
                        Limpiar();
                        MessageBox.Show("Producto eliminado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cargarDatos();
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }
            }
            else
            {
                MessageBox.Show("Debe elegir un producto.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 0)
            {

                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.check20.Width;
                var h = Properties.Resources.check20.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.check20, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btnseleccionar")
            {

                int indice = e.RowIndex;

                if (indice >= 0)
                {

                    txtindice.Text = indice.ToString();
                    txtid.Text = dgvdata.Rows[indice].Cells["ID_Producto"].Value.ToString();

                    txtcodigo.Text = dgvdata.Rows[indice].Cells["Codigo"].Value.ToString();
                    txtnombre.Text = dgvdata.Rows[indice].Cells["Nombre"].Value.ToString();
                    txtdescripcion.Text = dgvdata.Rows[indice].Cells["Descripcion"].Value.ToString();
                    txtstock.Text = dgvdata.Rows[indice].Cells["Stock"].Value.ToString();
                    txtpreciocatalogo.Text = dgvdata.Rows[indice].Cells["PrecioCatalogo"].Value.ToString();
                    txtdescuentocompra.Text = dgvdata.Rows[indice].Cells["DescuentoCompra"].Value.ToString();
                    txtpreciocompra.Text = dgvdata.Rows[indice].Cells["PrecioCompra"].Value.ToString();


                    foreach (OpcionCombo oc in cbocategoria.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["ID_Categoria"].Value))
                        {
                            int indice_combo = cbocategoria.Items.IndexOf(oc);
                            cbocategoria.SelectedIndex = indice_combo;
                            break;
                        }
                    }


                    foreach (OpcionCombo oc in cboestado.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["EstadoValor"].Value))
                        {
                            int indice_combo = cboestado.Items.IndexOf(oc);
                            cboestado.SelectedIndex = indice_combo;
                            break;
                        }
                    }


                }


            }
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {

                    if (row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtbusqueda.Text.Trim().ToUpper()))
                        row.Visible = true;
                    else
                        row.Visible = false;
                }
            }
        }

        private void btnlimpiarbuscador_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }
        }

        private void btnexportar_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("No hay datos para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable();

                foreach (DataGridViewColumn columna in dgvdata.Columns)
                {
                    if (columna.HeaderText != "" && columna.Visible)
                        dt.Columns.Add(columna.HeaderText, typeof(string));
                }

                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (row.Visible)
                        dt.Rows.Add(new object[] {
                            row.Cells[2].Value.ToString(),
                            row.Cells[3].Value.ToString(),
                            row.Cells[4].Value.ToString(),
                            row.Cells[6].Value.ToString(),
                            row.Cells[7].Value.ToString(),
                            row.Cells[8].Value.ToString(),
                            row.Cells[9].Value.ToString(),
                            row.Cells[10].Value.ToString(),
                            row.Cells[12].Value.ToString(),

                        });
                }

                SaveFileDialog savefile = new SaveFileDialog();
                savefile.FileName = string.Format("ReporteProducto_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                savefile.Filter = "Excel Files | *.xlsx";

                if (savefile.ShowDialog() == DialogResult.OK)
                {

                    try
                    {
                        XLWorkbook wb = new XLWorkbook();
                        var hoja = wb.Worksheets.Add(dt, "Informe");
                        hoja.ColumnsUsed().AdjustToContents();
                        wb.SaveAs(savefile.FileName);
                        MessageBox.Show("Reporte Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch
                    {
                        MessageBox.Show("Error al generar reporte", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }

            }
        }

        private void CalcularPrecioCompra()
        {
            decimal dsc, pctg, pcmp = 0;
            
            if (!string.IsNullOrEmpty(txtpreciocatalogo.Text) && !string.IsNullOrEmpty(txtdescuentocompra.Text))
            {
                pctg = Convert.ToDecimal(txtpreciocatalogo.Text);
                dsc = Convert.ToDecimal(txtdescuentocompra.Text);

                if (dsc > 0)
                {
                    pcmp = pctg - ((pctg * dsc) / 100);
                }
                else
                {
                    pcmp = 0;
                }
                txtpreciocompra.Text = pcmp.ToString();
            }
            else
            {
                MessageBox.Show("Ingrese el precio de catalogo y/ descuento de compra", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void txtdescuentocompra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                CalcularPrecioCompra();
            }
        }

        private void txtstock_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtstock.Text.Trim().Length == 0)
                {
                    e.Handled = true;
                }
                else
                {
                    if (Char.IsControl(e.KeyChar))
                    {
                        e.Handled = false;

                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void txtpreciocatalogo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // solo 1 punto decimal
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void txtdescuentocompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // solo 1 punto decimal
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void txtdescuentocompra_TextChanged(object sender, EventArgs e)
        {
            if (txtdescuentocompra.Text != "")
            {
                CalcularPrecioCompra();
            }
            else
            {
                txtpreciocompra.Text = "";
            }
        }
    }
}
