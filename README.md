# POS System

A complete desktop Point of Sale (POS) and Inventory Management System built with **C# WinForms** and **SQL Server**. Designed for small to medium retail businesses to manage sales, stock, suppliers, and reports in one place.

## Features

- 🔐 Role-Based Login (Admin / Cashier) with session management
- 🧾 Point of Sale (POS) – barcode scanning, split payments (Cash/Card/QR)
- ⏸  Held / Parked Sales – hold a transaction and resume it later
- 📦 Product & Category Management – full CRUD with discount percentages
- 🏷  Barcode Label Printing
- 🚚 Supplier & Purchase Management– stock-in with cost tracking
- 📊 Dashboard – real-time sales, invoices, expenses, low stock alerts, and weekly sales chart
- 📈 Reports– daily/monthly sales, profit & expense, low stock, exportable to **Excel & PDF**
- 💸 Expense Tracking
- 🧮 Stock Adjustment– track damaged/expired/lost stock with full audit history
- 🎁 Promotions – Buy X Get Y Free rules per product
- 🧾 Invoice History & Returns – reprint receipts, process item returns
- 👥 User Management – add/remove system users
- 💾 Database Backup & Restore – built into Settings
- 🌙 Dark Mode Toggle

## Tech Stack

- Frontend: C# Windows Forms (.NET Framework)
- Backend: SQL Server
- Libraries: ClosedXML (Excel export), iTextSharp (PDF export)

## Setup Instructions

### Requirements
- Visual Studio 2022 or later
- SQL Server (Express edition is sufficient)

### Steps
1. Clone this repository
2. Restore the database:
   - Open SQL Server Management Studio (SSMS)
   - Right-click **Databases** → **Restore Database**
   - Select the provided `.bak` file and restore it as `AdvancedPOS`
3. Update the connection string:
   - Open `Helpers/DatabaseConnection.cs`
   - Update the server name to match your SQL Server instance
4. Open `AdvancedPOS.slnx` in Visual Studio
5. **Build → Rebuild Solution**
6. Press `F5` to run

### Default Login
Username: admin
Password: admin123

## Screenshots
<img width="1122" height="717" alt="image" src="https://github.com/user-attachments/assets/820e959e-fba5-4fbb-8d04-23f3ce2cca48" />
<img width="1917" height="1037" alt="image" src="https://github.com/user-attachments/assets/3ebaf87c-40ea-4e62-ae1d-75b28cb9b5e1" />
<img width="1916" height="1015" alt="image" src="https://github.com/user-attachments/assets/11da7fb8-4b32-4946-9651-ccdadbdb609c" />
<img width="1911" height="1016" alt="image" src="https://github.com/user-attachments/assets/e85f2c9f-e0da-4a7f-b103-fdbf4c06aaa6" />


