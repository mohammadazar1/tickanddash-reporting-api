<<<<<<< HEAD
﻿# Tick & Dash Reporting Tool API

Reporting Tool API for Tick & Dash application.
=======
﻿# Tick & Dash Backend API

Backend API for Tick & Dash application.
>>>>>>> 01a3a44e9899ac6dc793c8d2bfd6fbbde4507144

## 📋 المشروع

هذا المشروع يحتوي على:
<<<<<<< HEAD
- **TickAndDashReportingTool** - المشروع الرئيسي (Controllers, Services, Startup)
- **TickAndDashDAL** - Data Access Layer (مطلوب كمشروع مرجعي)
=======
- **TickAndDash** - المشروع الرئيسي (Controllers, Services, Startup)
- **TickAndDashDAL** - Data Access Layer (Models, DAL)
- **TickAndDashSharedServices** - Shared Services
>>>>>>> 01a3a44e9899ac6dc793c8d2bfd6fbbde4507144

## 🚀 Build Command

```bash
<<<<<<< HEAD
dotnet restore TickAndDashReportingTool/TickAndDashReportingTool/TickAndDashReportingTool.csproj && dotnet publish TickAndDashReportingTool/TickAndDashReportingTool/TickAndDashReportingTool.csproj -c Release -o ./publish
=======
dotnet restore TickAndDash/TickAndDash/TickAndDash.csproj && dotnet publish TickAndDash/TickAndDash/TickAndDash.csproj -c Release -o ./publish
>>>>>>> 01a3a44e9899ac6dc793c8d2bfd6fbbde4507144
```

## ▶️ Start Command

```bash
<<<<<<< HEAD
dotnet ./publish/TickAndDashReportingTool.dll
=======
dotnet ./publish/TickAndDash.dll
>>>>>>> 01a3a44e9899ac6dc793c8d2bfd6fbbde4507144
```

## ⚙️ Environment Variables

أضف هذه المتغيرات في Render Dashboard:

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__TickAndDash=Server=tcp:tickanddash-server.database.windows.net,1433;Initial Catalog=TickAndDash;Persist Security Info=False;User ID=tickadmin;Password={Mhamd@12345};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

## 📝 ملاحظات

<<<<<<< HEAD
- **.NET Version:** .NET 8.0
- **Database:** Azure SQL Database (نفس قاعدة البيانات الرئيسية)
=======
- **.NET Version:** .NET Core 3.1
- **Database:** Azure SQL Database
>>>>>>> 01a3a44e9899ac6dc793c8d2bfd6fbbde4507144
- **Port:** Render will assign a port automatically
- **CORS:** Make sure CORS is enabled in `Startup.cs` for your frontend domain

## 🔗 API Endpoints

- Swagger UI: `https://your-service.onrender.com/swagger`
<<<<<<< HEAD
- API Base: `https://your-service.onrender.com/api/report/`

## 📚 الملفات المهمة

- `TickAndDashReportingTool/TickAndDashReportingTool/Startup.cs` - إعدادات CORS والخدمات
- `TickAndDashReportingTool/TickAndDashReportingTool/Program.cs` - نقطة البداية
- `TickAndDashReportingTool/TickAndDashReportingTool/appsettings.json.example` - قالب للإعدادات

## ⚠️ ملاحظة مهمة

هذا المشروع يحتاج إلى `TickAndDashDAL` لأنه مشروع مرجعي. تأكد من أن البنية كالتالي:

```
TickAndDash-Reporting/
├── TickAndDashReportingTool/
│   └── TickAndDashReportingTool/
│       └── TickAndDashReportingTool.csproj
└── TickAndDash/
    └── TickAndDashDAL/
        └── TickAndDashDAL.csproj
```

المسار في `.csproj` يجب أن يكون: `..\..\TickAndDash\TickAndDashDAL\TickAndDashDAL.csproj`
=======
- API Base: `https://your-service.onrender.com/api/v1/`

## 📚 الملفات المهمة

- `TickAndDash/TickAndDash/Startup.cs` - إعدادات CORS والخدمات
- `TickAndDash/TickAndDash/Program.cs` - نقطة البداية
- `TickAndDash/TickAndDash/appsettings.json.example` - قالب للإعدادات
>>>>>>> 01a3a44e9899ac6dc793c8d2bfd6fbbde4507144

---

**ملاحظة:** لا ترفع `appsettings.json` مع كلمة المرور إلى GitHub!
