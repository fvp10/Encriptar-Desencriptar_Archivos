using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Practica_CS_encriptar_desencriptar_F1
{
    public partial class ModalCompartir : Form
    {
        public List<string> SelectedUsers { get; private set; }

        public ModalCompartir(List<string> users)
        {
            InitializeComponent();
            SelectedUsers = new List<string>();
            PopulateUsers(users);
        }

        private void PopulateUsers(List<string> users)
        {
            foreach (var user in users)
            {
                var checkBox = new CheckBox
                {
                    Text = user,
                    AutoSize = true
                };
                flowLayoutPanel1.Controls.Add(checkBox); // Asumiendo que tienes un FlowLayoutPanel
            }
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            foreach (CheckBox checkBox in flowLayoutPanel1.Controls)
            {
                if (checkBox.Checked)
                {
                    SelectedUsers.Add(checkBox.Text);
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ModalCompartir_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
