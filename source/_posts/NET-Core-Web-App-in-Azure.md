---
title: .NET Core Web App in Azure
date: 2017-02-03 15:49:24
tags:
    - azure
    - dotnet
    - azure-cli
    - vscode
    - csharp
    - microsoft
categories:
    - Built with Azure Tools
---

{% raw %}
<style>
#burn:before {
  font-family: FontAwesome;
  content: "\f06d";
}
</style>
{% endraw %}

<!-- toc -->

# Introduction

Let's build a .NET Core Web App in Azure!  We will be performing all operations using the [Azure CLI](https://github.com/Azure/azure-cli), and all of our work will be built using [Visual Studio Code](https://code.visualstudio.com/) on [Bash On Windows](https://msdn.microsoft.com/en-us/commandline/wsl/about), Linux, Mac OS, or a container (we'll containerize our app in a few weeks).

## Prerequisites

* [Required] [.NET Core](https://www.microsoft.com/net/core).
* [Required] [Azure Subscription](https://azure.microsoft.com/en-us/free/).
* [Required] [Azure CLI](https://github.com/Azure/azure-cli) ([install guide](https://docs.microsoft.com/en-us/cli/azure/install-az-cli2)).
* [Required] [git](https://git-scm.com/downloads).
* [Visual Studio Code](https://code.visualstudio.com/).
* [Bash On Windows](https://msdn.microsoft.com/en-us/commandline/wsl/about).

# Build

## Create a .NET Core Web App

### Create the app

First, we are going to create a new .NET Web App.  I am going to make a simple "Hello, World!" app.  To your favorite **shell**!

```bash
mkdir netcore_hello_world
cd netcore_hello_world
dotnet new -t Web
```

Next, we need to test that our app works locally.

```bash
dotnet restore
dotnet run
```

At this point, your shell will block, and you can test your app by navigating to the URL specified (in my case, it is http://localhost:5000/).  Navigate to this URL, and you should see something like this.

{% asset_img default_app.png The default landing page for a new .NET Core Web App. %}

### Add a Web API endpoint

I have always wanted to have a website respond to my name, so I am going to add a Web API enpoint to my app which will respond the way I want.  You can add any endpoint you would like here, so have it respond in Klingon: it's your app, do what you want.

Awesomely enough, ASP.NET Core web apps have Web API routing built in.  In the `Controllers` directory, all I need do is simply emulate `HomeController.cs` to some degree.  My new controller is going to be pretty simple since I just want to say "hello".  So, I simply create a new controller (`Controllers/ApiController.cs`), and I add my controller class to it (usings and namespace ommitted).

```csharp
public class ApiController : Controller
{
    [HttpGet]
    [Route("api/hello/{name}")]
    public IActionResult SayHello(string name)
    {
        if(name == "Drumpf")
            return this.BadRequest("Drumpf?...Really?");

        var result = new { response = $"Hello, Awesome {name}‽" };
        
        return this.Ok(result);
    }
}
```

You will notice a few key points here:
* `[HttpGet]` informs the runtime that this method represents an HTTP GET endpoint.
* `[Route("api/hello/{name}")]` informs the runtime that the endpoint will be located at `api/hello`, and it takes a parameter, `{name}`.  This `name` parameter is reflected in the method signature of `SayHello`.
* By convention, we are returning an `IActionResult`, which allows us to easily send status codes with our content (e.g., `this.Forbid`, `this.Ok`).

Next, let's try our change.  First, `Ctrl+C` to stop the local web server, and restart the server.

```bash
dotnet run
```

Navigate to the endpoint, and include your name (e.g., http://localhost:5000/api/hello/Aaron).  You should see the expected response.

```json
{ "response" : "Hello, Aaron‽" }
```

If "Drumpf" tries, we will get the expected error result (i.e., `Drumpf?...Really?`, with a `400` status code). {% raw %} <span id="burn"></span> {% endraw %}

## Publish to Azure

### Configure Azure CLI

```bash
az login
az account set 
    --subscription "Aaron Personal (MSDN)"
```

### Create a new web app on Azure

Using the Azure CLI, we can easily create a new web app instance on Azure.

```bash
# Create a demo group for cleanup.
az group create 
    -l westus 
    -n DemoGroup 

# Create a shared app service plan ($10 / month).
az appservice plan create 
    -g DemoGroup 
    -n DemoAppPlan 
    --sku D1 # F1 for free tier (no custom domains).

# Name must be unique to all of Azure.
az appservice web create 
    -g DemoGroup 
    -p DemoAppPlan 
    -n AaronDemoHelloApp 
```

Now, we can verify the host name of our newly created app.

```bash
$ az appservice web show 
    -g DemoGroup 
    -n AaronDemoHelloApp 
    --query hostNames --out tsv
aarondemohelloapp.azurewebsites.net
```

Navigating to this address (i.e., http://aarondemohelloapp.azurewebsites.net/) yields a default Azure web app screen.

### Deploy to Azure via git

At the moment, we are just eager to get our app up on Azure.  For now, we will configure deployment using local git.

```bash
$ az appservice web source-control config-local-git 
    -g DemoGroup 
    -n AaronDemoHelloApp 
    --out tsv
https://twitchax@aarondemohelloapp.scm.azurewebsites.net/AaronDemoHelloApp.git
```

However, we need to create a username and password for this deployment endpoint.  Let's set our deployment credentials through the Azure CLI.

```bash
az appservice web deployment user set 
    --user-name twitchax
```

You will be prompted to set a password, and that's it for deployment authentication!

Now, we can push our app straight from our shell with git.  

```bash
# Create a repository and make the first commit.
git init
git add .
git commit -m "First commit!"

# Add the Azure endpoint (make sure to use your endpoint).
git remote add azure https://twitchax@aarondemohelloapp.scm.azurewebsites.net/AaronDemoHelloApp.git

# Push this deploy directory to Azure.
git push azure master

# Restart the app service (optional).
az appservice web restart 
    -g DemoGroup 
    -n AaronDemoHelloApp
```

### Revel in your awesomeness

Navigate to your azure domain address (i.e., http://aarondemohelloapp.azurewebsites.net/api/hello/Aaron) to test your deployment.  It's alive!

# Share (optional)

At this point, we can share our domain name with the world!  However, if we would like, we can fairly easily buy a custom domain and point that new domain (or a subdomain) at our new web app.

## Obtain a domain name

Go buy a domain name.  I mean, why not?  Most of them are as low as **$12 / year**.  I, personally, use [Google Domains](https://domains.google/#/), but you can use your favorite service.

## Configure a domain name

### Configure CNAME with your registrar

I added a CNAME record for `helloapp.twitchax.com` to `aarondemohelloapp.azurewebsites.net`.  In Google Domains, you just find your domain and click "DNS".  Then, add a "custom resource record" which points to your web app.

{% asset_img add_cname.png Add a CNAME in Google Domains. %}

### Bind the hostname in Azure

Azure requires that we specify which custom domains are allowed to point to our web app.  So, back to the trusty Azure CLI, and we can bind to our custom domain name with one, simple command.

```bash
az appservice web config hostname add 
    -g DemoGroup 
    --webapp AaronDemoHelloApp 
    -n helloapp.twitchax.com
```

**NOTE: There is currently a bug ([#1984](https://github.com/Azure/azure-cli/issues/1984)) in Azure CLI which prevents adding a host name, but a [proposed fix](https://github.com/Azure/azure-cli/pull/1985) is on the way!  This same operation can be completed in the Azure Portal for the time being (Settings >> Custom domains).**

# Done

That's it!  In about 10 minutes, we have built our web app and pushed it to Azure!

Next week, we will add some data to our web app, so stay tuned.