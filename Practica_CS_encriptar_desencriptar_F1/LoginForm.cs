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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("inicio correcto");
             
            this.Hide(); // OCULTAMOS EL LOGIN
            Form1 main = new Form1();   //CREAMOS UN NUEVO FORM 
            main.Show(); //LO MOSTRAMOS 
        }
    }
}
