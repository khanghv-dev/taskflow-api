# 🚀 TaskFlow API

**TaskFlow API** là một RESTful Web API được xây dựng bằng **ASP.NET Core 8**, cung cấp các chức năng quản lý công việc (Task Management) với xác thực JWT, quản lý danh mục, và nhiều tính năng khác.

---

## 📋 Mục lục

- [Tính năng](#-tính-năng)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Cấu trúc dự án](#-cấu-trúc-dự-án)
- [Hướng dẫn cài đặt](#-hướng-dẫn-cài-đặt)
- [Cấu hình](#-cấu-hình)
- [Chạy ứng dụng](#-chạy-ứng-dụng)
- [Tài khoản test mẫu](#-tài-khoản-test-mẫu)
- [API Endpoints](#-api-endpoints)
- [Thành viên nhóm](#-thành-viên-nhóm)

---

## ✨ Tính năng

- **Xác thực & Phân quyền**: Đăng ký, đăng nhập với JWT Bearer Token.
- **Quản lý danh mục (Category)**: CRUD danh mục công việc.
- **Quản lý công việc (Task)**: CRUD công việc theo danh mục.
- **Validation**: Data Annotations + Custom Validation (FutureDateAttribute).
- **Global Exception Handling**: Middleware xử lý lỗi tập trung, trả JSON chuẩn.
- **Data Seeding**: Dữ liệu mẫu sẵn sàng khi khởi tạo database.
- **Swagger UI**: Giao diện test API trực quan với hỗ trợ JWT.

---

## 🛠 Công nghệ sử dụng

| Công nghệ | Phiên bản |
|---|---|
| .NET SDK | 8.0 |
| ASP.NET Core Web API | 8.0 |
| Entity Framework Core | 8.0 |
| SQL Server | 2019+ |
| Swashbuckle (Swagger) | 6.x |
| BCrypt.Net | 4.x |
| JWT Bearer Authentication | 8.0 |

---

## 📁 Cấu trúc dự án

```
TaskFlow-API/
├── Controllers/
│   ├── AuthController.cs          # Đăng ký, đăng nhập
│   ├── CategoriesController.cs    # CRUD danh mục
│   └── DemoSecretController.cs    # API demo phân quyền
├── Data/
│   └── AppDbContext.cs            # DbContext + Seed Data
├── DTOs/
│   ├── AuthDtos.cs                # DTO xác thực
│   ├── CreateCategoryDto.cs       # DTO tạo danh mục
│   └── CategoryResponseDto.cs     # DTO response danh mục
├── Middlewares/
│   └── ExceptionMiddleware.cs     # Xử lý lỗi tập trung
├── Migrations/                    # EF Core Migrations
├── Models/
│   ├── User.cs                    # Entity User
│   ├── Category.cs                # Entity Category
│   └── TaskItem.cs                # Entity TaskItem
├── Services/
│   └── TokenService.cs            # Tạo JWT Token
├── Validation/
│   └── FutureDateAttribute.cs     # Custom validation ngày tương lai
├── Program.cs                     # Entry point & cấu hình
├── appsettings.json               # Cấu hình ứng dụng
└── README.md                      # Tài liệu dự án
```

---

## 🚀 Hướng dẫn cài đặt

### Yêu cầu hệ thống

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (hoặc SQL Server Express / LocalDB)
- [Git](https://git-scm.com/)

### Clone dự án

```bash
# Clone repository về máy
git clone https://github.com/<your-username>/taskflow-api.git

# Di chuyển vào thư mục dự án
cd taskflow-api
```

### Cập nhật code mới nhất

```bash
# Pull code mới nhất từ nhánh chính
git pull origin main
```

---

## ⚙ Cấu hình

### Connection String

Mở file `appsettings.json` và cập nhật `ConnectionStrings` phù hợp với SQL Server của bạn:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "Chuoi_Khoa_Bi_Mat_Cua_Lead_Dai_Tren_32_Ky_Tu_123456789!",
    "Issuer": "AuthDemoApi",
    "Audience": "AuthDemoClient"
  }
}
```

> **Lưu ý:**
> - Nếu dùng **SQL Server Express**: `Server=.\\SQLEXPRESS;...`
> - Nếu dùng **LocalDB**: `Server=(localdb)\\mssqllocaldb;...`
> - Đảm bảo `TrustServerCertificate=True` nếu chạy local.

---

## ▶ Chạy ứng dụng

### 1. Khôi phục packages

```bash
dotnet restore
```

### 2. Tạo/Cập nhật Database

```bash
# Tạo migration mới (nếu có thay đổi model)
dotnet ef migrations add AddCategoryAndTaskItem

# Cập nhật database (tạo bảng + seed data)
dotnet ef database update
```

### 3. Chạy ứng dụng

```bash
dotnet run
```

Ứng dụng sẽ chạy tại:
- **HTTPS**: `https://localhost:5001` (hoặc port trong `launchSettings.json`)
- **Swagger UI**: `https://localhost:5001/swagger`

---

## 🔑 Tài khoản test mẫu

Bạn có thể tạo tài khoản test qua API `/api/auth/register`:

| Trường | Giá trị |
|---|---|
| **Username** | `testuser` |
| **Email** | `testuser@example.com` |
| **Password** | `Test@123` |

### Các bước test:

1. **Đăng ký** tài khoản qua `POST /api/auth/register`:
   ```json
   {
     "username": "testuser",
     "email": "testuser@example.com",
     "password": "Test@123"
   }
   ```

2. **Đăng nhập** qua `POST /api/auth/login`:
   ```json
   {
     "username": "testuser",
     "password": "Test@123"
   }
   ```

3. **Copy JWT Token** từ response.

4. Trên Swagger UI, nhấn nút **🔓 Authorize**, dán token vào ô input và nhấn **Authorize**.

5. Gọi các API yêu cầu xác thực (ví dụ: `POST /api/categories`).

---

## 📡 API Endpoints

### 🔐 Authentication

| Method | Endpoint | Mô tả | Auth |
|---|---|---|---|
| POST | `/api/auth/register` | Đăng ký tài khoản mới | ❌ |
| POST | `/api/auth/login` | Đăng nhập, nhận JWT Token | ❌ |

### 📂 Categories

| Method | Endpoint | Mô tả | Auth |
|---|---|---|---|
| GET | `/api/categories` | Lấy toàn bộ danh mục | ❌ |
| POST | `/api/categories` | Tạo danh mục mới | ✅ JWT |

### Response Format

Tất cả API trả về định dạng JSON chuẩn:

```json
{
  "success": true,
  "message": "Thông báo kết quả.",
  "data": { }
}
```

Khi có lỗi (qua ExceptionMiddleware):

```json
{
  "success": false,
  "message": "Mô tả lỗi.",
  "data": null
}
```

---

## 👥 Thành viên nhóm

| STT | Thành viên | Phần việc |
|---|---|---|
| 1 | Thành viên 1 | Authentication (Register/Login), JWT Token |
| 2 | Thành viên 2 | Task Management (CRUD TaskItem) |
| 3 | Thành viên 3 | Category Management, Validation, Exception Middleware, Data Seeding, Tài liệu |

---

## 📄 License

Dự án này được phát triển cho mục đích học tập.

---

> 📌 **Ghi chú**: Sau khi clone về, hãy chạy `dotnet ef database update` trước khi `dotnet run` để đảm bảo database được tạo với dữ liệu seed sẵn.
