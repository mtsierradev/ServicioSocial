# 🎓 Sistema de Servicio Social - .NET Core 8

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0.403-512BD4?logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0.10-512BD4)
![Entity Framework](https://img.shields.io/badge/Entity_Framework-9.0.10-512BD4)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?logo=microsoft-sql-server)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)
![Linux](https://img.shields.io/badge/OS-Linux-FCC624?logo=linux)
![License](https://img.shields.io/badge/License-MIT-green)

**Sistema web para gestión de servicio social desarrollado con ASP.NET Core 8.0.10 MVC, Entity Framework 9 e Identity**

</div>

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Tecnologías](#-tecnologías)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Instalación](#-instalación)
- [Configuración](#-configuración)
- [Usuarios y Roles](#-usuarios-y-roles)
- [Despliegue](#-despliegue)
- [Desarrolladores](#-desarrolladores)

## 🚀 Características

- **✅ Autenticación y Autorización** con ASP.NET Identity
- **👥 Sistema de 3 Roles**: Admin, Docente, User
- **🗄️ Base de Datos** con Entity Framework Core 9
- **📱 Interface MVC** con Razor Pages
- **🔒 Seguridad** con políticas de lockout personalizadas
- **📊 Migraciones automáticas** en inicio
- **✉️ Servicio de Email** integrado
- **🏗️ Scaffolding** para generación de código

## 💻 Tecnologías

### 🛠️ Entorno de Desarrollo Confirmado
| Componente | Versión |
|------------|---------|
| **.NET SDK** | 8.0.403 |
| **.NET Runtime** | 8.0.10 |
| **ASP.NET Core** | 8.0.10 |
| **Entity Framework Core** | 9.0.10 |
| **Sistema Operativo** | Linux |
| **Base de Datos** | SQL Server / SQLite |

### 📦 Dependencias NuGet Exactas
| Paquete | Versión | Propósito |
|---------|---------|-----------|
| `Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore` | 8.0.10 | Diagnósticos EF Core |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 8.0.11 | Identity con EF Core |
| `Microsoft.AspNetCore.Identity.UI` | 8.0.11 | UI de Identity |
| `Microsoft.EntityFrameworkCore.Sqlite` | 8.0.11 | Soporte SQLite |
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.10 | EF Core 9 - Principal |
| `Microsoft.EntityFrameworkCore.Tools` | 9.0.10 | EF Core 9 - Herramientas |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | 8.0.7 | Scaffolding |

## 📁 Estructura del Proyecto
