# Tick & Dash Reporting Tool API

Reporting Tool API for Tick & Dash application.

## 📋 المشروع

هذا المشروع يحتوي على:
- **TickAndDashReportingTool** - المشروع الرئيسي (Controllers, Services, Startup)
- **TickAndDashDAL** - Data Access Layer (مطلوب كمشروع مرجعي)

## 🚀 Build Command

```bash
dotnet restore TickAndDashReportingTool/TickAndDashReportingTool/TickAndDashReportingTool.csproj && dotnet publish TickAndDashReportingTool/TickAndDashReportingTool/TickAndDashReportingTool.csproj -c Release -o ./publish
```

## ▶️ Start Command

```bash
dotnet ./publish/TickAndDashReportingTool.dll
```

## ⚙️ Environment Variables

أضف هذه المتغيرات في Render Dashboard:

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__TickAndDash=Server=tcp:tickanddash-server.database.windows.net,1433;Initial Catalog=TickAndDash;Persist Security Info=False;User ID=tickadmin;Password={Mhamd@12345};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

## 📝 ملاحظات

- **.NET Version:** .NET 8.0
- **Database:** Azure SQL Database (نفس قاعدة البيانات الرئيسية)
- **Port:** Render will assign a port automatically
- **CORS:** Make sure CORS is enabled in `Startup.cs` for your frontend domain

## 🔗 API Endpoints

- Swagger UI: `https://your-service.onrender.com/swagger`
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

---

**ملاحظة:** لا ترفع `appsettings.json` مع كلمة المرور إلى GitHub!
