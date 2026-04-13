
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia el proyecto
COPY . .
# Publicamos en Release
RUN dotnet publish -c Release -o /app/publish

#Nginx para servir los archivos estáticos de Blazor WASM
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
#Se limpia archivos basura
RUN rm -rf ./* 
# Copia los archivos publicados
COPY --from=build /app/publish/wwwroot .

# Copia la configuración personalizada de Nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expon el puerto 80
EXPOSE 80

# Arrancamos Nginx
CMD ["nginx", "-g", "daemon off;"]
