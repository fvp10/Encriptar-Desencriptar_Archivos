using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica_CS_encriptar_desencriptar_F1.Modelos
{
    public class DatosEncriptados
    {
        public string NombreUsuario { get; set; }
        public byte[] ClaveEncriptadaRSA { get; set; }
        public byte[] IV { get; set; }
        public string RutaArchivoEncriptado { get; set; }
    }
}
