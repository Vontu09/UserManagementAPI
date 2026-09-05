# User Management API - Complete Documentation

**Version**: 1.0.0  
**Framework**: .NET 9  
**Language**: C# 13.0  
**License**: MIT  
**Author**: TechHive Solutions  
**Repository**: https://github.com/Vontu09/UserManagementAPI

---

## Table of Contents

1. [Overview](#overview)
2. [Features](#features)
3. [Technology Stack](#technology-stack)
4. [Project Structure](#project-structure)
5. [Getting Started](#getting-started)
6. [Installation](#installation)
7. [Configuration](#configuration)
8. [API Endpoints](#api-endpoints)
9. [Authentication](#authentication)
10. [Middleware Pipeline](#middleware-pipeline)
11. [Error Handling](#error-handling)
12. [Testing](#testing)
13. [GitHub Copilot Integration](#github-copilot-integration)
14. [Security Considerations](#security-considerations)
15. [Contributing](#contributing)
16. [License](#license)
17. [Support](#support)

---

## Overview

**User Management API** is a secure, enterprise-grade REST API for managing users with JWT authentication, comprehensive logging, and standardized error handling. Built with .NET 9, this project demonstrates best practices in API development, including secure authentication, audit logging, and centralized error handling.

The API provides complete CRUD operations for user management with:
- Token-based authentication using JWT
- Request/response logging for compliance audits
- Standardized error responses across all endpoints
- Input validation with meaningful error messages
- Health monitoring endpoints
- Interactive Swagger documentation

---

## Features

### Core Functionality
- ✅ **CRUD Operations**: Create, Read, Update, Delete users
- ✅ **User Validation**: First name, last name, email, and phone validation
- ✅ **Duplicate Prevention**: Automatic detection and prevention of duplicate email addresses

### Security
- 🔐 **JWT Authentication**: Token-based security for all endpoints
- 🔐 **Token Expiration**: Configurable token lifetime
- 🔐 **Secure Configuration**: Sensitive data stored in environment variables
- 🔐 **HTTPS Support**: HTTP to HTTPS redirection in production

### Logging & Monitoring
- 📋 **Request/Response Logging**: Complete audit trail for compliance
- 📋 **Source IP Tracking**: Records client IP addresses
- 📋 **Structured Logging**: Console and debug output
- 🏥 **Health Checks**: API health monitoring endpoint

### Error Handling
- ⚠️ **Standardized Responses**: Consistent error format across all endpoints
- ⚠️ **Trace IDs**: Track errors across system with unique identifiers
- ⚠️ **Meaningful Messages**: Clear, actionable error descriptions
- ⚠️ **Exception Logging**: Comprehensive exception details in logs

### API Documentation
- 📚 **Swagger UI**: Interactive API documentation
- 📚 **OpenAPI Specification**: Machine-readable API schema
- 📚 **Endpoint Descriptions**: XML comments on all endpoints
- 📚 **Example Requests**: Complete cURL and Postman examples

### Additional Features
- 🛡️ **Input Validation**: Data annotations with custom rules
- 🔄 **CORS Support**: Cross-origin request handling
- 📦 **Dependency Injection**: Scoped and singleton services
- 🎯 **Middleware Pipeline**: Ordered processing of requests

---

## Technology Stack

| Component | Version | Purpose |
|-----------|---------|---------|
| .NET Framework | 9.0 | Latest .NET runtime |
| C# Language | 13.0 | Modern language features |
| Swagger | 10.2.3 | API documentation |
| JWT (System.IdentityModel.Tokens.Jwt) | 8.22.0 | Token authentication |
| Microsoft.IdentityModel.Tokens | 8.22.0 | Token validation |
| Visual Studio | 2022+ | IDE |
| Git | Latest | Version control |
| GitHub | - | Repository hosting |

### Development Tools
- GitHub Copilot - AI-assisted code generation
- Visual Studio Terminal - Command-line interface
- REST Client (Visual Studio) - HTTP testing
- Postman - API testing (alternative)

---

## Project Structure
UserManagementAPI/ │ ├── 📁 Controllers/ │   ├── AuthController.cs              # Authentication & token generation │   └── UsersController.cs             # User CRUD operations │ ├── 📁 Middleware/ │   ├── ExceptionHandlingMiddleware.cs     # Global exception handling │   ├── JwtAuthenticationMiddleware.cs     # JWT token validation │   └── RequestResponseLoggingMiddleware.cs # Audit logging │ ├── 📁 Models/ │   └── User.cs                        # User entity with validation │ ├── 📁 Services/ │   ├── IUserService.cs                # User service interface │   ├── UserService.cs                 # User service implementation │   ├── IJwtTokenService.cs            # JWT service interface │   └── JwtTokenService.cs             # JWT token generation │ ├── 📄 Program.cs                      # Application configuration ├── 📄 appsettings.json                # Configuration settings ├── 📄 UserManagementAPI.csproj        # Project file ├── 📄 UserManagementAPI.http          # HTTP test requests ├── 📄 .gitignore                      # Git ignore rules ├── 📄 README.md                       # This file └── 📄 LICENSE                         # MIT License

### Key Components

**Controllers**: Handle HTTP requests and responses
- `AuthController` - Login endpoint to generate JWT tokens
- `UsersController` - CRUD endpoints for user management

**Services**: Implement business logic
- `UserService` - User data management and validation
- `JwtTokenService` - Token generation and configuration

**Middleware**: Process requests/responses
- `ExceptionHandlingMiddleware` - Catches and formats exceptions
- `JwtAuthenticationMiddleware` - Validates tokens on protected endpoints
- `RequestResponseLoggingMiddleware` - Logs all API activity

**Models**: Data structures
- `User` - User entity with validation attributes

---

## Installation

### System Requirements

| Requirement | Version |
|------------|---------|
| .NET SDK | 9.0 or later |
| Operating System | Windows 10+, macOS, Linux |
| RAM | 2GB minimum |
| Disk Space | 500MB |

### Step-by-Step Installation

#### Step 1: Clone Repository
git clone https://github.com/Vontu09/UserManagementAPI.git cd UserManagementAPI

#### Step 2: Restore Dependencies
dotnet restore
