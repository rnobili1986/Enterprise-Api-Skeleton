# Enterprise-Api-Skeleton: .NET Identity & Zero-Trust Foundation

This repository provides a **foundational architecture** for systems requiring high security standards in identity management and data integrity. 

> [!IMPORTANT]
> **Project Status: Basic Skeleton.** This is a structural mockup and foundational boilerplate. It is designed to serve as a starting point for developers to build upon, not as a plug-and-play production-ready product without further implementation.

## 🚀 Architecture & Principles

The project is built under **Clean Architecture** and **SOLID** principles, ensuring security logic is decoupled from infrastructure and easily auditable.

* **Identity Management:** Robust **JWT (JSON Web Tokens)** implementation with custom claims validation.
* **E2E Security (Secure Vault):** Messaging flow using hybrid cryptography (`System.Security.Cryptography`) with dynamic IVs to ensure data integrity.
* **Hybrid Context Validation:** Dual-layer security combining **JWT** (Identity) with **Encrypted HttpOnly Cookies** (Context).
* **Decoupled Structure:** Strict separation between Domain, Application, Infrastructure, and API layers.

## ✨ Key Features

* ✅ **Centralized Auth Flow:** Designed to operate as a centralized authentication service for microservices ecosystems.
* ✅ **Cryptographic Vault:** Clean implementation of encryption services (AES-256) injected via interfaces.
* ✅ **Action Filter Security:** Custom `[ValidateUserContext]` attribute that intercepts requests to verify secure cookies before reaching the controller.
* ✅ **Enterprise Standards:** Code ready to pass security audits and comply with data protection regulations (GDPR / Privacy Laws).

## 🛡️ Advanced Security Flow (Zero-Trust)

In high-scale and sensitive systems, access is only the first step. This architecture mitigates risks through a **Defense in Depth** approach:

1. **Authentication:** The server issues a JWT for identity and a secondary encrypted "Secure Context" stored in a `HttpOnly` / `Secure` cookie.
2. **Interception:** A custom Action Filter (`ValidateUserContext`) intercepts requests to protected resources.
3. **Double-Check:** Access is granted only if the JWT is valid **AND** the encrypted cookie can be successfully decrypted/validated by the server's internal vault.
4. **Shielded Payloads:** Demonstrates how to move sensitive session data where traffic interception (XSS) does not compromise the information.

## 🧠 Why This Architecture?

In high-scale and sensitive systems, access is only the first step. This architecture mitigates risks through:

1.  **Logic Isolation:** Security is not a "patch"; it is a first-class citizen in the infrastructure layer.
2.  **Shielded Payloads:** Demonstrates how to move sensitive data (messages or session keys) where traffic interception does not compromise the information.

## 🛠️ Tech Stack

* **Runtime:** .NET 8.0
* **Language:** C#
* **Security:** JWT, AES-256.
* **Architecture:** Clean Architecture / DDD Pattern.

## 🚀 Roadmap & Potential Enhancements

This is a living skeleton. Future improvements or production-hardening steps could include:
* **MFA Integration:** Support for Multi-Factor Authentication (TOTP, SMS).
* **External Identity Providers:** Integration with Auth0, Azure AD, or Google Identity.
* **Refresh Token Pattern:** Implementation of persistent sessions via secure refresh tokens.
* **Rate Limiting:** Protection against brute-force attacks at the API level.
* **Secrets Management:** Integration with Azure Key Vault or HashiCorp Vault (moving away from `appsettings.json`).
* **Unit & Integration Tests:** Full coverage of the cryptographic and auth layers.

## ⚙️ Installation

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/rnobili1986/Enterprise-Api-Skeleton.git
    ```
2.  **Configuration:**
    Configure the security keys in the `appsettings.json` file.

---

# Versión en Español

Este repositorio proporciona una **arquitectura base** para sistemas que requieren altos estándares de seguridad en la gestión de identidades e integridad de datos.

> [!IMPORTANT]
> **Estado del Proyecto: Maqueta Básica.** Este es un esqueleto estructural y un boilerplate fundamental. Está diseñado para servir como punto de partida y no como un producto final "listo para usar" en producción sin implementaciones adicionales.

## 🚀 Arquitectura y Principios

* **Gestión de Identidad:** Implementación robusta de **JWT** con validación de claims.
* **Seguridad E2E (Secure Vault):** Flujo de mensajería con criptografía híbrida y IVs dinámicos.
* **Validación de Contexto Híbrida:** Seguridad de doble capa que combina **JWT** (Identidad) con **Cookies Cifradas HttpOnly** (Contexto).
* **Estructura Desacoplada:** Separación estricta entre las capas de Dominio, Aplicación, Infraestructura y API.

## 🛡️ Flujo de Seguridad Avanzado (Zero-Trust)

Este sistema no solo verifica si tienes una llave, sino que asegura que la llave pertenezca a la mano que la sostiene:

1. **Autenticación:** El servidor emite un JWT y un payload secundario cifrado almacenado en una cookie `HttpOnly`.
2. **Intercepción:** Un **Action Filter** personalizado (`ValidateUserContext`) intercepta las peticiones.
3. **Doble Validación:** El acceso se concede solo si el JWT es válido **Y** la cookie cifrada puede ser descifrada por la bóveda interna del servidor.
   
## ✨ Características Destacadas

* ✅ **Flujo de Auth Centralizado:** Diseñado para operar como un servicio de autenticación central para ecosistemas complejos.
* ✅ **Bóveda Criptográfica:** Implementación limpia de servicios de cifrado (AES-256) inyectados mediante interfaces.
* ✅ **Estándares Empresariales:** Código preparado para auditorías de seguridad y cumplimiento de normativas de protección de datos.

## 🧠 ¿Por qué esta arquitectura?

En sistemas de alta escala, esta arquitectura mitiga riesgos mediante:

1.  **Aislamiento de Lógica:** La seguridad no es un parche, es parte del núcleo de la infraestructura.
2.  **Payloads Blindados:** Asegura que la interceptación del tráfico no comprometa la información sensible del usuario.

## 🛠️ Stack Tecnológico

* **Runtime:** .NET 8.0
* **Lenguaje:** C#
* **Seguridad:** JWT, AES-256.
* **Arquitectura:** Clean Architecture / DDD Pattern.

## 🚀 Próximas Mejoras y Posibles Evoluciones

Al ser una maqueta base, existen varios caminos para robustecer el sistema:
* **Integración de MFA:** Soporte para Autenticación de Múltiples Factores (TOTP, SMS).
* **Proveedores Externos:** Integración con Auth0, Azure AD o Google Identity.
* **Patrón de Refresh Tokens:** Implementación de sesiones persistentes seguras.
* **Rate Limiting:** Protección contra ataques de fuerza bruta.
* **Gestión de Secretos:** Integración con Azure Key Vault o HashiCorp Vault (evitando `appsettings.json`).
* **Tests Unitarios e Integración:** Cobertura total de las capas criptográficas y de autenticación.

## ⚙️ Instalación

1.  **Clonar el repositorio:**
    ```bash
    git clone https://github.com/rnobili1986/Enterprise-Api-Skeleton.git
    ```
2.  **Configuración:**
    Configure las llaves de seguridad en el archivo `appsettings.json`.
