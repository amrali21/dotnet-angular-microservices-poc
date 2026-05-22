FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5052
EXPOSE 5052
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "nextjs-backend.dll"]
