@echo off
setlocal

set ROOT=%~dp0

REM Optional parameter: pass "nobuild" to skip building the dotnet services.
REM   run-all.bat            -> build then start everything
REM   run-all.bat nobuild    -> skip the build, just start everything
set BUILD=1
if /i "%~1"=="nobuild" set BUILD=0

if "%BUILD%"=="0" (
    echo ============================================
    echo  Skipping build ^(nobuild^)...
    echo ============================================
    goto :start
)

echo ============================================
echo  Building all projects...
echo ============================================

dotnet build "%ROOT%ledgerly-backend-invoice-service\ledgerly-backend.csproj" --configuration Debug
if %errorlevel% neq 0 ( echo [FAILED] ledgerly-backend build failed & exit /b %errorlevel% )

dotnet build "%ROOT%ledgerly-backend-cust-service\ledgerly-backend-cust-service.csproj" --configuration Debug
if %errorlevel% neq 0 ( echo [FAILED] ledgerly-backend-cust-service build failed & exit /b %errorlevel% )

dotnet build "%ROOT%ledgerly-backend-dashboard-service\ledgerly-backend-dashboard-service.csproj" --configuration Debug
if %errorlevel% neq 0 ( echo [FAILED] ledgerly-backend-dashboard-service build failed & exit /b %errorlevel% )

dotnet build "%ROOT%ledgerly-backend-auth-service\ledgerly-backend-auth-service.csproj" --configuration Debug
if %errorlevel% neq 0 ( echo [FAILED] ledgerly-backend-auth-service build failed & exit /b %errorlevel% )

dotnet build "%ROOT%ledgerly-api-gateway\ledgerly-api-gateway.csproj" --configuration Debug
if %errorlevel% neq 0 ( echo [FAILED] ledgerly-api-gateway build failed & exit /b %errorlevel% )

:start
echo.
echo ============================================
echo  Starting all projects...
echo  ledgerly-backend         -> https://localhost:7052
echo  ledgerly-backend-cust    -> https://localhost:7099
echo  ledgerly-backend-dash    -> https://localhost:7063
echo  ledgerly-backend-auth    -> https://localhost:7109
echo  ledgerly-api-gateway       -> https://localhost:7019
echo  angular-frontend       -> http://localhost:4200
echo ============================================
echo.

start "ledgerly-backend" cmd /k "dotnet run --project "%ROOT%ledgerly-backend-invoice-service\ledgerly-backend.csproj" --launch-profile ledgerly_backend"
timeout /t 3 /nobreak >nul
start "ledgerly-backend-cust-service" cmd /k "dotnet run --project "%ROOT%ledgerly-backend-cust-service\ledgerly-backend-cust-service.csproj" --launch-profile https"
timeout /t 3 /nobreak >nul
start "ledgerly-backend-dashboard-service" cmd /k "dotnet run --project "%ROOT%ledgerly-backend-dashboard-service\ledgerly-backend-dashboard-service.csproj" --launch-profile https"
timeout /t 3 /nobreak >nul
start "ledgerly-backend-auth-service" cmd /k "dotnet run --project "%ROOT%ledgerly-backend-auth-service\ledgerly-backend-auth-service.csproj" --launch-profile https"
timeout /t 3 /nobreak >nul
start "ledgerly-api-gateway" cmd /k "dotnet run --project "%ROOT%ledgerly-api-gateway\ledgerly-api-gateway.csproj" --launch-profile https"
timeout /t 3 /nobreak >nul
start "angular-frontend" cmd /k "cd /d "%ROOT%angular-frontend" && npm start"

echo All 5 backend projects + angular-frontend started in separate windows.
endlocal
