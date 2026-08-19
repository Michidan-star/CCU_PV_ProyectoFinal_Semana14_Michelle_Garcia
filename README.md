# Sistema de Gestión - Proyecto Final Programación V

Sistema web full-stack desarrollado con **.NET 10 (LTS)**, **ASP.NET Core (Razor Pages y Web API)**, **Entity Framework Core** y **SQL Server**.

## 🚀 Estructura de la Solución
* **Erdyka.Api**: Web API REST que maneja la lógica de negocio, endpoints y base de datos.
* **Erdyka.Web**: Aplicación cliente en Razor Pages que consume la API mediante `HttpClient`, implementando autenticación basada en cookies y roles.

---

## 🛠️ Requisitos Previos
* [.NET 10 SDK](https://dotnet.microsoft.com/)
* **SQL Server**
* **Docker Desktop**

---

## ⚙️ Instrucciones de Ejecución Local

1. **Clonar el repositorio:**
   ```bash
   git clone [https://github.com/Michidan-star/CCU_PV_ProyectoFinal_Semana14_Michelle_Garcia.git](https://github.com/Michidan-star/CCU_PV_ProyectoFinal_Semana14_Michelle_Garcia.git)

---
## Despliegue con Docker
Para construir y ejecutar la aplicación utilizando Docker, sigue estos pasos:

2. Construir la imagen de Docker:
   ```bash
   docker build -t erdyka-app .

---
## Ejecutar la aplicación en un contenedor Docker:
   ```bash
   docker run -d -p 8080:80 --name erdyka-container erdyka-app
   ```

---
## Video de Defensa
Puedes ver la demostración del sistema aquí:
[Ver video de defensa](https://umcasj-my.sharepoint.com/:f:/r/personal/604860538_castrocarazo_ac_cr/Documents/Programaci%C3%B3nV/ProyectoFinal_Defensa?d=wd29667706dc747f6bee3a92618a3d03e&csf=1&web=1&e=Np80mo)