# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY BlazorWasmStandaloneAuthZeroDemo.sln ./
COPY ./ApplicationClient/ApplicationClient.csproj /app/ApplicationClient/

RUN dotnet restore "./ApplicationClient/ApplicationClient.csproj"
COPY . ./
RUN dotnet publish "./ApplicationClient/ApplicationClient.csproj" -c Release -o out

# This stage is used to publish the service project to be copied to the final stage
FROM nginx:1.23.0-alpine
WORKDIR /app
EXPOSE 80
#EXPOSE 443

# Copy Nginx configuration
COPY ./ApplicationClient/nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/out/wwwroot /usr/share/nginx/html