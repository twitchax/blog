---
title: Add DocumentDB to your Web App
date: 2017-02-11 17:49:10
tags:
    - azure
    - dotnet
    - azure-cli
    - csharp
    - microsoft
    - documentdb
    - docker
categories:
    - Built with Azure Tools
---

<!-- toc -->

# Introduction

Let's build add Add [DocumentDB](https://azure.microsoft.com/en-us/services/documentdb/) to your Web App!  We will be performing all operations using the [Azure CLI](https://github.com/Azure/azure-cli), and all of our work will be built using [Visual Studio Code](https://code.visualstudio.com/) on [Bash On Windows](https://msdn.microsoft.com/en-us/commandline/wsl/about), Linux, Mac OS, or a container (we'll containerize our app in a few weeks).  If you have not already, make sure you have a {% post_link NET-Core-Web-App-in-Azure .NET Core Web App %} ready to go!


## Prerequisites

* [Required] [Azure CLI](https://github.com/Azure/azure-cli) ([install guide](https://docs.microsoft.com/en-us/cli/azure/install-az-cli2)).
* [Required] [.NET Core](https://www.microsoft.com/net/core) ([CLI 1.0.0-rc4-004800+](https://github.com/dotnet/cli)).
* [Required] [Azure Subscription](https://azure.microsoft.com/en-us/free/).
* [Required] [git](https://git-scm.com/downloads).
* [Required] [Docker](https://docs.docker.com/engine/installation/).

# Build

> NOTE: all command statements with multiple lines ignore the need for a newline escape.

## Create a DocumentDB with Azure CLI

DocumentDB is one of the cutting edge features available in the Azure CLI, so we need to use a nightly ([it's on the way](https://github.com/Azure/azure-cli/pull/1815)).  I am going to use [Docker](https://www.docker.com/) to keep the latest version of Azure CLI separate from my system configuration.  However, if you prever to use the latest build on your machine without Docker, you can [install the nightly](https://github.com/Azure/azure-cli#nightly-builds).

```bash
docker run -it azuresdk/azure-cli-python:latest
az login
az account set --subscription "Aaron Personal (MSDN)"
```

Next, let's create a new DocumentDB instance.  I am going to add friend-keeping functionality to my app.  I want to keep a list of friends, and some information about them: name, email, phone number, etc.

```bash
az documentdb create -g DemoGroup -n friendsdocdb
```

We can then get the endpoint for the DocumentDB we just created.

```bash
$ az documentdb show -g DemoGroup -n friendsdocdb 
      --query documentEndpoint -o tsv
https://friendsdocdb.documents.azure.com:443/
```

We will need this later, so keep it around.

## Connect your Web App to DocumentDB

### Obtain the primary master key

In order to connect, we need to get our primary master key for the DocumentDB.

```bash
az documentdb regenerate-key -g DemoGroup -n friendsdocdb --key-kind primary
az documentdb list-keys -g DemoGroup -n friendsdocdb 
    --query primaryMasterKey -o tsv
```

We will need this later, so keep it around.

### Add some DocumentDB code

Let's add DocumentDB capabilities to our app by adding the proper NuGet packages to our project.  I have created a nice little library called [DocumentDb.Fluent](https://github.com/twitchax/DocumentDb.Fluent) which drastically improves the DocumentDB interaction experience in .NET.

```bash
dotnet add package DocumentDb.Fluent
dotnet restore
```

In any location, you need to create a static DocumentDB connection generator.  I added a new class called `Helpers` and added my generator; in addition, I created an a `Friend` class as my document type.

```csharp
public static class Helpers
{
    private const string EndpointUri = "<your_endpoint_uri>";
    private const string PrimaryKey = "<your_primary_key>";
    
    public static IDocumentDbInstance DocumentDb => 
        DocumentDbInstance.Connect(EndpointUri, PrimaryKey);
    public static IDatabase Db = DocumentDb.Database("Db");
    public static IDocumentCollection<Friend> Friends => Db.Collection<Friend>();
}

public class Friend : HasId
{
    public string Name { get; set; }
    public string Email { get; set; }
}
```

Next, let's convert the `ValuesController` (from our {% post_link NET-Core-Web-App-in-Azure .NET Core Web App %}) into a `FriendsController`.  I also decided to rename `ValuesController.cs` to `FriendsController.cs`.

You may notice that the [DocumentDb.Fluent](https://github.com/twitchax/DocumentDb.Fluent) library makes all of the calls fairly simple and straightforward.  If you prefer, each of the methods I call has a synchronous version, as well.

```csharp
[Route("api/[controller]")]
public class FriendsController : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(Helpers.Friends.Query);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var friend = await Helpers.Friends.Document(id).ReadAsync();
        return Ok(friend);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody]Friend friend)
    {
        var doc = await Helpers.Friends.Document().CreateAsync(friend);
        friend.Id = doc.Id;
        return Created(doc.Id.ToString(), friend);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody]Friend friend)
    {
        await Helpers.Friends.Document(id).UpdateAsync(friend);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        await Helpers.Friends.ClearAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await Helpers.Friends.Document(id).DeleteAsync();
        return Ok();
    }
}
```

**Optional:** to pretty print JSON, add a formatter to the middleware in `Startup.cs` that looks like this.

```csharp 
services.AddMvc().AddJsonOptions(options => {
    options.SerializerSettings.Formatting = Formatting.Indented;
});
```

## Test

You can use whatever method you prefer to test your new web app interaction with DocumentDB.  In my case, I am using `curl` with [Bash On Windows](https://msdn.microsoft.com/en-us/commandline/wsl/about).

Get friends.

```bash
$ curl http://localhost:5000/api/friends
[]
```

Add friend.

```bash
$ curl -H "Content-Type: application/json" -X POST 
      -d '{ "name": "Chelsey", "email": "an@email.com" }' 
      http://localhost:5000/api/friends
{
  "name": "Chelsey",
  "email": "an@email.com",
  "id": "d98ebc3f-67df-4152-a15a-1ad32d473ad1"
}
```

Update friend.

```bash
$ curl -H "Content-Type: application/json" -X PUT 
      -d '{ "name": "Chelsey", "email": "new@email.com" }' 
      http://localhost:5000/api/friends/d98ebc3f-67df-4152-a15a-1ad32d473ad1
```

Delete one friend.

```bash
$ curl -H "Content-Type: application/json" -X DELETE 
      http://localhost:5000/api/friends/d98ebc3f-67df-4152-a15a-1ad32d473ad1
```

Delete all friends.

```bash
$ curl -H "Content-Type: application/json" -X DELETE 
      http://localhost:5000/api/friends
```

## Deploy

Just as we did when we built our app, we can {% post_link NET-Core-Web-App-in-Azure deploy these changes to Azure with git %}.

```bash
# Push this deploy directory to Azure.
git push azure master

# Restart the app service (optional).
az appservice web restart -g DemoGroup -n AaronDemoHelloApp
```

# Done

That's it!  In about 10 minutes, we have added DocumentDB functionality to our web app!