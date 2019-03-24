FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base

# Set environment variables


WORKDIR /app


FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Engaze.EventSubscriber/Engaze.EventSubscriber.csproj", "Engaze.EventSubscriber/"]



# Restore NuGet packages
RUN dotnet restore "Engaze.EventSubscriber/Engaze.EventSubscriber.csproj"
COPY . .
WORKDIR "/src/Engaze.EventSubscriber"

# Build
RUN dotnet build "Engaze.EventSubscriber.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Engaze.EventSubscriber.csproj" -c Release -o /app


FROM base AS final
WORKDIR /app
COPY --from=publish /app .

# Run the app
ENTRYPOINT ["dotnet", "Engaze.EventSubscriber.dll"]