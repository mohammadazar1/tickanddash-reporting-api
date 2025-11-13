# Use the official .NET 8.0 runtime as base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["TickAndDashReportingTool/TickAndDashReportingTool/TickAndDashReportingTool.csproj", "TickAndDashReportingTool/TickAndDashReportingTool/"]
COPY ["TickAndDash/TickAndDashDAL/TickAndDashDAL.csproj", "TickAndDash/TickAndDashDAL/"]

# Restore dependencies
RUN dotnet restore "TickAndDashReportingTool/TickAndDashReportingTool/TickAndDashReportingTool.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/TickAndDashReportingTool/TickAndDashReportingTool"
RUN dotnet build "TickAndDashReportingTool.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TickAndDashReportingTool.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "TickAndDashReportingTool.dll"]

