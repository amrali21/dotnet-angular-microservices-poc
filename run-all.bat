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

dotnet build "%ROOT%nextjs-backend-invoice-service\nextjs-backend.csproj" --configuration Debug
if %errorlevel% neq 0 ( echo [FAILED] nextjs-backend build failed & exit /b %errorlevel% )

dotnet build "%ROOT%nextjs-backend-cust-service\nextjs-backend-cust-service.csproj" --configuration Debug
if %errorlevel% neq 0 ( echo [FAILED] nextjs-backend-cust-service build failed & exit /b %errorlevel% )

dotnet build "%ROOT%nextjs-backend-dashboard-service\nextjs-backend-dashboard-service.csproj" --configuration Debug
if %errorlevel% neq 0 ( echo [FAILED] nextjs-backend-dashboard-service build failed & exit /b %errorlevel% )

dotnet build "%ROOT%next-api-gateway\next-api-gateway.csproj" --configuration Debug
if %errorlevel% neq 0 ( echo [FAILED] next-api-gateway build failed & exit /b %errorlevel% )

:start
echo.
echo ============================================
echo  Starting all projects...
echo  nextjs-backend         -> https://localhost:7052
echo  nextjs-backend-cust    -> https://localhost:7099
echo  nextjs-backend-dash    -> https://localhost:7063
echo  next-api-gateway       -> https://localhost:7019
echo  angular-frontend       -> http://localhost:4200
echo ============================================
echo.

start "nextjs-backend" cmd /k "dotnet run --project "%ROOT%nextjs-backend-invoice-service\nextjs-backend.csproj" --launch-profile nextjs_backend"
timeout /t 3 /nobreak >nul
start "nextjs-backend-cust-service" cmd /k "dotnet run --project "%ROOT%nextjs-backend-cust-service\nextjs-backend-cust-service.csproj" --launch-profile https"
timeout /t 3 /nobreak >nul
start "nextjs-backend-dashboard-service" cmd /k "dotnet run --project "%ROOT%nextjs-backend-dashboard-service\nextjs-backend-dashboard-service.csproj" --launch-profile https"
timeout /t 3 /nobreak >nul
start "next-api-gateway" cmd /k "dotnet run --project "%ROOT%next-api-gateway\next-api-gateway.csproj" --launch-profile https"
timeout /t 3 /nobreak >nul
start "angular-frontend" cmd /k "cd /d "%ROOT%angular-frontend" && ng serve"

echo All 4 backend projects + angular-frontend started in separate windows.
endlocal
