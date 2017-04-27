title: Azure PowerShell Strategy
author: Aaron Roney
date: 2017-04-26 15:06:04
---
> What are killer demos for Azure PowerShell?  How do we get there?

## Introduction

As the de facto standard for Azure automation scenarios, Azure PowerShell is ubiquitous in its penetration of the cloud market.  This penetration means that all services across Azure must provide their users with workable Azure PowerShell cmdlets.  Historically, Azure PowerShell was largely owned by a single team; however, over time, the model for Azure PowerShell has become a "hub and spoke", where the Azure PowerShell feature team provides repositories, guidelines, static analysis, reviews, build tools, and publish tools, and service teams own the design and functionality of their cmdlets.  This model functions well, and it is within this model that Azure PowerShell successfully cemented itself as the de facto standard for Azure automation scenarios.

Through the years of this "hub and spoke" model, Azure PowerShell became, for all intents and purposes, a "mature" product.  We say this because most of the practices, guidelines, and processes have been in place and well-oiled for quite some time.  Its maturity has brought high usage and a solid customer base.  However, this maturity and stable complacency comes at a cost to investment in innovation.

Azure PowerShell has become somewhat of a "second thought" to service teams; however, the recent emphasis on Azure CLI 2.0 by management has led this team and others to re-evaluate the value of Azure PowerShell and its place in the each resource provider's lifecycle.  It is with this renewed emphasis on developer tools (especially CLIs) in Azure that our team hopes to accomplish drastic improvements to the Azure PowerShell experience.  This document will view other cloud platforms and Azure CLI 2.0 itself as competitors in an effort to properly determine how Azure PowerShell stacks up with the innovations of other CLIs (both internal and external).

## Compete

### Current Azure PowerShell

Create VM.

```powershell
# Variables for common values
$ $resourceGroup = "myResourceGroup"
$ $location = "westeurope"
$ $vmName = "myVM"

# Create user object
$ $cred = Get-Credential -Message "Enter a username and password for the virtual machine."

# Create a resource group
$ New-AzureRmResourceGroup -Name $resourceGroup -Location $location

# Create a subnet configuration
$ $subnetConfig = New-AzureRmVirtualNetworkSubnetConfig -Name mySubnet -AddressPrefix 192.168.1.0/24

# Create a virtual network
$ $vnet = New-AzureRmVirtualNetwork -ResourceGroupName $resourceGroup -Location $location -Name MYvNET -AddressPrefix 192.168.0.0/16 -Subnet $subnetConfig

# Create a public IP address and specify a DNS name
$ $pip = New-AzureRmPublicIpAddress -ResourceGroupName $resourceGroup -Location $location -Name "mypublicdns$(Get-Random)" -AllocationMethod Static -IdleTimeoutInMinutes 4

# Create an inbound network security group rule for port 3389
$ $nsgRuleRDP = New-AzureRmNetworkSecurityRuleConfig -Name myNetworkSecurityGroupRuleRDP  -Protocol Tcp -Direction Inbound -Priority 1000 -SourceAddressPrefix * -SourcePortRange * -DestinationAddressPrefix * -DestinationPortRange 3389 -Access Allow

# Create a network security group
$ $nsg = New-AzureRmNetworkSecurityGroup -ResourceGroupName $resourceGroup -Location $location -Name myNetworkSecurityGroup -SecurityRules $nsgRuleRDP

# Create a virtual network card and associate with public IP address and NSG
$ $nic = New-AzureRmNetworkInterface -Name myNic -ResourceGroupName $resourceGroup -Location $location -SubnetId $vnet.Subnets[0].Id -PublicIpAddressId $pip.Id -NetworkSecurityGroupId $nsg.Id

# Create a virtual machine configuration
$ $vmConfig = New-AzureRmVMConfig -VMName $vmName -VMSize Standard_D1 | Set-AzureRmVMOperatingSystem -Windows -ComputerName $vmName -Credential $cred | Set-AzureRmVMSourceImage -PublisherName MicrosoftWindowsServer -Offer WindowsServer -Skus 2016-Datacenter -Version latest | Add-AzureRmVMNetworkInterface -Id $nic.Id

# Create a virtual machine
$ New-AzureRmVM -ResourceGroupName $resourceGroup -Location $location -VM $vmConfig
```

Create and deploy web app.

```powershell
$ $gitdirectory="<Replace with path to local Git repo>"
$ $webappname="mywebapp$(Get-Random)"
$ $location="West Europe"

# Create a resource group.
$ New-AzureRmResourceGroup -Name myResourceGroup -Location $location

# Create an App Service plan in `Free` tier.
$ New-AzureRmAppServicePlan -Name $webappname -Location $location -ResourceGroupName myResourceGroup -Tier Free

# Create a web app.
$ New-AzureRmWebApp -Name $webappname -Location $location -AppServicePlan $webappname -ResourceGroupName myResourceGroup

# Configure GitHub deployment from your GitHub repo and deploy once.
$ $PropertiesObject = @{
    scmType = "LocalGit";
}
$ Set-AzureRmResource -PropertyObject $PropertiesObject -ResourceGroupName myResourceGroup -ResourceType Microsoft.Web/sites/config -ResourceName $webappname/web -ApiVersion 2015-08-01 -Force

# Get app-level deployment credentials
$ $xml = (Get-AzureRmWebAppPublishingProfile -Name $webappname -ResourceGroupName myResourceGroup -OutputFile null)
$ $username = $xml.SelectNodes("//publishProfile[@publishMethod=`"MSDeploy`"]/@userName").value
$ $password = $xml.SelectNodes("//publishProfile[@publishMethod=`"MSDeploy`"]/@userPWD").value

# Add the Azure remote to your local Git respository and push your code
#### This method saves your password in the git remote. You can use a Git credential manager to secure your password instead.
$ git remote add azure "https://${username}:$password@$webappname.scm.azurewebsites.net"
git push azure master
```

### Azure CLI 2.0

Create VM.

```bash
$ az group create -n MyRg -l WestUS
$ az vm create -n MyVm -g MyRg --image UbuntuLTS
```

Create and deploy web app.

```bash
$ az group create -n MyRg -l WestUS
$ az appservice plan create -n MyWebAppPlan -g MyRg
$ az appservice web create -n MyWebApp -g MyRg -p MyWebAppPlan
$ url=$(az appservice web source-control config-local-git --name $webappname --resource-group myResourceGroup --query url --output tsv)
$ git remote add azure $url
$ git push azure master
```

### GCloud

Create a VM.

```powershell
$ config = New-GceInstanceConfig "webserver-1" -MachineType "n1-standard-4" -DiskImage (Get-GceImage -Family "windows-2012-r2")
$ config | Add-GceInstance -Project MyRg -Zone "us-central1-b"
```

No good web app story for PowerShell.

### AWS

Create a VM.

```powershell
$ New-EC2Instance -ImageId ami-c49c0dac -MinCount 1 -MaxCount 1 -KeyName myPSKeyPair -SecurityGroups myPSSecurityGroup -InstanceType t1.micro
```

No good web app story for PowerShell.

### Heroku

Create a web app.

```bash
$ heroku create

Creating falling-wind-1624... done, stack is cedar-14
http://falling-wind-1624.herokuapp.com/ | https://git.heroku.com/falling-wind-1624.git
Git remote heroku added

$ git push heroku master
```

### Zeit Now

Create a web app.

```bash
$ now

> Deploying ~/OneDrive/Projects/simpleStaticSiteTest
> Using Node.js 7.7.3 (default)
> Ready! https://simplestaticsitetest-vredymcymy.now.sh (copied to clipboard) [1s]
> Initializing…
> Building
> ▲ npm install
> ⧗ Installing:
>  ‣ serve@5.0.4
> ✓ Installed 178 modules [9s]
> ▲ npm start
> > simpleStaticSiteTest@ start /home/nowuser/src
> > NODE_ENV='production' serve ./content
> Deployment complete!
```

## Proposed Areas of Improvement

These areas of improvements were selected based on key differentiators in competing products.  For example, Azure CLI 2.0 has made a point of basing commands on _scenarios_ rather than _API surface area_; in addition, they have chosen a number of smart defaults which make "getting started" scenarios easier for end uers.  

### Scenario-based Cmdlets

**All** cmdlets should be designed around scenarios, not the Azure REST service.

### Smart Defaults

We will explore defaulting almost everything.  Popular defaults in competing products:
* Resource Group.
* Location.
* Dependent resources.

In many cases, some parameters could be "gray" or semi-optional.  That is, if the parameter is not specified, the user is asked if we should generate the parameter for them.  For example, resource group is required for almost all _create_ scenarios; however, users often want to create a resource without much concern for a specific group (especially during REPL use).  As such, resource group would be a "gray" parameter that is provisioned for the user with a random name if they choose to leave it off.

### Settable Defaults

Users should have the ability to default certain ubiquitous parameters like `-ResourceGroupName` and `-Location`.

### Shorter Names

This includes the names of cmdlets (e.g., `New-AzureRmVM` => `New-AzVm`) and the names of parameters (e.g., `-ResourceGroupName` => `-Rg`).

### Other

Parameter types:
* Required: e.g., name.
* Gray: e.g., resource group.
* Optional: e.g., VM size.

Other strategy ideas:
* Prompts for "gray" parameters.
* Generate script cmdlets.
* Better preview module support.
* Push logic server side.
* Rename "AzureRM" to something else (sort of goes with shorter cmdlet names).

## Killer Demos

### Create VM

```powershell
# In this sample, we show the fact that resource group is a "gray" parameter.
$ Login-AzAccount
$ New-AzVm -Name MyVm -Image UbuntuLTS

A resource group was not specified: would you like to create MyVmRg12345 (Y/n)? Y

Selected Defaults:
  NIC: NicName.
  NSG: NSGName.
    NSG Rule: Open, RDP.
    NSG Rule: Closed, all.
  Public IP: 21.32.43.54
  Size: Standard_DS1_v2

Done!
{PSObject Output}
```

The user should be able to specify certain global defaults, e.g., resource group.

```powershell
# In this sample, we show the user what smart defaults were selected.
$ Login-Az
$ New-AzRg MyRg | Set-AzDefaultRg
$ New-AzVm -Name MyVm -Image UbuntuLTS

Selected Defaults:
  NIC: NicName.
  NSG: NSGName.
    NSG Rule: Open, RDP.
    NSG Rule: Closed, all.
  Public IP: 21.32.43.54
  Size: Standard_DS1_v2

Done!
{PSObject Output}
```

### Create and Deploy Web App

Smart defaults would be chosen.  For example, a "free" plan should be the default web app plan.

```powershell
# In this sample, we show the fact that plan is a "gray" parameter.
$ Login-Az
$ New-AzRg MyRg | Set-AzDefaultRg
$ New-AzWeb -Name MyWebApp

A web app plan was not specified: would you like to create a free plan MyWebAppPlan (Y/n)? Y

A git repository was detected, would you like to add this web app as a remote named "azure" (Y/n)? Y

Would you like to push this git repository to this web app (Y/n)? Y

{A bunch of git output...}

Done!
```

```powershell
# In this sample, we show optional parameters which assist in a later deploy.
$ Login-Az
$ New-AzRg MyRg | Set-AzDefaultRg
$ New-AzWeb -Name MyWebApp -Plan MyWebAppPlan -AddRemote -Deploy

{A bunch of git output...}

Done!
```

```powershell
# This sample gets the deployment endpoint (not trivial currently) and deploys.
$ Login-Az
$ New-AzRg MyRg | Set-AzDefaultRg
$ New-AzWeb -Name MyWebApp -Plan MyWebAppPlan
$ $endpoint = Get-AzWebScm -Name MyWebApp
$ git remote add azure $endpoint
$ git push azure master
```

### Storage scenarios?

### SQL scenarios?

### Network scenarios?

### ACS/ACR/Container scenarios?

### Function app awesomeness?

```powershell
$ Login-Az
$ New-AzRg MyRg | Set-AzDefaultRg
$ New-AzFunction -AppName MyFunctionApp -Source myFunction.js
```

# Conclusion