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
    public partial class RegistroForm : Form
    {
        public RegistroForm()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide(); // OCULTAMOS EL LOGIN
            LoginForm login = new LoginForm();   //CREAMOS UN NUEVO FORM 
            login.Show(); //LO MOSTRAMOS 
        }
    }
}
