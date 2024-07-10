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
    public partial class frmClientes : Form
    {
        public frmClientes()
        {
            InitializeComponent();
        }

        private void CargarDatos()
        {
            dgvdata.Rows.Clear();
            //MOSTRAR TODOS LOS CLIENTES
            List<Cliente> lista = new CN_Cliente().Listar();

            foreach (Cliente item in lista)
            {
                dgvdata.Rows.Add(new object[] {"",item.ID_Cliente,
                    item.Tipo,
                    item.NombreCompleto,
                    item.TipoDocumento,
                    item.NDocumento,
                    item.Direccion,
                    item.Telefono,
                    item.Estado == true ? 1 : 0 ,
                    item.Estado == true ? "Activo" : "No Activo"
                });
            }
        }

        private void frmClientes_Load(object sender, EventArgs e)
        {
            //COMBOBOX DE TIPO
            cbotipocliente.Items.Add(new OpcionCombo() { Valor = "Socio", Texto = "Socio" });
            cbotipocliente.Items.Add(new OpcionCombo() { Valor = "Publico", Texto = "Publico" });
            cbotipocliente.DisplayMember = "Texto";
            cbotipocliente.ValueMember = "Valor";
            cbotipocliente.SelectedIndex = 0;
            //COMBOBOX DE TIPO DE DOCUMENTO
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "RUC", Texto = "RUC" });
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "DNI", Texto = "DNI" });
            cbotipodocumento.DisplayMember = "Texto";
            cbotipodocumento.ValueMember = "Valor";
            cbotipodocumento.SelectedIndex = 0;
            //COMBOBOX DE ESTADO
            cboestado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboestado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboestado.DisplayMember = "Texto";
            cboestado.ValueMember = "Valor";
            cboestado.SelectedIndex = 0;


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

            CargarDatos();
            txtnombrecompleto.Select();
        }

        private void Limpiar()
        {
            txtindice.Text = "-1";
            txtid.Text = "0";
            txtdocumento.Text = "";
            txtnombrecompleto.Text = "";
            txtdireccion.Text = "";
            txttelefono.Text = "";
            cbotipocliente.SelectedIndex = 0;
            cbotipodocumento.SelectedIndex = 0;
            cboestado.SelectedIndex = 0;
            txtnombrecompleto.Select();
        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            Cliente obj = new Cliente()
            {
                ID_Cliente = Convert.ToInt32(txtid.Text),
                Tipo = ((OpcionCombo)cbotipocliente.SelectedItem).Valor.ToString(),
                NombreCompleto = txtnombrecompleto.Text,
                TipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Valor.ToString(),
                NDocumento = txtdocumento.Text,
                Direccion = txtdireccion.Text,
                Telefono = txttelefono.Text,
                Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1 ? true : false
            };

            if (obj.ID_Cliente == 0)
            {
                int idgenerado = new CN_Cliente().Registrar(obj, out mensaje);

                if (idgenerado != 0)
                {

                    dgvdata.Rows.Add(new object[] {"",idgenerado,
                        ((OpcionCombo)cbotipocliente.SelectedItem).Valor.ToString(),
                        //((OpcionCombo)cbotipocliente.SelectedItem).Texto.ToString(),
                        txtnombrecompleto.Text,
                        ((OpcionCombo)cbotipodocumento.SelectedItem).Valor.ToString(),
                        //((OpcionCombo)cbotipodocumento.SelectedItem).Texto.ToString(),
                        txtdocumento.Text,txtdireccion.Text,txttelefono.Text,
                        ((OpcionCombo)cboestado.SelectedItem).Valor.ToString(),
                        ((OpcionCombo)cboestado.SelectedItem).Texto.ToString()
                    });

                    Limpiar();
                    MessageBox.Show("Cliente registrado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //CargarDatos();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }


            }
            else
            {
                bool resultado = new CN_Cliente().Editar(obj, out mensaje);

                if (resultado)
                {
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                    row.Cells["ID_Cliente"].Value = txtid.Text;
                    //row.Cells["TipoValor"].Value = ((OpcionCombo)cbotipocliente.SelectedItem).Valor.ToString();
                    row.Cells["Tipo"].Value = ((OpcionCombo)cbotipocliente.SelectedItem).Texto.ToString();
                    row.Cells["NombreCompleto"].Value = txtnombrecompleto.Text;
                    //row.Cells["TipoDocumentoValor"].Value = ((OpcionCombo)cbotipodocumento.SelectedItem).Valor.ToString();
                    row.Cells["TipoDocumento"].Value = ((OpcionCombo)cbotipodocumento.SelectedItem).Texto.ToString();
                    row.Cells["NDocumento"].Value = txtdocumento.Text;
                    row.Cells["Direccion"].Value = txtdireccion.Text;
                    row.Cells["Telefono"].Value = txttelefono.Text;
                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cboestado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cboestado.SelectedItem).Texto.ToString();
                    Limpiar();
                    MessageBox.Show("Cliente editado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //CargarDatos();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
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
                if (MessageBox.Show("¿Desea eliminar el cliente?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    string mensaje = string.Empty;
                    Cliente obj = new Cliente()
                    {
                        ID_Cliente = Convert.ToInt32(txtid.Text)
                    };

                    bool respuesta = new CN_Cliente().Eliminar(obj, out mensaje);

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtindice.Text));
                        Limpiar();
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }
            }
            else
            {
                MessageBox.Show("Debe elegir un cliente.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    txtid.Text = dgvdata.Rows[indice].Cells["ID_Cliente"].Value.ToString();
                    //cbotipocliente.SelectedText = dgvdata.Rows[indice].Cells["Tipo"].Value.ToString();
                    txtnombrecompleto.Text = dgvdata.Rows[indice].Cells["NombreCompleto"].Value.ToString();
                    //cbotipodocumento.SelectedText = dgvdata.Rows[indice].Cells["TipoDocumento"].Value.ToString();
                    txtdocumento.Text = dgvdata.Rows[indice].Cells["NDocumento"].Value.ToString();
                    txtdireccion.Text = dgvdata.Rows[indice].Cells["Direccion"].Value.ToString();
                    txttelefono.Text = dgvdata.Rows[indice].Cells["Telefono"].Value.ToString();
                    
                    foreach (OpcionCombo tc in cbotipocliente.Items)
                    {
                        if (dgvdata.Rows[indice].Cells["Tipo"].Value.ToString() == "Socio")
                        {
                            cbotipocliente.SelectedIndex = 0;
                        }
                        else
                        {
                            cbotipocliente.SelectedIndex = 1;
                        }
                    }

                    foreach (OpcionCombo td in cbotipodocumento.Items)
                    {
                        if (dgvdata.Rows[indice].Cells["TipoDocumento"].Value.ToString() == "RUC")
                        {
                            cbotipodocumento.SelectedIndex = 0;
                        }
                        else
                        {
                            cbotipodocumento.SelectedIndex = 1;
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
    }
}
