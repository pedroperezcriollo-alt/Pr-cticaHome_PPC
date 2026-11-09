namespace PrácticaHome_PPC
{
    /// <summary>
    /// Posibles resultados de un intento de inicio de sesión.
    /// </summary>
    public enum ResultadoLogin
    {
        Correcto,
        CamposVacios,
        UsuarioNoExiste,
        ContrasenaIncorrecta,
        UsuarioBloqueado
    }

    /// <summary>
    /// Lógica de autenticación simulada en memoria.
    /// No depende de WPF para poder probarla con pruebas unitarias.
    /// </summary>
    public class AuthService
    {
        /// <summary>Número de intentos fallidos que bloquean al usuario.</summary>
        public const int MaxIntentos = 3;

        /// <summary>Tiempo que permanece bloqueado el usuario.</summary>
        public static readonly TimeSpan TiempoBloqueo = TimeSpan.FromSeconds(30);

        // Almacén de credenciales en memoria: columna 0 = usuario, columna 1 = contraseña
        private readonly string[,] usuarios =
        {
            { "admin",    "1234" },
            { "pedro",    "ppc2026" },
            { "invitado", "invitado" }
        };

        private readonly Dictionary<string, int> intentosFallidos = new();
        private readonly Dictionary<string, DateTime> bloqueadoHasta = new();
        private readonly Func<DateTime> reloj;

        public AuthService() : this(() => DateTime.Now) { }

        /// <summary>
        /// Constructor que permite indicar el reloj (se usa en las pruebas
        /// para simular el paso del tiempo sin esperar 30 segundos).
        /// </summary>
        public AuthService(Func<DateTime> reloj)
        {
            this.reloj = reloj;
        }

        /// <summary>
        /// Valida el usuario y la contraseña contra la lista en memoria.
        /// </summary>
        public ResultadoLogin Validar(string? usuario, string? contrasena)
        {
            usuario = usuario?.Trim() ?? "";
            contrasena ??= "";

            // 1. Campos vacíos
            if (usuario.Length == 0 || contrasena.Length == 0)
                return ResultadoLogin.CamposVacios;

            // 2. Buscar el usuario en la lista
            int fila = BuscarUsuario(usuario);
            if (fila == -1)
                return ResultadoLogin.UsuarioNoExiste;

            // 3. ¿Está bloqueado?
            if (EstaBloqueado(usuario))
                return ResultadoLogin.UsuarioBloqueado;

            // 4. Comprobar la contraseña
            if (usuarios[fila, 1] == contrasena)
            {
                intentosFallidos.Remove(usuario);
                return ResultadoLogin.Correcto;
            }

            int fallos = intentosFallidos.GetValueOrDefault(usuario) + 1;
            intentosFallidos[usuario] = fallos;

            if (fallos >= MaxIntentos)
            {
                bloqueadoHasta[usuario] = reloj() + TiempoBloqueo;
                intentosFallidos.Remove(usuario);
                return ResultadoLogin.UsuarioBloqueado;
            }

            return ResultadoLogin.ContrasenaIncorrecta;
        }

        /// <summary>Intentos que le quedan al usuario antes de bloquearse.</summary>
        public int IntentosRestantes(string usuario)
        {
            return MaxIntentos - intentosFallidos.GetValueOrDefault(usuario.Trim());
        }

        /// <summary>Segundos que faltan para que se desbloquee el usuario (0 si no está bloqueado).</summary>
        public int SegundosBloqueoRestantes(string usuario)
        {
            usuario = usuario.Trim();
            if (!EstaBloqueado(usuario))
                return 0;
            return (int)Math.Ceiling((bloqueadoHasta[usuario] - reloj()).TotalSeconds);
        }

        private bool EstaBloqueado(string usuario)
        {
            if (!bloqueadoHasta.TryGetValue(usuario, out DateTime hasta))
                return false;

            if (reloj() >= hasta)
            {
                // Ya ha pasado el tiempo de bloqueo
                bloqueadoHasta.Remove(usuario);
                return false;
            }
            return true;
        }

        private int BuscarUsuario(string usuario)
        {
            for (int i = 0; i < usuarios.GetLength(0); i++)
            {
                if (usuarios[i, 0] == usuario)
                    return i;
            }
            return -1;
        }
    }
}
