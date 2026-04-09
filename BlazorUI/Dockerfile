
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos el proyecto
COPY . .
# Publicamos en Release
RUN dotnet publish -c Release -o /app/publish

# Usamos Nginx para servir los archivos estáticos de Blazor WASM
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copiamos los archivos publicados
COPY --from=build /app/publish/wwwroot .

# Exponemos el puerto 5000
EXPOSE 5000

# Arrancamos Nginx
CMD ["nginx", "-g", "daemon off;"]
