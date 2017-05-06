FROM microsoft/aspnetcore
LABEL name="NetCoreHello"
ENTRYPOINT ["dotnet", "NetCoreHello.dll"]
ARG source=bin/Release/netcoreapp1.1/publish
WORKDIR /app
EXPOSE 80
COPY $source .

