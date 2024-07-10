using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
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
    public partial class frmDevolucion : Form
    {
        public frmDevolucion()
        {
            InitializeComponent();
        }

        private void Limpiar()
        {
            txtindice.Text = "-1";
            txtid.Text = "0";
            txtdescripcion.Text = "";
            txtcantidad.Text = "";
            cbproducto.SelectedIndex = 0;
            cboestado.SelectedIndex = 1;
            txtdescripcion.Select();
        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            if (!string.IsNullOrEmpty(txtdescripcion.Text) && !string.IsNullOrEmpty(txtcantidad.Text))
            {
                Devolucion objdevolucion = new Devolucion()
                {
                    ID_Devolucion = Convert.ToInt32(txtid.Text),
                    Descripcion = txtdescripcion.Text,
                    Cantidad = Convert.ToInt32(txtcantidad.Text),
                    oProducto = new Producto() { ID_Producto = Convert.ToInt32(((OpcionCombo)cbproducto.SelectedItem).Valor) },
                    Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1 ? true : false
                };


                if (objdevolucion.ID_Devolucion == 0)
                {
                    int resultado = new CN_Devolucion().Registrar(objdevolucion, out mensaje);

                    if (resultado != 0)
                    {
                        dgvdata.Rows.Add(new object[] {"",((OpcionCombo)cbproducto.SelectedItem).Valor.ToString(),
                        ((OpcionCombo)cbproducto.SelectedItem).Texto.ToString(),
                        resultado,txtdescripcion.Text,txtcantidad.Text,
                        ((OpcionCombo)cboestado.SelectedItem).Valor.ToString(),
                        ((OpcionCombo)cboestado.SelectedItem).Texto.ToString(),
                        txtfecha.Text
                    });
                        Limpiar();
                        MessageBox.Show("Devolución registrada", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        MessageBox.Show(mensaje);
                    }

                }
                else
                {
                    bool resultado = new CN_Devolucion().Editar(objdevolucion, out mensaje);
                    if (resultado)
                    {
                        DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                        row.Cells["ID_Producto"].Value = ((OpcionCombo)cbproducto.SelectedItem).Valor.ToString();
                        row.Cells["Producto"].Value = ((OpcionCombo)cbproducto.SelectedItem).Texto.ToString();
                        row.Cells["ID_Devolucion"].Value = txtid.Text;
                        row.Cells["Descripcion"].Value = txtdescripcion.Text;
                        row.Cells["Cantidad"].Value = txtcantidad.Text;
                        row.Cells["EstadoValor"].Value = ((OpcionCombo)cboestado.SelectedItem).Valor.ToString();
                        row.Cells["Estado"].Value = ((OpcionCombo)cboestado.SelectedItem).Texto.ToString();
                        row.Cells["Fecha"].Value = txtfecha.Text;
                        Limpiar();
                        MessageBox.Show("Devolución editada", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void frmDevolucion_Load(object sender, EventArgs e)
        {
            cboestado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Resuelto" });
            cboestado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "Pendiente" });
            cboestado.DisplayMember = "Texto";
            cboestado.ValueMember = "Valor";
            cboestado.SelectedIndex = 1;
            txtfecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            List<Producto> productos = new CN_Producto().Listar();
            foreach(Producto item in productos)
            {
                cbproducto.Items.Add(new OpcionCombo() { Valor = item.ID_Producto, Texto = item.Nombre });
            }
            cbproducto.DisplayMember = "Texto";
            cbproducto.ValueMember = "Valor";
            cbproducto.SelectedIndex = 0;

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
            CargarTabla();

        }


        private void CargarTabla()
        {
            dgvdata.Rows.Clear();    
            List<Devolucion> listadevolucion = new CN_Devolucion().Listar();
            foreach (Devolucion item in listadevolucion)
            {
                dgvdata.Rows.Add(new object[] {"",
                    item.oProducto.ID_Producto,
                    item.oProducto.Nombre,
                    item.ID_Devolucion,
                    item.Descripcion,
                    item.Cantidad,
                    item.Estado == true ? 1:0,
                    item.Estado == true ? "Resuelto" : "Pendiente",
                    item.FechaRegistro
                });
                txtdescripcion.Select();
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
                if (MessageBox.Show("¿Desea eliminar la devolución", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    string mensaje = string.Empty;
                    Devolucion objdevolucion = new Devolucion()
                    {
                        ID_Devolucion = Convert.ToInt32(txtid.Text)
                    };

                    bool respuesta = new CN_Devolucion().Eliminar(objdevolucion, out mensaje);

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtindice.Text));
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }
                Limpiar();
            }
            else
            {
                MessageBox.Show("Debe elegir una devolución.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void btneliminarcompletado_Click(object sender, EventArgs e)
        {
            //if (Convert.ToInt32(txtid.Text) != 0)
            //{
                if (MessageBox.Show("¿Desea eliminar los registros resueltos?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Devolucion objdevolucion = new Devolucion();

                    bool respuesta = new CN_Devolucion().Eliminarcompleto(objdevolucion, out mensaje);

                    if (respuesta)
                    {
                        CargarTabla();
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }
                Limpiar();
            //}

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

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btnseleccionar")
            {

                int indice = e.RowIndex;

                if (indice >= 0)
                {

                    txtindice.Text = indice.ToString();
                    txtid.Text = dgvdata.Rows[indice].Cells["ID_Devolucion"].Value.ToString();
                    txtdescripcion.Text = dgvdata.Rows[indice].Cells["Descripcion"].Value.ToString();
                    txtcantidad.Text = dgvdata.Rows[indice].Cells["Cantidad"].Value.ToString();

                    foreach (OpcionCombo oc in cbproducto.Items)
                    {

                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["ID_Producto"].Value))
                        {
                            int indice_combo = cbproducto.Items.IndexOf(oc);
                            cbproducto.SelectedIndex = indice_combo;
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

        private void txtcantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtcantidad.Text.Trim().Length == 0)
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
    }
}
