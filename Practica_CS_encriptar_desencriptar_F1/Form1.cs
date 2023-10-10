using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace Practica_CS_encriptar_desencriptar_F1
{
    public partial class Form1 : Form

    {
        private int posicionVertical = 10; // Inicializa la posición vertical
        private Dictionary<System.Windows.Forms.Button, Panel> botonesYFilas = new Dictionary<System.Windows.Forms.Button, Panel>();
        private string rutaGuardado = "C:/CSarchivosENC";
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
            MetodoDeEncriptado();
        }

        private void MetodoDeEncriptado()
        {
            // Verificar si se ha seleccionado un archivo
            if (string.IsNullOrEmpty(txt1.Text))
            {
                MessageBox.Show("Por favor, seleccione un archivo para encriptar.");
                return;
            }

            // Obtener la ruta del archivo seleccionado
            string archivoOriginal = openFileDialog1.FileName;

            // Generar una clave aleatoria
            byte[] clave = GenerarClaveAleatoria();

            // Generar un IV aleatorio
            byte[] iv = GenerarIVAleatorio();

            string nombreArchivo = Path.GetFileName(archivoOriginal);

            // Crear una instancia de AES con la clave y el IV generados
            using (Aes objetoAes = Aes.Create())
            {
                objetoAes.Key = clave;
                objetoAes.IV = iv;
                objetoAes.Mode = CipherMode.CBC; // Modo CBC

                //Escribe la clave en otro archivo a parte
                string archivoClave = Path.Combine(rutaGuardado, nombreArchivo + "_clave.txt");
                File.WriteAllBytes(archivoClave, clave);
                //Escribe elIV en otro archivo a parte
                string archivoIV = Path.Combine(rutaGuardado, nombreArchivo + "_IV.txt");
                File.WriteAllBytes(archivoIV, iv);

                // Crear un flujo de cifrado
                //el obtejo de ICryptoTransform es la transformación de archivo desencriptado a encriptado y viceversa
                //se inicializa con el resultado del método "CreateEncryptor()" que es la interfaz que define la funcionalidad básica para transformar datos(cifrar/descifrar).
                using (ICryptoTransform cifrado = objetoAes.CreateEncryptor())
                {
                    // Ruta donde se guardará el archivo encriptado
                    string archivoEncriptado = Path.Combine(rutaGuardado, nombreArchivo + ".enc");

                    // Crear un flujo de escritura para el archivo encriptado
                    //archivoenc representa el archivo encriptado en el que estamos escribiendo datos
                    //FileMode.Create es un modo para abrir archivos, el cual crea un nuevo archivo si no existe o lo sobrescribe
                    using (FileStream archivoenc = new FileStream(archivoEncriptado, FileMode.Create))
                    {
                        // Escribir el IV en el archivo encriptado (para uso posterior durante la desencriptación)
                        //archivoenc representa el archivo encriptado en el que estamos escribiendo datos
                        //el 0 indica desde donde comienza a leer los bites
                        archivoenc.Write(iv, 0, iv.Length);

                        // Crear un flujo de lectura para el archivo original con FileMode.Open
                        //Este flujo de lectura se utilizará para leer los datos del archivo original
                        //y luego cifrarlos antes de escribirlos en el archivo encriptado.
                        using (FileStream fsIn = new FileStream(archivoOriginal, FileMode.Open))
                        {
                            //Crear un flujo de cifrado para cifrar los datos
                            using (CryptoStream cs = new CryptoStream(archivoenc, cifrado, CryptoStreamMode.Write))
                            {
                                // Leer el archivo original y escribir los datos cifrados en el archivo encriptado
                                int data;
                                while ((data = fsIn.ReadByte()) != -1) // va byte a byte escribiendo el cifrado correspondiente, si llega al final devuelve -1 y para el bucle
                                {
                                    cs.WriteByte((byte)data);
                                }
                            }
                        }
                    }
                }
            }

            // Notificar al usuario que la encriptación se ha completado
            MessageBox.Show("Archivo encriptado y guardado en: " + rutaGuardado);

            // Crear una nueva fila en la interfaz gráfica
            CrearNuevaFila();
        }

        // Método para generar una clave aleatoria de 32 bytes (256 bits)
        private byte[] GenerarClaveAleatoria()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] clave = new byte[32]; // 32 bytes para AES-256
                rng.GetBytes(clave);
                return clave;
            }
        }

        // Método para generar un IV aleatorio de 16 bytes (128 bits)
        private byte[] GenerarIVAleatorio()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] iv = new byte[16]; // 16 bytes para AES
                rng.GetBytes(iv);
                return iv;
            }
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
            btnBorrar.Click += (sender, e) => EliminarFila(nuevaFila, txtCifrado.Text);//he quitado + ".enc"

            // Agrega la nueva fila al panel de contenedor
            panelContenedor.Controls.Add(nuevaFila);

            txt1.Text = "";
        }
        private void EliminarFila(Panel fila,string nombreArchivoEncriptado)
        {
            BorrarArchivoEncriptado(nombreArchivoEncriptado);
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
        private void BorrarArchivoEncriptado(string nombreArchivo)
        {
            
            string rutaArchivoEncriptado = Path.Combine(rutaGuardado, nombreArchivo+".enc");
            string rutaClave = Path.Combine(rutaGuardado, nombreArchivo+"_clave.txt");
            string rutaIV = Path.Combine(rutaGuardado, nombreArchivo + "_IV.txt");
            // Verifica si existen los archivos
            if (File.Exists(rutaArchivoEncriptado) && File.Exists(rutaClave) && File.Exists(rutaIV))
            {
                // Elimina los archivos encriptados
                File.Delete(rutaArchivoEncriptado);
                File.Delete(rutaClave);
                File.Delete(rutaIV);
            }
        }
        private void panelContenedor_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
