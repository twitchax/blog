title: Dockerize your Web App
tags:
  - azure
  - dotnet
  - azure-cli
  - csharp
  - microsoft
  - docker
categories:
  - Built with Azure Tools
author: Aaron Roney
date: 2017-05-05 17:00:00
---
<!-- toc -->

# Introduction

Let's dockerize our web app!  We will be performing all operations using the [Azure CLI](https://github.com/Azure/azure-cli), and all of our work will be built using [Visual Studio Code](https://code.visualstudio.com/) on [Bash On Windows](https://msdn.microsoft.com/en-us/commandline/wsl/about), Linux, Mac OS.  If you have not already, make sure you have a {% post_link NET-Core-Web-App-in-Azure .NET Core Web App %} ready to go.

## Prerequisites

* [Required] [Azure CLI](https://github.com/Azure/azure-cli) ([install guide](https://docs.microsoft.com/en-us/cli/azure/install-az-cli2)).
* [Required] [.NET Core](https://www.microsoft.com/net/core) ([CLI 1.0.0-rc4-004800+](https://github.com/dotnet/cli)).
* [Required] [Azure Subscription](https://azure.microsoft.com/en-us/free/).
* [Required] [git](https://git-scm.com/downloads).
* [Required] [Docker](https://docs.docker.com/engine/installation/).

# Build

> NOTE: all command statements with multiple lines ignore the need for a newline escape.

## Build and pubish docker image

### Dockerfile

First, we need a dockerfile.  Minimally, we need a docker file that looks like this.

```docker
FROM microsoft/aspnetcore
LABEL name="NetCoreHello"
ENTRYPOINT ["dotnet", "NetCoreHello.dll"]
ARG source=bin/Release/netcoreapp1.1/publish
WORKDIR /app
EXPOSE 80
COPY $source .
```

### Build image

Next, let's create a docker image of our web app.

```bash
$ dotnet publish -c Release
$ docker build -t twitchax/netcorehello:v1 .
```

If you would like, you can check that your image was built properly.

```bash
$ docker run -p 80:80 -it twitchax/netcorehello:v1

Hosting environment: Production
Content root path: /app
Now listening on: http://+:80
Application started. Press Ctrl+C to shut down.
```

We can verify that our site is working by navigating to http://localhost/api/name/Aaron (or any endpoint in your app).

### Publish image

Let's pubish our image to docker hub (though, you can use any container registry).

```bash
$ docker login
$ docker push twitchax/netcorehello:v1
```

## Create and deploy web app

First, we need to do some setup if you have not already done so (some of these steps may have been completed before, but we need an `--is-linux` container).

```bash
$ az group create -n DemoGroup -l westus
$ az appservice plan create -n DemoPlan -g DemoGroup --location westus --is-linux --sku B1
$ az appservice web create -n DockerNetCoreHello -p DemoPlan -g DemoGroup
```

Finally, we need to configure our web app to run of our docker image.

```bash
$ az appservice web config container update --docker-custom-image-name twitchax/netcorehello:v1 -n DockerNetCoreHello -g DemoGroup
```

## Test

You can use whatever method you prefer to test your new web app interaction with DocumentDB.  In my case, I am using `curl` with [Bash On Windows](https://msdn.microsoft.com/en-us/commandline/wsl/about).

Get from an endpoint.

```bash
$ curl http://https://dockernetcorehello.azurewebsites.net/api/hello/Aaron
{
  "response": "Hello, Awesome Aaron‽"
}
```

# Done

That's it!  In about 10 minutes, we have dockerized our web app and deployed it to Azure!