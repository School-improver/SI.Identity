#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SI.Identity/SI.Identity.csproj", "SI.Identity/"]
RUN dotnet restore "SI.Identity/SI.Identity.csproj"
COPY . .
WORKDIR "/src/SI.Identity"
RUN dotnet build "SI.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SI.Identity.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SI.Identity.dll"]