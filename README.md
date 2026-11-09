# PrácticaHome_PPC – Catálogo de videojuegos (WPF / .NET 10)

Unidad Didáctica 1 – Confección de Interfaces de Usuario (RA1). Autor: Pedro Pérez Criollo.

- **Práctica 1 – Home del portal**: `F_Principal` (barra superior, menú lateral y zona central de videojuegos).
- **Práctica 2 – Inicio de sesión**: `F_Login` con autenticación simulada en memoria y navegación entre formularios.

## Estructura

```
PrácticaHome_PPC/
├── App.xaml                → arranca en F_Login
├── F_Login.xaml(.cs)       → formulario de login (usuario, contraseña, Login, Salir, mensaje dinámico)
├── F_Principal.xaml(.cs)   → Home + mensaje de bienvenida + "Cerrar sesión" + "Salir"
└── AuthService.cs          → almacén de usuarios (string[,]) y lógica de validación
PrácticaHome_PPC.Tests/
└── AuthServiceTests.cs     → pruebas unitarias MSTest (TC01–TC05)
```

## Usuarios de prueba (en memoria)

| Usuario    | Contraseña |
|------------|------------|
| `admin`    | `1234`     |
| `pedro`    | `ppc2026`  |
| `invitado` | `invitado` |

## Comportamiento

| Caso | Resultado |
|------|-----------|
| Usuario y contraseña correctos | Se abre la Home con “¡Bienvenido, usuario!” |
| Contraseña incorrecta | “La contraseña introducida no es correcta” + intentos restantes |
| Usuario inexistente | “El usuario introducido no existe” y se limpian los campos |
| Campos vacíos | “Introduzca los datos…” |
| 3 contraseñas incorrectas | Usuario bloqueado 30 segundos |
| Cerrar sesión | Vuelve al login |
| Salir (en cualquier formulario) | Cierra la aplicación |

## Ejecutar

- Aplicación: abrir `PrácticaHome_PPC.slnx` en Visual Studio y pulsar **F5**.
- Pruebas: **Prueba → Explorador de pruebas → Ejecutar todas**, o `dotnet test` desde la carpeta de la solución.
