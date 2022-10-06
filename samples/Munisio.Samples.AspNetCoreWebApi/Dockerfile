#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["samples/Munisio.Samples.AspNetCoreWebApi/Munisio.Samples.AspNetCoreWebApi.csproj", "samples/Munisio.Samples.AspNetCoreWebApi/"]
RUN dotnet restore "samples/Munisio.Samples.AspNetCoreWebApi/Munisio.Samples.AspNetCoreWebApi.csproj"
COPY . .
WORKDIR "/src/samples/Munisio.Samples.AspNetCoreWebApi"
RUN dotnet build "Munisio.Samples.AspNetCoreWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Munisio.Samples.AspNetCoreWebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Munisio.Samples.AspNetCoreWebApi.dll"]