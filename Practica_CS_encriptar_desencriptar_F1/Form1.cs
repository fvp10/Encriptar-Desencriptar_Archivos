using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Practica_CS_encriptar_desencriptar_F1
{
    public partial class Form1 : Form

    {
        private int posicionVertical = 10; // Inicializa la posición vertical
        private Dictionary<System.Windows.Forms.Button, Panel> botonesYFilas = new Dictionary<System.Windows.Forms.Button, Panel>();

        public Form1()
        {
            InitializeComponent();
        }

        private void desencriptar__Click(object sender, EventArgs e)
        {
            MessageBox.Show("Proceso de desencriptar");
        }

        private void encriptar__Click(object sender, EventArgs e)
        {
            CrearNuevaFila();
        }

        private void btn_selc_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Busca tu archivo a encriptar";
            openFileDialog1.ShowDialog();
            string texto = openFileDialog1.FileName;

            if (File.Exists(openFileDialog1.FileName))
            {         
                TextReader leer = new StreamReader(texto);        
                leer.Close();
            }
            txt1.Text = Path.GetFileName(texto);
        }

        private void CrearNuevaFila()
        {
            //AQUI HAY QUE LLAMAR A LA FUNCION DE ENCRIPTAR ANTES DE QUE CREE LA FILA 
            //PARA QUE CUANDO SE CREE LA FILA SALGA YA ENCRIPTADO



            // CREACION DE LAS FILAS
            Label txtCifrado = new Label();
            txtCifrado.Text = txt1.Text; //ESTO ES TEMPORAL YA QUE HABRÁ QUE PONER EL ARCHIVO CIFRADO
            txtCifrado.Width = 150;
            txtCifrado.Height = 30;
            
            System.Windows.Forms.Button btnDesencriptar = new System.Windows.Forms.Button();
            btnDesencriptar.Text = "Desencriptar";
            btnDesencriptar.Width = 80;
            btnDesencriptar.Height = 30;

            System.Windows.Forms.Button btnBorrar = new System.Windows.Forms.Button();
            btnBorrar.Text = "Borrar";
            btnBorrar.Width = 80;
            btnBorrar.Height = 30;

            // Crea un nuevo contenedor para la fila (por ejemplo, un panel)
            Panel nuevaFila = new Panel();
            nuevaFila.Width = panelContenedor.Width;
            nuevaFila.Height = 30;

            // Establece las ubicaciones de los controles dentro de la fila
            txtCifrado.Location = new System.Drawing.Point(0, 0);
            btnDesencriptar.Location = new System.Drawing.Point(txtCifrado.Right + 5, 0);
            btnBorrar.Location = new System.Drawing.Point(btnDesencriptar.Right + 5, 0);

            // Agrega los controles a la fila
            nuevaFila.Controls.Add(txtCifrado);
            nuevaFila.Controls.Add(btnDesencriptar);
            nuevaFila.Controls.Add(btnBorrar);

            // Establece la posición de la fila
            nuevaFila.Location = new System.Drawing.Point(10, posicionVertical);

            // Incrementa la posición vertical para la próxima fila
            posicionVertical += nuevaFila.Height + 5; // Puedes ajustar el espacio entre filas

            // Asigna un manejador de eventos a los botones de la fila
            btnDesencriptar.Click += desencriptar__Click;
            btnBorrar.Click += (sender ,e) => EliminarFila(nuevaFila);

            // Agrega la nueva fila al panel de contenedor
            panelContenedor.Controls.Add(nuevaFila);

            txt1.Text = "";
        }
        private void EliminarFila(Panel fila)
        {
            // Elimina la fila del panel de contenedor y del diccionario
            panelContenedor.Controls.Remove(fila);
            // Busca y elimina la entrada correspondiente en el diccionario
            foreach (var kvp in botonesYFilas)
            {
                if (kvp.Value == fila)
                {
                    botonesYFilas.Remove(kvp.Key);
                    break;
                }
            }
        }
        private void panelContenedor_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
