title: Azure PowerShell Feature Interviews
date: 2017-06-12 00:53:14
---
<!-- toc -->

## Motivation

Previous customer feedback has validated our primary hypotheses:

> We believe Azure developers are frustrated when deploying Azure VMs/Web apps from PowerShell because **it requires multiple lines of complex code**.


## General

* How do you use PowerShell currently? 
* How many on your team use Powershell? 
* Do you share or distribute scripts and templates? How do you share these currently? 
* How do you decide when to create a template or re-usable script versus entering commands?
* How often do you write scripts?  What are these scripts doing? 
* What is the most important item that we could fix, change, or add to Powershell that would help you the most? 
* Where do you go to find help on Powershell? How do you find what you need?

## VM Interview

### Current

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

### Cmdlet Design

#### Hypotheses

* We believe that **cmdlets with smart defaults** will reduce _complexity_ and be valuable to Azure developers when they are **deploying VMs to Azure**.
* We believe that **cmdlets with interactive prompts** will reduce _complexity_ and be valuable to Azure developers when they are **deploying VMs to Azure**.
* We believe that **cmdlets with auto-generated defaults for parameters that are not explicitly provided** will reduce _complexity_ and be valuable to Azure developers when they are **deploying VMs to Azure**.
* We believe that **user-settable defaults** will reduce _complexity_ and be valuable to Azure developers when they are **deploying multiple VMs to Azure within a single session**.
* We believe that **semantic sizes for pricing** will solve _uncertainty of pricing_ and be valuable to Azure developers when they are **deploying multiples VMs/Web apps to Azure within a single session**.


#### Questions

* Get live feedback from users on their general thoughts.
* Stack rank each of these features.
* How would you use each of these features at the REPL?
* How would you use each of these features in a script?
* Any of these options strike you as particularly confusing?
* Any of these options strike you as particularly awesome?

#### Azure PowerShell to automates everything

All parameters autogenerated.

```powershell
$ Login-AzAccount
$ New-AzVm
$
```

#### Azure PowerShell guides users with smart prompts

Required parameters are prompted.

```powershell
$ Login-AzAccount
$ New-AzVm -Name MyVm -Image WinServer2016

Resource group options:
   [Default] MyRg12345
   [1] MyRg1
   [2] MyRg2
Enter your selection or a new resource group (leave blank for default): MyCustomRgName
$
```

#### Azure PowerShell provides an autogenerate mechanism

Unprovided parameters are autogenerated.

```powershell
$ Login-AzAccount
$ New-AzVm -Auto
$
```

```powershell
$ Login-AzAccount
$ New-AzVm -Name MyAwesomeVm -Auto
$
```

#### Azure PowerShell has settable defaults

A default resource group is persisted for this session.

```powershell
$ Login-Az
$ New-AzRg MyRg | Set-AzDefaultRg
$ New-AzVm -Name MyVm -Image WinServer2016
$
```

#### Azure PowerShell has semantic sizes

A VM size that costs less than or equal to $50/month will be selected.

```powershell
$ Login-Az
$ New-AzVm -Name MyVm -ResourceGroup MyRg -Image Ubuntu -Size 50
$
```

### Cmdlet Output

#### Hypotheses

* We believe that **providing a output regarding smart defaults and dependent resources** will reduce _complexity_ and be valuable to Azure developers when they are **deploying VMs to Azure**.
* We believe that **providing helpful next steps** will reduce _complexity_ and be valuable to Azure developers when they are **deploying VMs to Azure**.

#### Questions

* Stack rank each of the output options.

#### Azure PowerShell returns a `PSObject`

```powershell
$ Login-AzAccount
$ $result = New-AzVm -Name MyAwesomeVm -Auto
$
```

#### Azure PowerShell outputs smart defaults and dependent resources

```powershell
$ Login-AzAccount
$ New-AzVm -Name MyAwesomeVm -Auto

Name: MyAwesomeVm.
Resource group: MyRg12345.
Image: WinServer2016.
NIC: NicName.
NSG: NSGName.
  NSG Rule: Open, RDP (3389).
  NSG Rule: Open, PSRemote (5985).
  NSG Rule: Closed, all.
Public IP: 21.32.43.54.
Size: Standard_DS1_v2.
$
```

#### Azure PowerShell outputs helpful next steps

```powershell
$ Login-Az
$ New-AzRg MyRg | Set-AzDefaultRg
$ New-AzVm -Name MyVm -Image Ubuntu -Size 50

Connect to the VM with `ssh myvmdnslabel.centralus.cloudapp.azure.com`.
$
```


## Web App Interview

### Current

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
$ git push azure master
```

### Cmdlet Design

#### Hypotheses

* We believe that **cmdlets with smart defaults** will reduce _complexity_ and be valuable to Azure developers when they are **deploying web apps to Azure**.
* We believe that **cmdlets with interactive prompts** will reduce _complexity_ and be valuable to Azure developers when they are **deploying web apps to Azure**.
* We believe that **providing specific action switches** will reduce _complexity when adding a remote repository for code management_ and be valuable to Azure developers when they are **deploying web apps to Azure**.
* We believe that **returning the code management URI as part of the response ** will reduce _solve complexity when adding a remote repository for code management_ and be valuable to Azure developers when they are **deploying web apps to Azure**.

#### Questions

* Get live feedback from users on their general thoughts.
* Stack rank each of these features.
* How would you use each of these features at the REPL?
* How would you use each of these features in a script?
* Any of these options strike you as particularly confusing?
* Any of these options strike you as particularly awesome?

#### Azure PowerShell automates everything

All parameters autogenerated.  The remote "azure" is added as a git remote if a repository exists.

```powershell
$ Login-Az
$ New-AzWeb
$
```

#### Azure PowerShell guides users with smart prompts

Required parameters are prompted.
```powershell
$ Login-Az
$ New-AzRg MyRg | Set-AzDefaultRg
$ New-AzWeb -Name MyWebApp

Plan options:
   [Default] MyPlan12345
   [1] MyPlan1
   [2] MyPlan2
Enter your selection or a new resource group (leave blank for default): # {Can skip to use default.}

A git repository was detected, would you like to add this web app as a remote named "azure" (Y/n)? Y
$
```

#### Azure PowerShell provides an autogenerate mechanism

Unprovided parameters are autogenerated.  As in "just do it", git remotes are added automatically.

```powershell
$ Login-Az
$ New-AzRg MyRg | Set-AzDefaultRg
$ New-AzWeb -Auto
$
```

#### Azure PowerShell provides specific action switches

```powershell
$ Login-Az
$ New-AzRg MyRg | Set-AzDefaultRg
$ New-AzWeb -Name MyWebApp -Plan MyWebAppPlan -AddRemote

Git repository detected, added remote "azure".
$
```

#### Azure PowerShell returns source code management URI

```powershell
$ Login-Az
$ New-AzRg MyRg | Set-AzDefaultRg
$ $webApp = New-AzWeb -Name MyWebApp -Plan MyWebAppPlan
$ git remote add azure $webapp.ScmUri
$ git push azure master
$
```

### Cmdlet Output

#### Hypotheses

* We believe that **providing a output regarding smart defaults and dependent resources** will reduce _complexity_ and be valuable to Azure developers when they are **deploying web apps to Azure**.
* We believe that **providing helpful next steps** will reduce _complexity_ and be valuable to Azure developers when they are **deploying web apps to Azure**.

#### Questions

* Stack rank each of the output options.

#### Azure PowerShell returns a `PSObject`

```powershell
$ Login-AzAccount
$ $result = New-AzVm -Name MyAwesomeVm -Auto

Resource Group: MyRg12345.
Plan: MyPlan12345.
$
```

#### Azure PowerShell outputs smart defaults and dependent resources

```powershell
$ Login-Az
$ New-AzRg MyRg | Set-AzDefaultRg
$ $webApp = New-AzWeb -Name MyWebApp -Plan MyWebAppPlan

Use `ScmUri` to deploy via git, GitHub or FTP.  For example, you can configure a local git remote with `git remote add azure $result.ScmUri`.
$
```

#### Azure PowerShell outputs helpful next steps

```powershell
$ Login-Az
$ New-AzRg MyRg | Set-AzDefaultRg
$ $webApp = New-AzWeb -Name MyWebApp -Plan MyWebAppPlan

Use `ScmUri` to deploy via git, GitHub or FTP.  For example, you can configure a local git remote with `git remote add azure $result.ScmUri`.
$
```


## Conclusion