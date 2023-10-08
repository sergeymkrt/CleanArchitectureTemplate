FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CleanArchitectureTemplate.WebApi/CleanArchitectureTemplate.WebApi.csproj", "CleanArchitectureTemplate.WebApi/"]
COPY ["CleanArchitectureTemplate.Domain/CleanArchitectureTemplate.Domain.csproj", "CleanArchitectureTemplate.Domain/"]
COPY ["CleanArchitectureTemplate.Infrastructure.Persistence/CleanArchitectureTemplate.Infrastructure.Persistence.csproj", "CleanArchitectureTemplate.Infrastructure.Persistence/"]
COPY ["CleanArchitectureTemplate.Infrastructure.Shared/CleanArchitectureTemplate.Infrastructure.Shared.csproj", "CleanArchitectureTemplate.Infrastructure.Shared/"]
COPY ["CleanArchitectureTemplate.Application/CleanArchitectureTemplate.Application.csproj", "CleanArchitectureTemplate.Application/"]

RUN dotnet restore "CleanArchitectureTemplate.WebApi/CleanArchitectureTemplate.WebApi.csproj"
COPY . .
WORKDIR "/src/CleanArchitectureTemplate.WebApi"
RUN dotnet build "CleanArchitectureTemplate.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CleanArchitectureTemplate.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CleanArchitectureTemplate.WebApi.dll"]
