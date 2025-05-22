# ✅ to-so-api

**to-so-api** is a clean and modular task management API built with **ASP.NET Core**. It supports full **JWT-based authentication**, user registration, and categorized task tracking with categories and subcategories.

---

## 🚀 Features

- 🔐 Secure JWT authentication
- 👤 User registration and login
- 📁 Category & subcategory system for organizing tasks
- 📝 Full CRUD for tasks, categories, and subcategories
- 📦 RESTful controllers for clean separation of concerns
- 🧪 Easy to test and extend

---

## 📦 Tech Stack

- ASP.NET Core 8+
- Entity Framework Core
- JWT Authentication
- SQL Server / SQLite (configurable)

---

## 🧱 Models Overview

### 👤 User
- `Id`: int
- `FirstName`: string
- `LastName`: string
- `Email`: string
- `Password`: string
- `Role`: string
- Has many `Categories`

### 📁 Category
- `Id`: int
- `Name`: string
- `IconName`: string
- `IconType`: string
- `ColorHash`: string
- `UserId`: foreign key to `User`
- Has one `User`
- Has many `Subcategories`
- Has many `Task`

### 🗂️ Subcategory
- `Id`: int
- `Name`: string
- `IconName`: string
- `IconType`: string
- `ColorHash`: string
- `CategoryId`: foreign key to `Category`
- Has one `Category`
- Has many `Task`

### 📝 Task
- `Id`: int
- `Title`: string
- `SubTitle`: string
- `Description`: string
- `Checked`: bool
- `Date`: DateTime
- `CategoryId`: foreign key to `Category`
- `SubcategoryId`: foreign key to `Subcategory`
- Has one `Category`
- Has one `Subcategory`
