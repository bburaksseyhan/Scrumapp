FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS base
WORKDIR /app
#EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["scrum-ui.csproj", "./"]
RUN dotnet restore "scrum-ui.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "scrum-ui.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "scrum-ui.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "scrum-ui.dll"]
