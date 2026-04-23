# LocalVault

**LocalVault** es una aplicación web full stack para la gestión de archivos en una nube local.  
Permite subir, organizar, consultar, descargar y eliminar archivos, además de agruparlos en categorías o carpetas lógicas.  

El proyecto está dividido en:
- **API** en ASP.NET Core  
- **Frontend** en Blazor WebAssembly  

---

## 🎯 Objetivo del proyecto
Construir una solución sencilla pero funcional para almacenamiento y organización de archivos en un entorno local, con una arquitectura separada entre frontend, backend y modelos compartidos.

---

## ⚙️ Funcionalidades
- Subida de archivos desde el navegador  
- Cálculo de hash **SHA-256** para identificar archivos  
- Almacenamiento de metadatos en base de datos  
- Descarga y eliminación de archivos  
- Creación y gestión de categorías  
- Organización de archivos por carpeta lógica  
- Interfaz web sencilla y orientada a uso práctico  

---

## 🛠️ Tecnologías utilizadas
- ASP.NET Core Web API  
- Blazor WebAssembly  
- Entity Framework Core  
- SQLite  
- C#  
- HTML, CSS y Bootstrap  

---

## 🏗️ Arquitectura del proyecto
El repositorio está organizado en tres partes principales:

- **FrontApp**: frontend desarrollado en Blazor WebAssembly  
- **NubeApi**: backend con la API REST y la lógica de negocio  
- **Models**: proyecto compartido con los DTOs utilizados entre frontend y backend  

---

## 🔗 Endpoints principales

### Archivos
- `POST /api/ArchivoReferencia/subir-archivo`  
- `GET /api/ArchivoReferencia/obtener-archivos/{id?}`  
- `GET /api/ArchivoReferencia/obtener-archivo/{id}`  
- `GET /api/ArchivoReferencia/descargar-archivo/{id}`  
- `DELETE /api/ArchivoReferencia/eliminar-archivo/{id}`  

### Categorías
- `POST /api/Categoria/crear-categoria`  
- `GET /api/Categoria/obtener-categorias`  
- `PATCH /api/Categoria/insertar-archivo-en-categoria/{ID_archivo_referencia}/{ID_Categoria}`  
- `DELETE /api/Categoria/eliminar-categoria/{ID_Categoria}`  

---

## 🚀 Cómo ejecutar el proyecto
1. Abrir la solución completa  
2. Ejecutar primero la API  
3. Ejecutar después el frontend  
4. Verificar que la base de datos SQLite se haya creado correctamente  
5. Acceder a la aplicación desde el navegador  

---

## 📂 Estructura general

### FrontApp
- **Pages**: pantallas principales de la interfaz  
- **Servicios**: consumo de la API  
- **wwwroot**: archivos estáticos  

### NubeApi
- **Controllers**: endpoints REST  
- **Servicios**: lógica de negocio  
- **Datos**: contexto de base de datos  
- **Clases**: entidades del dominio  

### Models
- **DTOs** compartidos entre proyectos  

---

## 🔮 Mejoras futuras
- Sistema de autenticación de usuarios  
- Soporte para múltiples cuentas y espacios privados  
- Búsqueda y filtros avanzados  
- Vista previa de archivos  
- Deploy en red local o servidor propio  

---

## 📌 Nota del proyecto
Este proyecto fue desarrollado como una solución funcional para gestión de archivos en nube local, priorizando la parte backend, la estructura limpia y la separación entre frontend y API.
