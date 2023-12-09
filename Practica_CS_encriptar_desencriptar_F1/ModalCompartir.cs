using Practica_CS_encriptar_desencriptar_F1.Modelos;
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
        public List<Usuario> SelectedUsuarios { get; private set; }

        public ModalCompartir(List<Usuario> users)
        {
            InitializeComponent();
            SelectedUsuarios = new List<Usuario>();
            PopulateUsers(users);
        }

        private void PopulateUsers(List<Usuario> users)
        {
            foreach (var user in users)
            {
                var checkBox = new CheckBox
                {
                    Text = user.NombreUsuario,
                    AutoSize = true,
                    Tag = user // Almacenar el objeto Usuario completo en la propiedad Tag
                };
                flowLayoutPanel1.Controls.Add(checkBox);
            }
        }

        private void ModalCompartir_Load(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            foreach (CheckBox checkBox in flowLayoutPanel1.Controls)
            {
                if (checkBox.Checked)
                {
                    SelectedUsuarios.Add((Usuario)checkBox.Tag); 
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

