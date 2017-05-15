title: Azure PowerShell Customer Day 2017
author: Aaron Roney
date: 2017-05-14 21:52:13
---
## Lenny Self

Senior Software Engineer, New Relic, https://github.com/geekexplorer.

* Not a lot of powershell 
* Lots of azure stuff going forward 
* For automation, would use a scripting language like Python, not a compiled language, not necessarily 
PowerShell.
* 2 people will be doing automation tasks related to the cloud 
* Do you constantly update scripts, or write and pass off? Bit of both, spinning up resources are more constantly developing, build/test pipleine tends to be the other 
* Do you develop in repl or in editor?  Use both, depends on familiarity - more start in editor 
* Azure: more azure services in the org - different language agents working in Azure - for testing and internal processes.
* Containers? Functions? VM? More VMs but open to the new stuff, especially to increase their surface 
* What would you use to choose? Containers are cool because you can move it, spin up multiple copies, easliy, Being able to run their profile in a container would be more important 
* On the create vm script: 
  * certificates instead of password for the vm 
  * pulling in your windows account credentials and add to the VM 
  * logical defaults for most of the network setup stuff - it is great to have the capability, but this is too difficult to start up with 
  * set up a default using a cmdlet 
  * fill out a couple of fields and default everything else
  * Length of cmdlet name: tab completion makes this less interesting 
  * likes descriptive names by default
  * doesn't mind azure being spelled out, but abbreviations 
  * Set up a default resource group? Likes 
  * How to select images - value completion not important for this? 
  * Prompts with Force are a good way to balance prompting with scriptins 
* Output data for creating vm
* Default size? 
  * Should default to smaller size 
  * Prompt for input plus default 
  * Setting up a session default 
  * Maintain across sessions + override locally
  
## Mark Gu

Product Architect, Xero, https://www.linkedin.com/in/xuangu.

* Company is mainly AWS, not really using Azure.
* Use PowerShell for AWS.
  * Provision VMs.
  * Deploy services.
  * Getting status.
  * Automation.
  * Hooking up a new instance.
* Do not really use the AWS Portal.
* 150 people in organization.  80 engineers.
* 10 using PowerShell/scripting for AWS.
* Using PowerShell from command line and scripts.
* Do a lot of resource management for VMs.
* Maintain a library of scripts that they all share.
* Maintain a lot of troubleshooting scripts to get information back.
* Recently wrote a script to write a script.  For DNS settings, migrating to a DNS zone.
* The love of PowerShell comes from the underlying .NET infrastructure.
* Loves the object model.
* Resisted using PowerShell for a long time, but loved it once he used it.
* Normally goes to MSDN/StackOverflow for help.
* Would only use PowerShell on Windows.  PowerShell Core is limited.
* AWS does not provide PowerShell Core cmdlets.
* Uses full virtual machines.
* Notes on current create VM example:
  * Made a note that you could serialize network variables and reload.
  * Uses "Cloud Formation" (ARM template) to configure AWS VMs.
  * Mostly has existing instances of things, so does not really mind the "longness" of the script for one*off VMs.
* Notes on proposed create VM scenario:
  *  "This is brilliant!"
  * Loves the smart defaults.
  * "What's frustrating at AWS is that if you get something wrong, you just get exception...I Like the 'guided' experience."
  * Prompts should give a "suggested" default that happens when you hit ENTER, but if you want to type  specific name, then you can:
    * Create resource group (MyRg12345)?
    * Choose size (Standard\_DS\_v2)?
    * Choose image (WindowsServer2016)?
* HATES INCONSISTENCIES BETWEEN CMDLETS IN AWS.
* WANTS CONSISTENCY.

## Chris Dickerson

Software Engineer, Technology Service Corporation, https://github.com/dev-cdd/.

* DOD: fips compliance not necessary
* Labview, MatLab/C
* Java to C Bridges
* People really need Labview, don't work with compiled languages
* Labview records data very fast
* Barcode scanning with the last available Windows CE
* Programming on those platforms
* VM: Windows XP, Server 2008
* Programs scanner VMs
* Lots of Windows 10
* PowerShell workflow: Build a product for them to deploy
  * Detecting if I have a DVD Drive
  * Build out a deployment for an existing customer
  * Deploy using your scripts
* Reaction to vm script:
  * It doesn't look terrible
  * Not terifically convoluted, this is actually not that complex
  * Where to go for help: Books on PowerShell
  * Custom modules: os is too old to run it
  * Writes a script
  * No central place for powershell scripts like nuget
  * No nuget ui for powershell gallery
  * No getting started stuff for PowerShell
* New script:
  * Reduced set is way better
  * Important to keep the power
  * Json template is a good idea for creating complex objects
* How would you discover templates?
  * Use the samples to build up your own template
  * Comments inside the sample templates
  * Samples are a preferred method of learning
  * Reference based help doesn't really tell you what you need to know, but the samples sjhow the pieces and the intent

## Jon Cwiak

Software Architect, Humana, https://github.com/binaryjanitor.

* Do not use Azure PowerShell.
* Adopting Azure...heading there _really_ fast.
* SQL, AD, Dynamics, Office 365, Custom Apps...all in Azure.
* External and internal partners.
* Tons of legacy code that they are trying to upgrade.
* His team owns the productivity tools, deployment pipelines, etc.
* Deploys via PowerShell, DSC.
* The scripts will soon become Azure PowerShell ARM templates.
* Loooooves PowerShell...he is trying to script everything and move it to Azure.
* Also looking at Azure Stack.
* Had HP and Cisco in to talk about implementing Azure Stack.
* Very excited to see shared ARM templates across Stack and Global Cloud.
* Not ready to put secrets outside the corporate firewall.
* DoD, government agreements.
* FIPS compliance!!!!1
* DevTest workloads are a must.
* Use a lot of TFS steps, but use DSC/PowerShell for automation.
* REALLY wants a declarative state of the system in source control.
* ALSO want the system, including the DB, infrastructure.
* Would not necessarily gravitate towards ARM template, but a lot of his team would be at a disadvantage when writing scripts because they aren't programmers.
* How do you write your scripts? "Copy/paste a reference example, change what I need."
* They use a Phoenix model.  Burn it to the ground and rebuild!
* He uses PowerShell as command*line ALL THE TIME: health monitoring, API automation, all sorts of things.
* Notes on current "create VM" scenario:
  * Immediate reaction is that this shows me the logical progression.  It shows intent.  Very procedural.
  * "If I am an IT Pro, this makes perfect sense."
  * "To me, it seems awfully verbose.  There's a lot of words."
  * "Lots of opportunities to make a mistake."
* Notes on proposed "create a VM" scenario:
  * "I like DSC.  It is declaritive.  I am describing the 'what' I want instead of 'how' I want."
  * "I like the declarative nature of this...not necessarily because it is shorter."
  * Length of cmdlet name makes almost no difference to you.
  * It is declarative and more easy to read.
  * "DSC is my best friend", and this is like that.