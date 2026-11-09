using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrácticaHome_PPC;

namespace PrácticaHome_PPC.Tests
{
    /// <summary>
    /// Pruebas unitarias de la lógica de autenticación (AuthService).
    /// Cada prueba indica el caso del Plan de Pruebas (TCxx) que cubre.
    /// </summary>
    [TestClass]
    public class AuthServiceTests
    {
        // TC01. Inicio de sesión exitoso
        [TestMethod]
        public void TC01_UsuarioYContrasenaCorrectos_DevuelveCorrecto()
        {
            var auth = new AuthService();

            var resultado = auth.Validar("admin", "1234");

            Assert.AreEqual(ResultadoLogin.Correcto, resultado);
        }

        // TC01. Los espacios alrededor del usuario no impiden el login
        [TestMethod]
        public void TC01_UsuarioConEspacios_DevuelveCorrecto()
        {
            var auth = new AuthService();

            Assert.AreEqual(ResultadoLogin.Correcto, auth.Validar("  pedro  ", "ppc2026"));
        }

        // TC02. Inicio de sesión fallido (contraseña incorrecta)
        [TestMethod]
        public void TC02_ContrasenaIncorrecta_DevuelveContrasenaIncorrecta()
        {
            var auth = new AuthService();

            var resultado = auth.Validar("admin", "0000");

            Assert.AreEqual(ResultadoLogin.ContrasenaIncorrecta, resultado);
            Assert.AreEqual(2, auth.IntentosRestantes("admin"));
        }

        // TC03. El usuario no existe
        [TestMethod]
        public void TC03_UsuarioInexistente_DevuelveUsuarioNoExiste()
        {
            var auth = new AuthService();

            var resultado = auth.Validar("noexiste", "1234");

            Assert.AreEqual(ResultadoLogin.UsuarioNoExiste, resultado);
        }

        // TC04. Campos vacíos (los dos, uno de ellos o solo espacios)
        [TestMethod]
        [DataRow("", "")]
        [DataRow("admin", "")]
        [DataRow("", "1234")]
        [DataRow("   ", "1234")]
        [DataRow(null, null)]
        public void TC04_CamposVacios_DevuelveCamposVacios(string? usuario, string? contrasena)
        {
            var auth = new AuthService();

            Assert.AreEqual(ResultadoLogin.CamposVacios, auth.Validar(usuario, contrasena));
        }

        // TC05. Tres contraseñas incorrectas bloquean al usuario
        [TestMethod]
        public void TC05_TresContrasenasIncorrectas_BloqueaUsuario()
        {
            var auth = new AuthService();

            Assert.AreEqual(ResultadoLogin.ContrasenaIncorrecta, auth.Validar("admin", "mal1"));
            Assert.AreEqual(ResultadoLogin.ContrasenaIncorrecta, auth.Validar("admin", "mal2"));
            Assert.AreEqual(ResultadoLogin.UsuarioBloqueado, auth.Validar("admin", "mal3"));
        }

        // TC05. Estando bloqueado, ni siquiera la contraseña correcta permite entrar
        [TestMethod]
        public void TC05_UsuarioBloqueado_NoPuedeEntrarConContrasenaCorrecta()
        {
            var auth = new AuthService();
            for (int i = 0; i < AuthService.MaxIntentos; i++)
                auth.Validar("admin", "mal");

            Assert.AreEqual(ResultadoLogin.UsuarioBloqueado, auth.Validar("admin", "1234"));
            Assert.IsTrue(auth.SegundosBloqueoRestantes("admin") > 0);
        }

        // TC05. El bloqueo es temporal: pasado el tiempo se puede volver a entrar
        [TestMethod]
        public void TC05_PasadoElTiempoDeBloqueo_SePuedeEntrar()
        {
            DateTime ahora = new DateTime(2026, 11, 9, 12, 0, 0);
            var auth = new AuthService(() => ahora);
            for (int i = 0; i < AuthService.MaxIntentos; i++)
                auth.Validar("admin", "mal");

            ahora = ahora + AuthService.TiempoBloqueo + TimeSpan.FromSeconds(1);

            Assert.AreEqual(ResultadoLogin.Correcto, auth.Validar("admin", "1234"));
        }

        // TC05. El bloqueo de un usuario no afecta a los demás
        [TestMethod]
        public void TC05_BloqueoDeUnUsuario_NoAfectaAOtro()
        {
            var auth = new AuthService();
            for (int i = 0; i < AuthService.MaxIntentos; i++)
                auth.Validar("admin", "mal");

            Assert.AreEqual(ResultadoLogin.Correcto, auth.Validar("pedro", "ppc2026"));
        }

        // Un login correcto reinicia el contador de intentos fallidos
        [TestMethod]
        public void LoginCorrecto_ReiniciaContadorDeIntentos()
        {
            var auth = new AuthService();
            auth.Validar("admin", "mal");
            auth.Validar("admin", "mal");

            auth.Validar("admin", "1234");

            Assert.AreEqual(AuthService.MaxIntentos, auth.IntentosRestantes("admin"));
            Assert.AreEqual(ResultadoLogin.ContrasenaIncorrecta, auth.Validar("admin", "mal"));
        }
    }
}
