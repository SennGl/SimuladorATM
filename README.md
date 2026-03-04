# Console ATM System (C# & JSON)

Un sistema de cajero automático funcional desarrollado en C# que simula operaciones bancarias con persistencia de datos y seguridad.

## Características
- **Seguridad:** Hash de contraseñas mediante SHA256 (nunca se guardan en texto plano).
- **Persistencia:** Guardado y carga automática de datos mediante archivos JSON.
- **Historial:** Registro detallado de movimientos por cada usuario.
- **Validaciones:** Control estricto de entradas de usuario para evitar errores (Crash-proof).

## Tecnologías
- **Lenguaje:** C# (.NET 6.0/7.0/8.0)
- **Formato de datos:** JSON (System.Text.Json)
- **Seguridad:** System.Security.Cryptography
