# Etapa 1: Compilación / Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos los proyectos
COPY ["Erdyka.Api/Erdyka.Api.csproj", "Erdyka.Api/"]
COPY ["Erdyka.Web/Erdyka.Web.csproj", "Erdyka.Web/"]

# Restauramos dependencias
RUN dotnet restore "Erdyka.Web/Erdyka.Web.csproj"

# Copiamos todo el código fuente
COPY . .

# Publicamos la aplicación
WORKDIR "/src/Erdyka.Web"
RUN dotnet publish "Erdyka.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Imagen final / Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Erdyka.Web.dll"]