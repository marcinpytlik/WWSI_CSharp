# syntax=docker/dockerfile:1
FROM nginx:1.27-alpine
COPY demonstracje/43_SignalRApiSql/www /usr/share/nginx/html
