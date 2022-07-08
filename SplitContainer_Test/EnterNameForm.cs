using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CloudProjectClient
{
    public partial class EnterNameForm : Form
    {
        public EnterNameForm()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (folderNameTextBox.Text != string.Empty)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Пустые имена недопустимы!");
            }

        }
    }
}
