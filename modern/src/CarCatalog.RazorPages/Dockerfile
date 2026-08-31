# Build from the repository root so the central package version files are in context:
#   docker build -f modern/src/CarCatalog.RazorPages/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Directory.Packages.props Directory.Build.props Directory.Build.targets ./
COPY modern/Directory.Build.props modern/
COPY modern/src/ modern/src/

RUN dotnet publish modern/src/CarCatalog.RazorPages/CarCatalog.RazorPages.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CarCatalog.RazorPages.dll"]
