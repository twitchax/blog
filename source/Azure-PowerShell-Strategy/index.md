title: Azure PowerShell Strategy
date: 2017-04-26 15:06:04
author: Aaron Roney
---

> What are killer demos for Azure PowerShell?  How do we get there?

## Introduction

As the de facto standard for Azure automation scenarios, Azure PowerShell is ubiquitous in its penetration of the cloud market.  This penetration means that all services across Azure must provide their users with workable Azure PowerShell cmdlets.  Historically, Azure PowerShell was largely owned by a single team; however, over time, the model for Azure PowerShell has become a "hub and spoke", where the Azure PowerShell feature team provides repositories, guidelines, static analysis, reviews, build tools, and publish tools, and service teams own the design and functionality of their cmdlets.  This model functions well, and it is within this model that Azure PowerShell successfully cemented itself as the de facto standard for Azure automation scenarios.

Through the years of this "hub and spoke" model, Azure PowerShell became, for all intents and purposes, a "mature" product.  We say this because most of the practices, guidelines, and processes have been in place and well-oiled for quite some time.  Its maturity has brought high usage and a solid customer base.  However, this maturity and stable complacency comes at a cost to innovation investment.

Azure PowerShell has become somewhat of a "second thought" to service teams; however, the recent emphasis on Azure CLI 2.0 by management has led this team and others to re-evaluate the value of Azure PowerShell and its place in the each resource provider's lifecycle.  It is with this renewed emphasis on developer tools (especially CLIs) in Azure that our team hopes to accomplish drastic improvements to the Azure PowerShell experience.  This document will view other cloud platforms and Azure CLI 2.0 itself as competitors in an effort to properly determine how Azure PowerShell stacks up with the innovations of other CLIs (both internal and external).

## Compete

### Azure CLI 2.0

Create VM.

```bash
$ az vm create -n MyVm -g MyRg --image UbuntuLTS
```

Create and deploy web app.

```bash
az appservice web create -n MyWebApp -g MyRg -p MyWebAppPlan
```

### Heroku

### Zeit Now

## Proposed Areas of Improvement

These areas of improvements were selected based on key differentiators in competing products.  For example, Azure CLI 2.0 has made a point of basing commands on _scenarios_ rather than _API surface area_; in addition, they have chosen a number of smart defaults which make "getting started" scenarios easier for end uers.  

### Scenario-based Cmdlets

### Smart Defaults

### Shorter Cmdlet Names (and parameter names?)

### Other

Parameter types:
* Required: e.g., name.
* Grey: e.g., resource group.
* Optional: e.g., VM size.

Other strategy ideas:
* Prompts for "grey" parameters.
* Generate script cmdlets.
* Better preview module support.
* Push logic server side.
* Rename "AzureRM" to something else (sort of goes with shorter cmdlet names)?

## Killer Demos

### Create VM

```powershell
# In this sample, we show the fact that resource group is a "grey" parameter.
PS> Login-AzAccount
PS> New-AzVm -Name MyVm -Image UbuntuLTS

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
PS> Login-Az
PS> New-AzRg MyRg | Set-AzDefaultRg
PS> New-AzVm -Name MyVm -Image UbuntuLTS

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
# In this sample, we show the fact that plan is a "grey" parameter.
PS> Login-Az
PS> New-AzRg MyRg | Set-AzDefaultRg
PS> New-AzWeb -Name MyWebApp

A web app plan was not specified: would you like to create a free plan MyWebAppPlan (Y/n)? Y

A git repository was detected, would you like to add this web app as a remote named "azure" (Y/n)? Y

Would you like to push this git repository to this web app (Y/n)? Y

{A bunch of git output...}

Done!
```

```powershell
# In this sample, we show optional parameters which assist in a later deploy.
PS> Login-Az
PS> New-AzRg MyRg | Set-AzDefaultRg
PS> New-AzWeb -Name MyWebApp -Plan MyWebAppPlan -AddRemote -Deploy

{A bunch of git output...}

Done!
```

```powershell
# This sample gets the deployment endpoint (not trivial currently) and deploys.
PS> Login-Az
PS> New-AzRg MyRg | Set-AzDefaultRg
PS> New-AzWeb -Name MyWebApp -Plan MyWebAppPlan
PS> $endpoint = Get-AzWebScm -Name MyWebApp
PS> git remote add azure $endpoint
PS> git push azure master
```

### Storage scenarios?

### SQL scenarios?

### Network scenarios?

### ACS/Container scenarios?

### Function app awesomeness?

```powershell
PS> Login-Az
PS> New-AzRg MyRg | Set-AzDefaultRg
PS> New-AzFunction -AppName MyFunctionApp -Source myFunction.js # need endpoint?
```

# Conclusion