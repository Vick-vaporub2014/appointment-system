# Usamos Nginx para servir los archivos estáticos de Blazor WASM
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Copiamos los archivos publicados de Blazor
COPY bin/Release/net9.0/publish/wwwroot .

# Exponemos el puerto 5000
EXPOSE 5000

# Arrancamos Nginx
CMD ["nginx", "-g", "daemon off;"]
