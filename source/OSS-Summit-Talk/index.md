title: OSS Summit Talk
author: Aaron Roney
date: 2017-09-13 12:00:00
---

# Tools

* Management
  * [AutoRest](https://github.com/Azure/autorest)
  * CLIs
    * [Azure PowerShell](https://docs.microsoft.com/en-us/powershell/azure/overview):
      * [Repository](https://github.com/Azure/azure-powershell)
      * [Windows](https://docs.microsoft.com/en-us/powershell/azure/install-azurerm-ps): `Install-Module AzureRM`
      * [Mac/Linux](https://docs.microsoft.com/en-us/powershell/azure/install-azurermps-maclinux): `Install-Module AzureRM.Netcore`
      * Docker
        * [Windows](https://hub.docker.com/r/azuresdk/azure-powershell/): `docker run -it azuresdk/azure-powershell`
        * [Ubuntu](https://hub.docker.com/r/azuresdk/azure-powershell-core/): `docker run -it azuresdk/azure-powershell-core`
    * [Azure CLI 2.0](https://docs.microsoft.com/en-us/cli/azure/overview)
      * [Repository](https://github.com/Azure/azure-cli)
      * [Windows](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest#install-on-windows): [MSI](https://aka.ms/InstallAzureCliWindows)
      * [Mac/Linux](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest#a-namemacosinstall-on-macos): `curl -L https://aka.ms/InstallAzureCli | bash`
      * [Docker](https://hub.docker.com/r/azuresdk/azure-cli-python/): `docker run -it azuresdk/azure-cli-python`
  * Languages
    * [.NET](https://docs.microsoft.com/en-us/dotnet/azure/)
      * [Repository](https://github.com/Azure/azure-sdk-for-net)
      * [Libraries](https://www.nuget.org/packages?q=microsoft.azure.management)
    * [Java](https://docs.microsoft.com/en-us/java/azure/) 
      * [Repository](https://github.com/Azure/azure-sdk-for-java)
      * [Libraries](https://search.maven.org/#search%7Cga%7C1%7Cg%3A%22com.microsoft.azure%22)
    * [Node](https://docs.microsoft.com/en-us/nodejs/azure/)
      * [Repository](https://github.com/Azure/azure-sdk-for-node)
      * [Libraries](https://www.npmjs.com/search?q=azure%20managment&page=1&ranking=optimal)
    * [Python](https://docs.microsoft.com/en-us/python/azure/)
      * [Repository](https://github.com/Azure/azure-sdk-for-python)
      * [Libraries](https://pypi.python.org/pypi?%3Aaction=search&term=azure-mgmt&submit=search)
    * Go:
      * [Repository](https://github.com/Azure/azure-sdk-for-go)
      * Libraries: coming soon!
    * Ruby:
      * [Repository](https://github.com/Azure/azure-sdk-for-ruby)
      * [Libraries](https://rubygems.org/search?utf8=%E2%9C%93&query=azure_mgmt).
* Client:
  * [Repository](https://github.com/twitchax/PodcastsSyndicate)
  * Development
    * [VSCode](https://code.visualstudio.com/)
      * [Repository](https://github.com/Microsoft/vscode)
    * [.NET](https://docs.microsoft.com/en-us/dotnet)
      * [Repository](https://github.com/Microsoft/dotnet)
    * [.NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/)
      * [Repository](https://github.com/dotnet/cli)
    * [Cosmos DB](https://docs.microsoft.com/en-us/azure/cosmos-db/)
      * [Repository](https://github.com/Azure/azure-documentdb-dotnet)
      * [Wrapper Repository](https://github.com/twitchax/DocumentDb.Fluent)
      * [Libraries](https://www.nuget.org/packages/DocumentDb.Fluent/)
    * [Redis Cache](https://docs.microsoft.com/en-us/azure/redis-cache)
      * [Repository](https://github.com/StackExchange/StackExchange.Redis/)
      * [Libraries](https://www.nuget.org/packages/StackExchange.Redis/)

# Demo

## What do we have?

Run Azure PowerShell.

```bash
docker run -it --rm azuresdk/azure-powershell-core
```

Get resources.

```powershell
Login-AzureRmAccount
Select-AzureRmSubscription -SubscriptionName "Aaron Personal (MSDN)"
Get-AzureRmResource | Where-Object { $_.ResourceGroupName -eq "PodcastsSyndicate" } | Select -Property Name,ResourceType
```

## Create resources.

Run Azure CLI 2.0

```bash
az login
az redis create --sku Basic -g PodcastsSyndicate -n podcastssyndicateredis -l westus2 --vm-size C0
```

Get redis keys

```bash
redisuri=$(az redis show -n podcastssyndicateredis -g PodcastsSyndicate --query hostName -o tsv)
rediskey=$(az redis list-keys -n podcastssyndicateredis -g PodcastsSyndicate --query primaryKey -o tsv)
```

Set the keys as app settings.

```bash
az webapp config appsettings set -n podcastssyndicate -g PodcastsSyndicate --settings RedisUri=$redisuri RedisKey=$rediskey && clear
```

## Add Redis Cache

Add the package.

```bash
dotnet add package StackExchange.Redis
```

Create a Redis helper.

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using StackExchange.Redis;

namespace PodcastsSyndicate.Dal
{
    public static class Redis
    {
        public static IDatabase Self = ConnectionMultiplexer.Connect($"{Environment.GetEnvironmentVariable("RedisUri") ?? Helpers.Configuration["RedisUri"]},ssl=true,password={Environment.GetEnvironmentVariable("RedisKey") ?? Helpers.Configuration["RedisKey"]}").GetDatabase();

        public static Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None) => Self.StringSetAsync(key, value, expiry, when, flags);

        public static async Task<string> StringGetAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            var value = await Self.StringGetAsync(key, flags);

            return value.IsNullOrEmpty ? null : value.ToString();
        }
    }
}
```

Add new logic.

```csharp
[HttpGet("/podcast/{podcastId}/rss2")]
[HttpHead("/podcast/{podcastId}/rss2")]
[Produces("application/xml")]
public async Task<IActionResult> GetRss2(string podcastId) => Ok((await GetRssXDocument(podcastId)).Root);

private static async Task<XDocument> GetRssXDocument(string podcastId)
{
    var cacheKey = $"rss_{podcastId}";

    var docString = await Redis.StringGetAsync(cacheKey);
    if(docString != null)
        return XDocument.Parse(docString);

    var doc = await GenerateRssXDocument(podcastId);
    await Redis.StringSetAsync(cacheKey, doc.ToString(), TimeSpan.FromHours(1));

    return doc;
}
```

## Deploy

Build docker image.

```bash
dotnet restore
dotnet publish -c Release

docker build bin/Release/netcoreapp2.0/publish -t twitchax/podcastssyndicate
```

Push docker image.

```bash
docker push twitchax/podcastssyndicate
```

Update web app.

```bash
az webapp restart -n podcastssyndicate -g PodcastsSyndicate
```

## Benefit