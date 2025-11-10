# Tick & Dash Backend API

Backend API for Tick & Dash application.

## 📋 المشروع

هذا المشروع يحتوي على:
- **TickAndDash** - المشروع الرئيسي (Controllers, Services, Startup)
- **TickAndDashDAL** - Data Access Layer (Models, DAL)
- **TickAndDashSharedServices** - Shared Services

## 🚀 Build Command

```bash
dotnet restore TickAndDash/TickAndDash/TickAndDash.csproj && dotnet publish TickAndDash/TickAndDash/TickAndDash.csproj -c Release -o ./publish
```

## ▶️ Start Command

```bash
dotnet ./publish/TickAndDash.dll
```

## ⚙️ Environment Variables

أضف هذه المتغيرات في Render Dashboard:

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__TickAndDash=Server=tcp:tickanddash-server.database.windows.net,1433;Initial Catalog=TickAndDash;Persist Security Info=False;User ID=tickadmin;Password={Mhamd@12345};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

## 📝 ملاحظات

- **.NET Version:** .NET Core 3.1
- **Database:** Azure SQL Database
- **Port:** Render will assign a port automatically
- **CORS:** Make sure CORS is enabled in `Startup.cs` for your frontend domain

## 🔗 API Endpoints

- Swagger UI: `https://your-service.onrender.com/swagger`
- API Base: `https://your-service.onrender.com/api/v1/`

## 📚 الملفات المهمة

- `TickAndDash/TickAndDash/Startup.cs` - إعدادات CORS والخدمات
- `TickAndDash/TickAndDash/Program.cs` - نقطة البداية
- `TickAndDash/TickAndDash/appsettings.json.example` - قالب للإعدادات

---

**ملاحظة:** لا ترفع `appsettings.json` مع كلمة المرور إلى GitHub!
