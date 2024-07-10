using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
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
    public partial class frmCompras : Form
    {
        private Usuario _Usuario;
        public frmCompras(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
        }

        private void frmCompras_Load(object sender, EventArgs e)
        {
            txtfecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtidproducto.Text = "0";
            txtnserie.Select();
        }

        private void btnbuscarproducto_Click(object sender, EventArgs e)
        {
            using (var mod = new mdProducto())
            {
                var result = mod.ShowDialog();

                if (result == DialogResult.OK)
                {
                    txtidproducto.Text = mod._Producto.ID_Producto.ToString();
                    txtcodproducto.Text = mod._Producto.Codigo.ToString();
                    txtproducto.Text = mod._Producto.Nombre.ToString();
                    txtpreciocompra.Text = mod._Producto.PrecioCompra.ToString();
                    txtprecioventa.Text = mod._Producto.PrecioCatalogo.ToString();
                    txtcantidad.Focus();

                }
            }
        }

        private void btnagregarproducto_Click(object sender, EventArgs e)
        {
            decimal pcompra = 0;
            decimal pventa = 0;
            bool pEx = false;


            if (int.Parse(txtidproducto.Text) == 0)
            {
                MessageBox.Show("Debe seleccionar un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!decimal.TryParse(txtpreciocompra.Text, out pcompra))
            {
                MessageBox.Show("Campo PrecioCompra incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtpreciocompra.Select();
                return;
            }
            if (!decimal.TryParse(txtprecioventa.Text, out pventa))
            {
                MessageBox.Show("Campo PrecioVenta incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtprecioventa.Select();
                return;
            }

            foreach (DataGridViewRow fila in dgvdata.Rows)
            {
                if (fila.Cells["IdProducto"].Value.ToString() == txtidproducto.Text)
                {
                    pEx = true;
                    break;
                }
            }
            if (!pEx)
            {
                dgvdata.Rows.Add(new object[]
                {
                    txtidproducto.Text,
                    txtproducto.Text,
                    pcompra.ToString("0.0"),
                    pventa.ToString("0.0"),
                    txtcantidad.Value.ToString(),
                    (txtcantidad.Value * pcompra).ToString("0.0")
                });
                calcularTotal();
                limpiarProducto();
            }
        }

        private void limpiarProducto()
        {
            txtidproducto.Text = "0";
            txtcodproducto.Text = "";
            txtidproducto.BackColor = Color.White;
            txtproducto.Text = "";
            txtprecioventa.Text = "";
            txtpreciocompra.Text = "";
            txtcantidad.Value = 1;
        }

        private void calcularTotal()
        {
            decimal t = 0;
            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                    t += Convert.ToDecimal(row.Cells["Subtotal"].Value.ToString());
            }
            txttotalpagar.Text = t.ToString("0.0");
            txtigv.Text = Convert.ToString(Convert.ToDouble(t) * 0.18);
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == 6)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.delete25.Width;
                var h = Properties.Resources.delete25.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.delete25, new Rectangle(x, y, w, h));
                e.Handled = true;

            }
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btneliminar")
            {
                int indice = e.RowIndex;
                if (indice >= 0)
                {
                    dgvdata.Rows.RemoveAt(indice);
                    calcularTotal();
                }
            }
        }

        private void btnregistrar_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("Debe ingresar al menos un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (txtndocumento.Text == "")
            {
                MessageBox.Show("Debe ingresar un número de documento", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DataTable detalle_compra = new DataTable();

            detalle_compra.Columns.Add("IdProducto", typeof(int));
            detalle_compra.Columns.Add("PrecioCompra", typeof(decimal));
            detalle_compra.Columns.Add("Cantidad", typeof(int));
            detalle_compra.Columns.Add("Subtotal", typeof(decimal));

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                detalle_compra.Rows.Add(
                    new object[]
                    {
                        Convert.ToInt32(row.Cells["IdProducto"].Value.ToString()),
                        row.Cells["PrecioCompra"].Value.ToString(),
                        row.Cells["Cantidad"].Value.ToString(),
                        row.Cells["Subtotal"].Value.ToString(),
                    });
            }

            Compra nCompra = new Compra()
            {
                oUsuario = new Usuario() { ID_Usuario = _Usuario.ID_Usuario },
                Serie = txtnserie.Text,
                NDocumento = txtndocumento.Text,
                Total = Convert.ToDecimal(txttotalpagar.Text),
                Igv = Convert.ToDecimal(txtigv.Text),
                FechaRegistro = txtfecha.Text
            };
            string mensaje = string.Empty;
            bool respuesta = new CN_Compra().Registrar(nCompra, detalle_compra, out mensaje);

            if (respuesta)
            {
                try
                {
                    var result = MessageBox.Show("Compra registrada con éxito!\n\n¿Desea copiar el número de documento?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        Clipboard.SetText(txtndocumento.Text);
                    } 
                    MessageBox.Show("Texto copiado al portapapeles.","Copiado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception err)
                {
                    MessageBox.Show("Error al copiar texto al portapapeles: " +Environment.NewLine + err.Message, "Error al copiar",MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                txtidproducto.Text = "0";
                txtnserie.Text = "";
                txtndocumento.Text = "";
                dgvdata.Rows.Clear();
                calcularTotal();
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtpreciocompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtpreciocompra.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
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

        private void txtcodproducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {

                Producto oProducto = new CN_Producto().Listar().Where(p => p.Codigo == txtcodproducto.Text && p.Estado == true).FirstOrDefault();

                if (oProducto != null)
                {
                    txtcodproducto.BackColor = Color.Honeydew;
                    txtidproducto.Text = oProducto.ID_Producto.ToString();
                    txtproducto.Text = oProducto.Nombre;
                    txtpreciocompra.Select();
                }
                else
                {
                    txtcodproducto.BackColor = Color.MistyRose;
                    txtidproducto.Text = "0";
                    txtproducto.Text = "";
                }


            }
        }

        private void txtprecioventa_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtpreciocompra.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
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
    }
}
