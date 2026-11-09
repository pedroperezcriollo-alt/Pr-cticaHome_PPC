using System.Windows;

namespace PrácticaHome_PPC
{
    /// <summary>
    /// Formulario principal (Home del portal), se muestra tras un login correcto.
    /// </summary>
    public partial class F_Principal : Window
    {
        private readonly Window? login;
        private bool cerrandoSesion;

        // Constructor sin parámetros (lo usa el diseñador de Visual Studio)
        public F_Principal()
        {
            InitializeComponent();
        }

        public F_Principal(string usuario, Window login) : this()
        {
            this.login = login;
            txtBienvenida.Text = $"¡Bienvenido, {usuario}!";
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            cerrandoSesion = true;
            login?.Show();   // Volvemos al formulario de login
            Close();
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Si se cierra la ventana con la X (sin cerrar sesión), se cierra toda la aplicación,
        // porque el login está oculto y si no la aplicación seguiría abierta.
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (!cerrandoSesion)
                Application.Current.Shutdown();
        }
    }
}
