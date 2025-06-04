using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BringUp_Control
{
    public partial class PRBS_Form : Form
    {
        public PRBS_Form()
        {
            InitializeComponent();
        }

        private void Cmd_Quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
