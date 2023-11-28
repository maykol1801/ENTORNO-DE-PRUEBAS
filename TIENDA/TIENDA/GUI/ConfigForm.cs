using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataManager;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;

namespace TIENDA.GUI
{
    public partial class ConfigForm : Form
    {
        private readonly DBConexion dbConexion = new DBConexion();

        public ConfigForm()
        {
            InitializeComponent();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("Formulario de configuración cargado");

            // Si no hay configuración en la base de datos, establecer la configuración predeterminada
            if (!ConfiguracionEnBaseDeDatosExiste())
            {
                EstablecerConfiguracionPredeterminada();
            }

            // Cargar la configuración desde la base de datos al cargar el formulario
            CargarConfiguracionDesdeBD();

            // Actualizar los controles en la interfaz de usuario con los valores cargados
            ActualizarControles();

            Debug.WriteLine($"Nombre después de actualizar controles: {Properties.Settings.Default.NombreEmpresa}");
            Debug.WriteLine($"Dirección después de actualizar controles: {Properties.Settings.Default.DireccionEmpresa}");
            Debug.WriteLine($"Teléfono después de actualizar controles: {Properties.Settings.Default.TelefonoEmpresa}");
        }


        private void CargarConfiguracionDesdeBD()
        {
            if (dbConexion.Conectar())
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("SELECT * FROM empresa", dbConexion.ObtenerConexion()))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Leer valores directamente desde la base de datos sin asignar a propiedades
                                string nombreEmpresa = Convert.ToString(reader["NombreEmpresa"]);
                                string direccionEmpresa = Convert.ToString(reader["DireccionEmpresa"]);
                                string telefonoEmpresa = Convert.ToString(reader["TelefonoEmpresa"]);
                                string rutaImagenEmpresa = Convert.ToString(reader["RutaImagenEmpresa"]);

                                // Verificar si la ruta de la imagen está vacía antes de cargarla
                                Debug.WriteLine($"Ruta de la imagen cargada desde la base de datos: {rutaImagenEmpresa}");

                                // Asignar siempre la ruta de la imagen, incluso si está vacía
                                Properties.Settings.Default.RutaImagenEmpresa = rutaImagenEmpresa;

                                // Verificar si los valores leídos son diferentes de los predeterminados
                                if (nombreEmpresa != "EXYDOS" || direccionEmpresa != "xxxx,xxxx, El salvador" || telefonoEmpresa != "xxxx-xxxx")
                                {
                                    // Asignar los valores solo si son diferentes de los predeterminados
                                    Properties.Settings.Default.NombreEmpresa = nombreEmpresa;
                                    Properties.Settings.Default.DireccionEmpresa = direccionEmpresa;
                                    Properties.Settings.Default.TelefonoEmpresa = telefonoEmpresa;
                                }

                                Properties.Settings.Default.Save();

                                Debug.WriteLine($"Nombre cargado desde la base de datos: {Properties.Settings.Default.NombreEmpresa}");
                                Debug.WriteLine($"Dirección cargada desde la base de datos: {Properties.Settings.Default.DireccionEmpresa}");
                                Debug.WriteLine($"Teléfono cargado desde la base de datos: {Properties.Settings.Default.TelefonoEmpresa}");
                                Debug.WriteLine($"Ruta de imagen cargada desde la base de datos: {Properties.Settings.Default.RutaImagenEmpresa}");
                            }
                            else
                            {
                                Debug.WriteLine("No se encontraron datos en la base de datos.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error al cargar configuración desde la base de datos: {ex.Message}");
                }
                finally
                {
                    dbConexion.Desconectar();
                }
            }
            else
            {
                Debug.WriteLine("No se pudo conectar a la base de datos.");
            }
        }

        private void MostrarImagenDesdeConfiguracion()
        {
            // Mostrar la imagen si la ruta de la imagen no está vacía
            if (!string.IsNullOrEmpty(Properties.Settings.Default.RutaImagenEmpresa))
            {
                MostrarImagen(Properties.Settings.Default.RutaImagenEmpresa);
            }
            else
            {
                // Si la ruta de la imagen está vacía, puedes restablecer el PictureBox
                pictureBoxEmpresa.Image = null;
            }
        }



        private bool ConfiguracionEnBaseDeDatosExiste()
        {
            if (dbConexion.Conectar())
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM empresa", dbConexion.ObtenerConexion()))
                    {
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    // Manejar errores aquí
                    Debug.WriteLine($"Error al verificar la existencia de configuración en la base de datos: {ex.Message}");
                    return false;
                }
                finally
                {
                    dbConexion.Desconectar();
                }
            }
            else
            {
                Debug.WriteLine("No se pudo conectar a la base de datos.");
                return false;
            }
        }



        private bool ConfiguracionPredeterminadaPresente()
        {
            return Properties.Settings.Default.NombreEmpresa == "EXYDOS" &&
                   Properties.Settings.Default.DireccionEmpresa == "xxxx,xxxx, El salvador" &&
                   Properties.Settings.Default.TelefonoEmpresa == "xxxx-xxxx";

            
        }
    



        private bool ConfiguracionModificada()
        {
            // Verificar si la configuración ha sido modificada
            return !string.IsNullOrEmpty(Properties.Settings.Default.NombreEmpresa);
        }

        private void EstablecerConfiguracionPredeterminada()
        {
            // Establecer valores predeterminados si la configuración no ha sido modificada
            Properties.Settings.Default.NombreEmpresa = "EXYDOS";
            Properties.Settings.Default.DireccionEmpresa = "xxxx,xxxx, El salvador";
            Properties.Settings.Default.TelefonoEmpresa = "xxxx-xxxx";
            Properties.Settings.Default.Save();
        }




        private void ActualizarControles()
        {
            // Actualizar los controles en la interfaz de usuario con los valores cargados
            txtNombre.Text = Properties.Settings.Default.NombreEmpresa;
            txtDireccion.Text = Properties.Settings.Default.DireccionEmpresa;
            txtTelefono.Text = Properties.Settings.Default.TelefonoEmpresa;

            // Mostrar la imagen desde la configuración
            MostrarImagenDesdeConfiguracion();

            Debug.WriteLine("Controles actualizados.");
            Debug.WriteLine($"Nombre después de actualizar controles: {txtNombre.Text}");
            Debug.WriteLine($"Dirección después de actualizar controles: {txtDireccion.Text}");
            Debug.WriteLine($"Teléfono después de actualizar controles: {txtTelefono.Text}");
        }


        private void MostrarImagen(string rutaImagen)
        {
            try
            {
                // Verificar si la ruta almacenada no es nula o vacía
                if (!string.IsNullOrEmpty(rutaImagen))
                {
                    // Combinar la ruta almacenada con el directorio de trabajo
                    string rutaCompleta = Path.Combine(Environment.CurrentDirectory, rutaImagen);

                    Debug.WriteLine($"Ruta completa de la imagen: {rutaCompleta}");

                    if (File.Exists(rutaCompleta))
                    {
                        pictureBoxEmpresa.Image = Image.FromFile(rutaCompleta);
                    }
                    else
                    {
                        Debug.WriteLine("La ruta de la imagen no es válida o el archivo no existe.");
                    }
                }
                else
                {
                    Debug.WriteLine("La ruta de la imagen almacenada en la base de datos es nula o vacía.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar la imagen: {ex.Message}");
            }
        }




        private void btnCargarImagen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de imagen|*.png;*.jpg;*.jpeg;*.gif;*.bmp|Todos los archivos|*.*";
                openFileDialog.Title = "Seleccionar Imagen de la Empresa";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Obtener la ruta de la imagen seleccionada
                    string rutaImagen = openFileDialog.FileName;

                    // Mostrar la imagen en el PictureBox
                    pictureBoxEmpresa.Image = Image.FromFile(rutaImagen);

                    // Guardar la ruta de la imagen seleccionada en la configuración
                    Properties.Settings.Default.RutaImagenEmpresa = rutaImagen;
                    Properties.Settings.Default.Save();

                    // También, guarda los valores en la base de datos después de seleccionar la imagen
                    GuardarConfiguracionEnBD(
                        Properties.Settings.Default.NombreEmpresa,
                        Properties.Settings.Default.DireccionEmpresa,
                        Properties.Settings.Default.TelefonoEmpresa,
                        Properties.Settings.Default.RutaImagenEmpresa);
                }
            }
        }


        private void GuardarConfiguracionEnBD(string nombreEmpresa, string direccionEmpresa, string telefonoEmpresa, string rutaImagenEmpresa)
        {
            Console.WriteLine("Guardando configuración en la base de datos...");

            // Verificar si la ruta de la imagen es nula antes de guardarla
            if (rutaImagenEmpresa == null)
            {
                rutaImagenEmpresa = string.Empty; // o el valor predeterminado que desees
            }

            if (dbConexion.Conectar())
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("GuardarConfiguracionEmpresa", dbConexion.ObtenerConexion()))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@NombreEmpresa", nombreEmpresa);
                        command.Parameters.AddWithValue("@DireccionEmpresa", direccionEmpresa);
                        command.Parameters.AddWithValue("@TelefonoEmpresa", telefonoEmpresa);
                        command.Parameters.AddWithValue("@RutaImagenEmpresa", (object)rutaImagenEmpresa ?? DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        Console.WriteLine($"Index #{i}\n" +
                                          $"Error Number: {ex.Errors[i].Number}\n" +
                                          $"State: {ex.Errors[i].State}\n" +
                                          $"Class: {ex.Errors[i].Class}\n" +
                                          $"Message: {ex.Errors[i].Message}\n" +
                                          $"LineNumber: {ex.Errors[i].LineNumber}\n" +
                                          $"Source: {ex.Errors[i].Source}\n" +
                                          $"Procedure: {ex.Errors[i].Procedure}\n\n");
                    }
                }
                catch (Exception ex)
                {
                    // Manejar otros tipos de errores
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    dbConexion.Desconectar();
                }
            }

            Console.WriteLine("Configuración guardada en la base de datos.");
        }



        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Guardar los valores ingresados por el usuario en la configuración
            Properties.Settings.Default.NombreEmpresa = txtNombre.Text;
            Properties.Settings.Default.DireccionEmpresa = txtDireccion.Text;
            Properties.Settings.Default.TelefonoEmpresa = txtTelefono.Text;

            // Guardar los cambios en la configuración
            Properties.Settings.Default.Save();

            // Guardar la configuración en la base de datos
            GuardarConfiguracionEnBD(
                Properties.Settings.Default.NombreEmpresa,
                Properties.Settings.Default.DireccionEmpresa,
                Properties.Settings.Default.TelefonoEmpresa,
                Properties.Settings.Default.RutaImagenEmpresa);

            // Cerrar la ventana de configuración
            this.Close();
        }



        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtNombre_MouseEnter(object sender, EventArgs e)
        {
            // Cambia el texto y color del texto cuando el usuario hace clic en el TextBox
            if (txtNombre.Text == "Ejemplo: Exydos Company")
            {
                txtNombre.Text = "";
                txtNombre.ForeColor = Color.Black; // Cambia el color del texto a negro
            }
        }

        private void txtNombre_MouseLeave(object sender, EventArgs e)
        {
            // Restaura el texto y color del texto si el usuario no ingresó nada
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                txtNombre.Text = "Ejemplo: Exydos Company";
                txtNombre.ForeColor = Color.Gray; // Cambia el color del texto a gris
            }
        }

        private void txtDireccion_MouseEnter(object sender, EventArgs e)
        {
            if (txtDireccion.Text == "Ejemplo: 501 W Dallas, TX, 503")
            {
                txtDireccion.Text = "";
                txtDireccion.ForeColor = Color.Black;
            }
        }

        private void txtDireccion_MouseLeave(object sender, EventArgs e)
        {
            // Restaura el texto y color del texto si el usuario no ingresó nada
            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                txtDireccion.Text = "Ejemplo: 501 W Dallas, TX, 503";
                txtDireccion.ForeColor = Color.Gray;
            }
        }

        private void txtTelefono_MouseEnter(object sender, EventArgs e)
        {
            if (txtTelefono.Text == "Ejemplo: +1 888-751-9752")
            {
                txtTelefono.Text = "";
                txtTelefono.ForeColor = Color.Black;
            }
        }

        private void txtTelefono_MouseLeave(object sender, EventArgs e)
        {
            // Restaura el texto y color del texto si el usuario no ingresó nada
            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                txtTelefono.Text = "Ejemplo: +1 888-751-9752";
                txtTelefono.ForeColor = Color.Gray;
            }
        }
    }
}
