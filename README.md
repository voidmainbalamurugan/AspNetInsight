# AspNetInsight 
	- An instrumentation tool for ASP.NET written in C#

![Build status](https://nle-abcdef.visualstudio.com/Git-AspNetInsight/_apis/build/status/Dev-AspNetInsight) ![License Apache](https://img.shields.io/github/license/nleabcdef/AspNetInsight.svg)

<p align="center">
  <img src="https://raw.githubusercontent.com/nleabcdef/AspNetInsight/master/common/AspNetInsight4.png"/>
</p>

AspNetInsight is a light weight and plug & play tool for ASP.NET applications, to instrument and collect performance counter data of your dynamic pages/content running under ASP.NET;
It intersect ASP.NET pipeline and collect necessary performance data related to each request such as total response time, page/handler processing time.. etc
And also It gives you an insight about your site's performance in terms of response time and bandwidth!

<p align="center">
  <img src="https://raw.githubusercontent.com/nleabcdef/AspNetInsight/master/common/Html-widget.png" />
</p>


# features
  - Collect performance counters for every request, served by ASP.NET
 	- Total time spent, Handler(IHttpHandler) processing time
    - Response body size in bytes
  - Produces live **insight** about your site
    - Minimum, Average, Maximum response time of your app
    - Minimum, Average, Maximum response body size of your app
  - Embed live **insight-widget** into your website/application
  
# configuration support, include
  - Ability to Install AspNetInsight at machine(IIS) or individual site level
  - Configure to collect performance counters in **silent mode**; configure to show live insight on demand at site level
  - Preserve performance data in **memory** or **SQLite data store**
	- both machine(IIS) or individual site level configurations are allowed

# installation support, include
  - **Plug and play installation** with single dll file, without any prerequisites.
  - Install AspNetInsight in **GAC** and configure it on individual site level
  - **AspNetInsight.Installer command line tool** for your specific deployment needs such as **Continues Delivery** or manual or XCOPY
	- for both installation and un-installation

# Library Dependencies – build for
	.NET Framework 4.0 (CLR V4)
	
# Supported ASP.NET framework
	ASP.NET 4.0 and above (which supports .NET CLR V4)

# Installation
### 1) Download latest release from GitHub
 > https://github.com/nleabcdef/AspNetInsight/releases

 ###### Please note, AspNetInsight.Installer.exe supports IIS7.0 and above
 ###### Use **AspNetInsight.dll** for XCOPY deployments.

### 2a) Using AspNetInsight.Installer

 open Windows Command prompt as "**Run as administrator**" mode

- Installation - install it in Global Assembly Cache (GAC)
	 ```dos
	> AspNetInsight.Installer.exe /i
	 ```
- Configuration - configure your local IIS site by site-name 
	 ```dos
	> AspNetInsight.Installer.exe /cs "site-name.domain.com"
	 ```
- Installation - install it in GAC and all IIS sites running under .NET CLR V4
	 ```dos
	> AspNetInsight.Installer.exe /ic
	 ```

- Un-installation - remove from GAC and IIS sites
	 ```dos
	> AspNetInsight.Installer.exe /r
	 ```

- for help,
	 ```dos
	> AspNetInsight.Installer.exe /?
	 ```
### 2b) XCOPY deployment or installation
- Prerequisite, 
	- copy the **AspNetInsight.dll** into your app's **bin** folder or install it in **Global Assembly Cache**
 
- Install the AspNetInsight HTTP Module in IIS 6.0 and IIS 7.0 **Classic Mode**
	```xml
	<configuration>
		<system.web>
		 <httpModules>
			<add name="si_ResponseTracker" 
			type="AspNetInsight4.ResponseTracker, AspNetInsight4, Version=1.0.0.0, Culture=neutral, PublicKeyToken=fd287cc2521f79a3"/>
		 </httpModules>
		</system.web>
	</configuration>
	```

- Install the AspNetInsight HTTP Module in IIS 7.0 and above **Integrated Mode**
	```xml
	<configuration>
	 <system.webServer>
		<modules>
			<add name="si_ResponseTracker" 
			type="AspNetInsight4.ResponseTracker, AspNetInsight4, Version=1.0.0.0, Culture=neutral, PublicKeyToken=fd287cc2521f79a3" preCondition="managedHandler"/>
		</modules>
	 </system.webServer>
	</configuration>
	```
for more deployment and manual configuration options, refer: https://msdn.microsoft.com/en-us/library/ms227673.aspx

### 3) Setup permissions

AspNetInsight module uses **system's temporary directory (TEMP)** to extract its dependency dlls based on current target platform (x86 or x64).
Please provide necessary access(R,W,M and Delete) to the account/user on which your application's app pool is running!

	- for example in Window 10,
		its %WINDIR%\TEMP
		or %WINDIR%\TMP

### 4) You are done ! 
- next step is configure html widget in your app.

# Configuration

by default AspNetInsight configure your Asp.Net site to collect performance data in silent mode, but as a Site Administrator you will be allowed to disable response tracking at your site level and/or configure to show live insight html widget.

-  add the below app setting into your web.config file
    -  to enable or disable live insight html widget    
		```xml
		<!-- possible values - "yes" or "no" -->
		<add key="ShowAspNetInsightBanner" value="yes" />
		 ```
	-  to enable or disable respsone tracking    
		```xml
		<!-- possible values - "yes" or "no" -->
		<add key="AspNetInsightEnabled" value="no" />
		 ```
# License : Apache License 2.0
 - read more here http://www.apache.org/licenses/ 