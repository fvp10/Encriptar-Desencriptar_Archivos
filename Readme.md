# Memoria del Proyecto de Encriptación y Desencriptación

## Índice 📑
* [Introducción](#introducción) 🚀
* [Fases del proyecto](#fases-del-proyecto) 📈
* [Manual de usuario](#manual-de-usuario) 📖
* [Software](#software) 💻
* [Librerías](#librerías) 📚
* [Detalles de implementación](#detalles-de-implementación) 🔨
    - [Interfaz Dinámica](#interfaz-dinámica) 🖥️
    - [Encriptar](#encriptar) 🔒
    - [Desencriptar](#desencriptar) 🔓
* [Team](#team) 👫

## Introducción 🚀
Esto es una práctica dentro de la carrera __Ing.Multimedia__ en la __Universidad de Alicante__. En este proyecto se realiza un trabajo en grupo (4 alumnos) relacionado con la seguridad y donde se pretende que desarrollemos conocimientos en el ambito de seguridad. 

El proyecto irá cambiando en función con las fases hasta tener un programa completo con todas las funcionalidades.

## Fases del proyecto 📈

| Fase | Descripción | Requisitos | Estado |
|------|-------------|------------|-------|
| Fase 1 | Implementación de un sistema que cifra y descifra archivos multimedia con AES256 | - Cifrado de al menos 6 archivos multimedia con claves diferentes <br> - Almacenamiento de claves en un archivo o base de datos | ✅ Completado |
| Fase 2 | Diseño e implementación de un sistema para autentificar al administrador y encriptar claves AES con RSA | - Autenticación del administrador <br> - Generación de claves RSA (pública y privada) <br> - Encriptación de claves AES con clave pública | ❌ Pendiente |
| Fase 3 | Sistema para el acceso de usuarios a los contenidos cifrados | - Generación de claves RSA para cada usuario <br> - Sistema de acceso a contenidos cifrados <br> - Mejoras en el sistema de autenticación | ❌ Pendiente |

## Manual de Usuario 📖

__Encriptar un Archivo__ 🔒
1. Abre la aplicación.
2. Haz clic en el botón "Seleccionar Archivo" y elije el archivo que deseas encriptar.
3. Haz clic en "Encriptar". El archivo encriptado se guardará en una carpeta específica, y se mostrará un mensaje de confirmación.

__Desencriptar un Archivo__ 🔓
1. En la lista de archivos encriptados, haz clic en "Desencriptar" al lado del archivo que desea desencriptar.
2. El archivo desencriptado se guardará en otra carpeta, se borrara en la que se encontraba actualmente, y se mostrará un mensaje de confirmación.

## Software 💻
* __Visual Studio:__ Entorno de desarrollo integrado de Microsoft utilizado para desarrollar aplicaciones, sitios web, servicios web y aplicaciones móviles.

* __.NET Framework__ Tecnología que soporta la construcción y ejecución de aplicaciones y servicios web, simplifica el desarrollo y la implementación de aplicaciones.

__Lenguaje: C#__

## Librerías 📚

<span style="font-size: larger; font-weight: bold; color:#ED9B40 ">Fase 1</span>

```
System: Se usa para operaciones generales y básicas

System.Collections.Generic: Almacenado y manipulación de dátos, como "botonesYFilas".

System.IO: Para la lectura y escritura de archivos.

System.Windows.Forms: Construye y manipula la interfaz gráfica, como botones y paneles.

System.Security.Cryptography:  Se utiliza para encriptar y desencriptar archivos.

System.Linq: Se usa para manipular y consultar datos a la hora del uso del "any".

``` 
<!-- ### Fase 2
```
```
### Fase 3
```
``` -->

## Detalles de implementación 🔨

<span style="font-size: larger; font-weight: bold; color:#ED9B40 ">Fase 1</span>

#### Interfaz Dinámica 🖥️
El código utiliza un diseño dinámico donde los archivos encriptados se muestran en tiempo real. Cuando un archivo se encripta, se agrega automáticamente a la lista visual en la interfaz de usuario. Esto se logra a través del método <span style="color:#9FD1F5">CrearNuevaFila()</span> que crea una nueva fila en la interfaz para cada archivo encriptado.

Para borrar un archivo, se usa el método <span style="color:#9FD1F5">EliminarFila()</span> que remueve la fila de la interfaz y elimina el archivo encriptado, así como sus claves asociadas, del sistema de archivos.

La actualización de la lista visual se realiza a través del método <span style="color:#9FD1F5">ActualizarLista()</span> , que limpia y reconstruye la lista de archivos encriptados basándose en los archivos presentes en el directorio de almacenamiento.

Para verificar y listar los archivos encriptados al iniciar la aplicación, se utiliza el método <span style="color:#9FD1F5">ComprobarArchivosEncriptados()</span> .

#### Encriptar 🔒
El proceso de encriptación se inicia cuando el usuario selecciona un archivo para encriptar y hace clic en "Encriptar". El evento asociado __(encriptar__Click)__ llama al método  <span style="color:#9FD1F5">MetodoDeEncriptado()</span>.

1. <span style="color:#9FD1F5">MetodoDeEncriptado()</span>: Este método se asegura de que un archivo esté seleccionado, genera una clave y un IV aleatorios, y procede a encriptar el archivo seleccionado. Utiliza AES para la encriptación.

2. <span style="color:#9FD1F5">GenerarClaveAleatoria() y GenerarIVAleatorio()</span>: Estos métodos usan RNGCryptoServiceProvider para generar una clave y un IV aleatorios.

3. El archivo encriptado, junto con la clave y el IV, se almacenan en archivos separados dentro de un directorio específico.

4. La interfaz se actualiza automáticamente para listar el nuevo archivo encriptado mediante el <span style="color:#9FD1F5">ActualizarLista()</span>.

#### Desencriptar 🔓
El  proceso de desencriptación inicia cuando el usuario hace clic en "Desencriptar" al lado de un archivo encriptado listado en la interfaz. El evento asociado __(desencriptar__Click)__ llama al método <span style="color:#9FD1F5">Desencriptado()</span> .

1. <span style="color:#9FD1F5">Desencriptado(String nombre)</span>: Este método recupera la clave y el IV asociados con el archivo encriptado desde sus archivos respectivos. Luego, utiliza estos para desencriptar el archivo.

2. Utiliza AES para la desencriptación, especificando la clave y el IV, y el modo CBC.

3. El archivo desencriptado se almacena en un directorio específico, y el usuario recibe una notificación de que la desencriptación fue exitosa.

4. La interfaz se actualiza automáticamente para reflejar los cambios con el <span style="color:#9FD1F5">ActualizarLista()</span>, y el archivo desencriptado se puede acceder desde el directorio especificado.

<!-- ### Fase 2
### Fase 3 -->

## Team 👨🏻‍🤝‍👨🏻
🧑 Yohannes Befikadu - ybbb2@alu.ua.es

🧑 Valentino Quiles - vqmq1@alu.ua.es

🧑 Felix Valois - fvp10@alu.ua.es

🧑 Jose Angel - jsg25@alu.ua.es
