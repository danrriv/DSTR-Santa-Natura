using CapaEntidad;
using CapaNegocio;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.IO;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmDetalleCompra : Form
    {
        public frmDetalleCompra()
        {
            InitializeComponent();
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            Compra oCompra = new CN_Compra().ObtenerCompra(txtndocumento.Text);
            if (oCompra.ID_Compra != 0)
            {
                txtndocumento.Text = oCompra.NDocumento;

                txtfecha.Text = oCompra.FechaRegistro;
                txtnserie.Text = oCompra.Serie;
                txtusuario.Text = oCompra.oUsuario.Nombre;

                dgvdata.Rows.Clear();
                foreach (Detalle_Compra dc in oCompra.oDetalleCompra)
                {
                    dgvdata.Rows.Add(new object[] { dc.oProducto.Nombre, dc.PrecioCompra, dc.Cantidad, dc.MontoTotal });
                }
                txtmontototal.Text = "S/." + oCompra.Total.ToString("0.00");
            }
            else
            {
                MessageBox.Show("No se encontro la compra asegurese\nde que los datos sean correctos", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            txtfecha.Text = "";
            txtmontototal.Text = "S/.0.00";
            txtnserie.Text = "";
            txtusuario.Text = "";
            dgvdata.Rows.Clear();
        }

        private void btnpdf_Click(object sender, EventArgs e)
        {
            if (txtfecha.Text == "")
            {
                MessageBox.Show("No se encontraron los resultados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string Texto_html = Properties.Resources.PlantillaCompra.ToString();
            Negocio odatos = new CN_Negocio().ObtenerDatos();

            Texto_html = Texto_html.Replace("@nombrenegocio", odatos.Nombre.ToUpper());
            Texto_html = Texto_html.Replace("@docnegocio", odatos.RUC);
            Texto_html = Texto_html.Replace("@direcnegocio", odatos.Direccion);

            Texto_html = Texto_html.Replace("@tipodocumento", "Factura");
            Texto_html = Texto_html.Replace("@numerodocumento", txtndocumento.Text);

            Texto_html = Texto_html.Replace("@nombreproveedor", "Santa Natura");
            Texto_html = Texto_html.Replace("@fecharegistro", txtfecha.Text);
            Texto_html = Texto_html.Replace("@usuarioregistro", txtusuario.Text);

            string filas = string.Empty;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["Producto"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["PrecioCompra"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Subtotal"].Value.ToString() + "</td>";
                filas += "</tr>";
            }
            Texto_html = Texto_html.Replace("@filas", filas);
            Texto_html = Texto_html.Replace("@montototal", txtmontototal.Text);

            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Compra_{0}.pdf", txtndocumento.Text);
            savefile.Filter = "Pdf Files|*.pdf";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                {

                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);

                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    bool obtenido = true;
                    byte[] byteImage = new CN_Negocio().ObtenerLogo(out obtenido);

                    if (obtenido)
                    {
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(byteImage);
                        img.ScaleToFit(60, 60);
                        img.Alignment = iTextSharp.text.Image.UNDERLYING;
                        img.SetAbsolutePosition(pdfDoc.Left, pdfDoc.GetTop(51));
                        pdfDoc.Add(img);
                    }

                    using (StringReader sr = new StringReader(Texto_html))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    }

                    pdfDoc.Close();
                    stream.Close();
                    MessageBox.Show("Documento Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
