title: Azure PowerShell Experiments
author: Aaron Roney
date: 2017-04-26 15:15:49
---
> What are some good experiments to run for Azure PowerShell?

## Introduction

Each proposed experiment discusses four facets at length.
* Customer: the customers impacted by the problem.
* Problem: the identified issue with the product.
* Concept: the proposed solution, including the hypothesis.
* Feature: the product changes which will evaluate the hypothesis.

Where applicable, experiments can be run by releasing experimental modules and soliciting feedback from a subsequent survey; in addition, a live user study with an interview may provide valuable insight into how users interact with Azure PowerShell.

A live interview can answer overarching questions.  For example, where do users go when they get stuck?
* `Get-Help`?
* Docs?
* Google?
* Stack Overflow?

## End Users

Each experiment category has the same set of end users.  While some experiment categories may serve internal customers or partner teams, the primary end users must be the centerpiece of every experiment.  While some changes may benefit partner teams, we must ensure that such a benefit is also transferred, in prt, to end users.  This benefit may be as simple as "users get to try new features more often" or "users will be able to author their scripts twice as quickly".

These Azure PowerShell end users can be split into two main categories (henceforth deemed "**User**"):
1. DevOps and IT Professionals: typically engineering or authoring scripts to serve some business goal.
2. Software Engineers: hobbyists or "creators" who are running one-off commands.

Each of these customers can be split by level expertise with either Azure or PowerShell ("**Expertise**"):
1. Exisiting Azure PowerShell users.
2. Existing PowerShell users new to Azure.
3. Existing Azure users new to PowerShell.
4. New users to both Azure and PowerShell.

## Cmdlet Design

Design new Azure PowerShell cmdlets which would help Azure PowerShell customers get started faster with less complexity, leading to better user experience and adoption of Azure PowerShell.

### Customer

Improved cmdlet design would largely target _newer_ users, and they would especially benefit newer PowerShell users.  We believe that cmdlet complexity may be a barrier to entry for those in **Expertise 3** and **Expertise 3**, while those in **User 1** and **Expertise 1** likely have existing scripts which would not be impacted heavily by cmdlet redesigns.

All customers can be impacted positively by improving cmdlet design, but we feel that the target customer for such improvements are _new adopters_.

### Problem

In general, Azure PowerShell cmdlets are overly complex; this complexity is often exacerbated in POST or PUT scenarios, while GET (and to a lesser degree PATCH) scenarios tend to be simple.  This complexiy is largely due to the history of Azure PowerShell as the de facto standard for Azure automation scenarios without a strong enough investment in the quality of its production.  The hub and spoke model maintained by the Azure PowerShell team has led towards partner teams emphasizing the _availability_ or _coverage_ of their cmdlets over their _quality_.

This lack of emphasis on quality by resource providers without a strong review process by a central authority has led to some ugly looking code for some standard scenarios.  Take, for example, the "create VM" scenario in Azure PowerShell.

```powershell
# Variables for common values
$resourceGroup = "myResourceGroup"
$location = "westeurope"
$vmName = "myVM"

# Create user object
$cred = Get-Credential -Message "Enter a username and password for the virtual machine."

# Create a resource group
New-AzureRmResourceGroup -Name $resourceGroup -Location $location

# Create a subnet configuration
$subnetConfig = New-AzureRmVirtualNetworkSubnetConfig -Name mySubnet -AddressPrefix 192.168.1.0/24

# Create a virtual network
$vnet = New-AzureRmVirtualNetwork -ResourceGroupName $resourceGroup -Location $location `
  -Name MYvNET -AddressPrefix 192.168.0.0/16 -Subnet $subnetConfig

# Create a public IP address and specify a DNS name
$pip = New-AzureRmPublicIpAddress -ResourceGroupName $resourceGroup -Location $location `
  -Name "mypublicdns$(Get-Random)" -AllocationMethod Static -IdleTimeoutInMinutes 4

# Create an inbound network security group rule for port 3389
$nsgRuleRDP = New-AzureRmNetworkSecurityRuleConfig -Name myNetworkSecurityGroupRuleRDP  -Protocol Tcp `
  -Direction Inbound -Priority 1000 -SourceAddressPrefix * -SourcePortRange * -DestinationAddressPrefix * `
  -DestinationPortRange 3389 -Access Allow

# Create a network security group
$nsg = New-AzureRmNetworkSecurityGroup -ResourceGroupName $resourceGroup -Location $location `
  -Name myNetworkSecurityGroup -SecurityRules $nsgRuleRDP

# Create a virtual network card and associate with public IP address and NSG
$nic = New-AzureRmNetworkInterface -Name myNic -ResourceGroupName $resourceGroup -Location $location `
  -SubnetId $vnet.Subnets[0].Id -PublicIpAddressId $pip.Id -NetworkSecurityGroupId $nsg.Id

# Create a virtual machine configuration
$vmConfig = New-AzureRmVMConfig -VMName $vmName -VMSize Standard_D1 | `
Set-AzureRmVMOperatingSystem -Windows -ComputerName $vmName -Credential $cred | `
Set-AzureRmVMSourceImage -PublisherName MicrosoftWindowsServer -Offer WindowsServer -Skus 2016-Datacenter -Version latest | `
Add-AzureRmVMNetworkInterface -Id $nic.Id

# Create a virtual machine
New-AzureRmVM -ResourceGroupName $resourceGroup -Location $location -VM $vmConfig
```

Azure CLI 2.0, for example, has implemented a much stronger and opinionated central authority, which has led to _scenario_-focused commands.  Let us contrast the Azure PowerShell implementation with the Azure CLI 2.0 implementation.

```bash
#!/bin/bash

# Update for your admin password
AdminPassword=ChangeYourAdminPassword1

# Create a resource group.
az group create --name myResourceGroup --location westus

# Create a virtual machine. 
az vm create \
    --resource-group myResourceGroup \
    --name myVM \
    --image win2016datacenter \
    --admin-username azureuser \
    --admin-password $AdminPassword \
    --no-wait
```

While **Azure PowerShell and Azure CLI 2.0 do not target the same audience, and they do not share the exact same goals**, it is quite clear that the differences are stark.  In addition, it is likely that the complexities of these scenarios in Azure PowerShell present a clear problem for newcomers to Azure PowerShell.

#### Appendix

Create a VM for GCloud.

```powershell
$ config = New-GceInstanceConfig "webserver-1" -MachineType "n1-standard-4" -DiskImage (Get-GceImage -Family "windows-2012-r2")
$ config | Add-GceInstance -Project MyRg -Zone "us-central1-b"
```

Create a VM for AWS.

```powershell
$ New-EC2Instance -ImageId ami-c49c0dac -MinCount 1 -MaxCount 1 -KeyName myPSKeyPair -SecurityGroups myPSSecurityGroup -InstanceType t1.micro
```

### Concept

We plan on identifying five (5) to ten (10) key POST, PUT, or PATCH scenarios which have been identified as overly complex, difficult to use, or common enough to warrant improvement.  These scenarios will be improved as part of an experiment, and will be tested against _all_ user groups, including those that we believe may not benefit from such improvements.

These scenarios will be studied by providing users with such functionality and having them answer a survey.  In addition, where applicable, we will conduct a human-computer interaction study where an interviewer instructs users to complete a task using the both the current and proposed cmdlets.

**We believe new cmdlet designs will improve customer experience and likelihood of long term adoption.**

### Feature

A preview module containing the cmdlets pertaining to the scenarios selected above will be authored.  The cmdlet improvements will focus on _scenario_-based usability and _scenario_-based smart defaults.

## Command Length

Shorten Azure PowerShell cmdlet names which would help Azure PowerShell customers develop their applications and scripts more quickly, leading to better user experience and adoption of Azure PowerShell.

### Customer

Shortened cmdlet names would impact all users positively, and it would impact users at each **Expertise** level positively.  Those in **User 1** will have a much easier time writing scripts, with much less time to completion of cmdlet names, and thoe in **User 2** would likely achieve tab completion much more rapidly.

### Problem

In general, Azure PowerShell cmdlet names are too long.  As of today, each cmdlet takes the following form.

```powershell
Verb-AzureRm
```

where `Verb` is the common PowerShell verb (e.g., `Get`, `Update`, `Select`).  For a user using code completion in a text editor, or tab completion in the PowerShell terminal, a minimum of eleven (11) characters must be typed before any meaningful completion can take place.

In addition, some cmdlet names have gotten out of hand with some cmdlets approaching seventy-five (75) characters in length.  Most users will use completion before typing the full cmdlet name; however, there are some resource providers that ship long prefixes.

```powershell
Unregister-AzureRmRecoveryServicesBackupManagementServer -AzureRmBackupManagementServer $BMS
```

In the case of these cmdlets, a user must type **INSERT NUMBER OF CHARACTERS HERE** characters before any meaningful completion can take place for and cmdlet **INSERT RESOURCE PROVIDER NAME** ships.

### Concept

We plan on identifying areas where cmdlet names can be shortened.  The standard prefix (i.e., `AzureRm`) could be shortened to `Az` or the like.  In addition, resource providers can be encouraged to reduce their prefix length.  This is currently varied, with resource providers like Virtual Machines using the prefix `AzureRmVm` and others like thos mentioned above.

These scenarios will be studied by providing users with such functionality and having them answer a survey.  In addition, where applicable, we will conduct a human-computer interaction study where an interviewer instructs users to complete a task using the both the current and proposed cmdlets.

If the experiment validates the hypothesis, then static analysis tools would be employed to achieve certain guidelines.

**We believe shorter cmdlet names will improve customer experience while both authoring scripts or using the REPL.**

### Feature

A preview module with a few shortened cmdlet names will be authored.  The cmdlet improvements will focus on both consumability and reduced length (i.e., if cmdlets cannot be properly identified due to extreme brevity, then the hypothesis would be partially invalidated).

## Preview Modules

Release preview modules which would help resource providers get early crucial feedback on experimental features, leading to better Azure PowerShell cmdlets and user experience in the long run.

### Customer

Preview module support largely benefits the product team and resource providers.  Both the product team and resource providers often find themselves in need of a way to validate cmdlet designs with users in the form of experimental or preview features.  Simplifying the preview module process would make it easier for Microsoft teams to receive rapid feedback on improvement ideas.

While the end user does not directly benefit from preview modules, preview modules ensure that teams can get early feedback to eventually make the best cmdlets possible.  In the end, when an experimental or preview feature GAs, it will have undergone user validation which validates its usefulness.

### Problem

Azure PowerShell currently has no concept of "experimental" or "preview" features.  At present, service temas which absolutely require such a feature must work extensively with the Azure PowerShell team to ship out-of-band, which causes wasted time for the Azure PowerShell team.

The current difficulties with releasing separate modules and releasing out-of-band discourages the product team and partner teams from running experiments which may have provided valuable feedback from customers.  Without this feeback, cmdlets are often overly complex, and improvements to their design would require a breaking change which the product team makes great pains to avoid.

### Concept

We plan on building preview module functionality in the hopes that it will validate itself over time.  If only the product group engages in experimental and preview modules, the feature will have been validated.

**We believe that preview modules will drastically improve Azure PowerShell over time by providing the product group and feature teams with the ability to rapidly solicit feedback from customers.**

### Feature

The product team is currently adding this functionality to the product.

## Conclusion

Science!