using AxAcroPDFLib;
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
    public partial class frmAyuda : Form
    {
        public frmAyuda()
        {
            InitializeComponent();
        }

        private void frmAyuda_Load(object sender, EventArgs e)
        {
            string temporal = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "GUIA.pdf");
            System.IO.File.WriteAllBytes(temporal, Properties.Resources.GUIA);
            adobepdf.src = temporal;
        }
    }
}
