FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base

# Set environment variables
ENV ASPNETCORE_URLS="http://*:5000"
ENV ASPNETCORE_ENVIRONMENT="Development"

WORKDIR /app
EXPOSE 5000
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Engaze.Evento.API/Engaze.Evento.API.csproj", "Engaze.Evento.API/"]
COPY ["Engaze.Evento.ApplicationService/Engaze.Evento.ApplicationService.csproj", "Engaze.Evento.ApplicationService/"]
COPY ["Engaze.Evento.Persistance/Engaze.Evento.Persistance.csproj", "Engaze.Evento.Persistance/"]
COPY ["Engaze.Evento.Domain/Engaze.Evento.Domain.csproj", "Engaze.Evento.Domain/"]
COPY ["Engaze.Evento.Contract/Engaze.Evento.Contract.csproj", "Engaze.Evento.Contract/"]


# Restore NuGet packages
RUN dotnet restore "Engaze.Evento.API/Engaze.Evento.API.csproj"
COPY . .
WORKDIR "/src/Engaze.Evento.API"

# Build
RUN dotnet build "Engaze.Evento.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Engaze.Evento.API.csproj" -c Release -o /app

# Open up port
EXPOSE 5000

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

# Run the app
ENTRYPOINT ["dotnet", "Engaze.Evento.API.dll"]