using System.Windows;
using System.Windows.Media;

namespace PrácticaHome_PPC
{
    /// <summary>
    /// Formulario de inicio de sesión.
    /// </summary>
    public partial class F_Login : Window
    {
        private readonly AuthService auth = new AuthService();

        public F_Login()
        {
            InitializeComponent();
            txtUsuario.Focus();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = pwdContrasena.Password;

            ResultadoLogin resultado = auth.Validar(usuario, contrasena);

            switch (resultado)
            {
                case ResultadoLogin.Correcto:
                    MostrarMensaje("Inicio de sesión correcto.", esError: false);
                    AbrirPrincipal(usuario);
                    break;

                case ResultadoLogin.CamposVacios:
                    MostrarError("Introduzca los datos: el usuario y la contraseña son obligatorios.");
                    if (usuario.Length == 0) txtUsuario.Focus(); else pwdContrasena.Focus();
                    break;

                case ResultadoLogin.UsuarioNoExiste:
                    MostrarError("El usuario introducido no existe.");
                    // Limpiamos los campos para que vuelva a escribir el usuario
                    txtUsuario.Clear();
                    pwdContrasena.Clear();
                    txtUsuario.Focus();
                    break;

                case ResultadoLogin.ContrasenaIncorrecta:
                    MostrarError("La contraseña introducida no es correcta.\n" +
                                 $"Intentos restantes: {auth.IntentosRestantes(usuario)}");
                    pwdContrasena.Clear();
                    pwdContrasena.Focus();
                    break;

                case ResultadoLogin.UsuarioBloqueado:
                    MostrarError($"Usuario bloqueado temporalmente por {AuthService.MaxIntentos} intentos fallidos.\n" +
                                 $"Inténtelo de nuevo en {auth.SegundosBloqueoRestantes(usuario)} segundos.");
                    pwdContrasena.Clear();
                    break;
            }
        }

        private void AbrirPrincipal(string usuario)
        {
            var principal = new F_Principal(usuario, this);
            principal.Show();

            // Dejamos el login limpio para cuando se cierre sesión
            txtUsuario.Clear();
            pwdContrasena.Clear();
            txtMensaje.Text = "";
            Hide();
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>Muestra el error en la etiqueta y en una ventana emergente.</summary>
        private void MostrarError(string texto)
        {
            MostrarMensaje(texto, esError: true);
            MessageBox.Show(this, texto, "Error de inicio de sesión",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void MostrarMensaje(string texto, bool esError)
        {
            txtMensaje.Text = texto;
            txtMensaje.Foreground = esError ? Brushes.IndianRed : Brushes.LightGreen;
        }
    }
}
