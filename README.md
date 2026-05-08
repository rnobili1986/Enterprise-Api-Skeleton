# Enterprise-Api-Skeleton: .NET Identity & Zero-Trust Foundation

This repository provides a foundational architecture for **mission-critical** systems requiring the highest security standards in identity management and data integrity. Unlike standard authentication implementations, this boilerplate introduces an **End-to-End (E2E) encryption** layer for sensitive payloads, following **Zero-Knowledge** principles.

## 🚀 Architecture & Principles

The project is built under **Clean Architecture** and **SOLID** principles, ensuring security logic is decoupled from infrastructure and easily auditable.

* **Identity Management:** Robust **JWT (JSON Web Tokens)** implementation with custom claims validation.
* **E2E Security (Secure Vault):** Messaging flow using hybrid cryptography (`System.Security.Cryptography`) to ensure the server can deliver sensitive information that only the authorized client can decrypt.
* **Decoupled Structure:** Strict separation between Domain, Application, Infrastructure, and API layers to maintain a clean codebase.

## ✨ Key Features

* ✅ **Centralized Auth Flow:** Designed to operate as a centralized authentication service for microservices ecosystems.
* ✅ **Cryptographic Vault:** Clean implementation of encryption services (AES-256) injected via interfaces.
* ✅ **Enterprise Standards:** Code ready to pass security audits and comply with data protection regulations (GDPR / Privacy Laws).

## 🧠 Why This Architecture?

In high-scale and sensitive systems (such as **National Census** processing or **Core Banking** platforms), access is only the first step. This architecture mitigates risks through:

1.  **Logic Isolation:** Security is not a "patch"; it is a first-class citizen in the infrastructure layer.
2.  **Shielded Payloads:** Demonstrates how to move sensitive data (messages or session keys) where traffic interception does not compromise the information.

## 🛠️ Tech Stack

* **Runtime:** .NET 8.0
* **Language:** C#
* **Security:** JWT, AES-256.
* **Architecture:** Clean Architecture / DDD Pattern.

## ⚙️ Installation

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/rnobili1986/Enterprise-Api-Skeleton.git
    ```
2.  **Configuration:**
    Configure the security keys in the `appsettings.json` file.

---

# Versión en Español

Este repositorio proporciona una arquitectura base para sistemas de **misión crítica** que requieren los más altos estándares de seguridad en la gestión de identidades y la integridad de los datos. A diferencia de las implementaciones estándar, este boilerplate introduce una capa de **encriptación de extremo a extremo (E2E)** siguiendo principios de **Zero-Knowledge**.

## 🚀 Arquitectura y Principios

El proyecto está construido bajo los principios de **Clean Architecture** y **SOLID**, garantizando que la lógica de seguridad esté desacoplada de la infraestructura y sea fácilmente auditable.

* **Gestión de Identidad:** Implementación robusta de **JWT** con validación de claims personalizada.
* **Seguridad E2E (Secure Vault):** Flujo de mensajería que utiliza criptografía híbrida para asegurar que la información sensible solo sea accesible por clientes autorizados.
* **Estructura Desacoplada:** Separación estricta entre las capas de Dominio, Aplicación, Infraestructura y API.

## ✨ Características Destacadas

* ✅ **Flujo de Auth Centralizado:** Diseñado para operar como un servicio de autenticación central para ecosistemas complejos.
* ✅ **Bóveda Criptográfica:** Implementación limpia de servicios de cifrado (AES-256) inyectados mediante interfaces.
* ✅ **Estándares Empresariales:** Código preparado para auditorías de seguridad y cumplimiento de normativas de protección de datos.

## 🧠 ¿Por qué esta arquitectura?

En sistemas de alta escala (como el procesamiento de datos del **Censo Nacional**), esta arquitectura mitiga riesgos mediante:

1.  **Aislamiento de Lógica:** La seguridad no es un parche, es parte del núcleo de la infraestructura.
2.  **Payloads Blindados:** Asegura que la interceptación del tráfico no comprometa la información sensible del usuario.

## 🛠️ Stack Tecnológico

* **Runtime:** .NET 8.0
* **Lenguaje:** C#
* **Seguridad:** JWT, AES-256.
* **Arquitectura:** Clean Architecture / DDD Pattern.

## ⚙️ Instalación

1.  **Clonar el repositorio:**
    ```bash
    git clone https://github.com/rnobili1986/Enterprise-Api-Skeleton.git
    ```
2.  **Configuración:**
    Configure las llaves de seguridad en el archivo `appsettings.json`.
