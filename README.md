# NTP Setup

## About

Automation script to setup NTP on a Linux server, as either a server or a client.

## Usage

The script can be executed with an **empty json** or with pre-specified inputs for custom installation, for example:

```cs
{
	"host": "10.1.100.101",
	"username": "myUser",
	"password": "myPassword",
	"isSilent": "true",
	"asServer": "false",
	"server": "10.1.100.102",
	"isOnline": "false",
	"packageFolderPath": "C:\\Skyline DataMiner\\Documents\\DMA_COMMON_DOCUMENTS\\InstallPackages\\NTPSetup\\4.2.8"
}
```
|Field|Description|Remarks|
|-|-|-|
|host| Ip address of the target Linux server for NTP Setup| Required for silent installation |
|username| Username credentials to access the Linux server specified under the 'host' field| Required for silent installation |
|password| Password credentials to access the Linux server specified under the 'host' field| Required for silent installation |
|isSilent| Specifies if the NTP setup should run silently or with GUI| Either 'true' or 'false' (defaults to false if not specified)|
|asServer| Specifies if the Linux server should be setup as a server or as a client | Either 'true' for server or 'false' for client (Required for silent installation )|
|server| Specifies the Server Ip Address to sync with, if setup as a client | Used together with the 'asServer' field (Required for silent installation of NTP client) |
|isOnline| Specifies if the NTP setup should proceed with network capability | Can be excluded to let the script detect network capability |
|packageFolderPath| Specifies the local path of the unzipped software bundle for NTP setup | Can be created using Package Manager [SLC-AS-ManagePackages](https://github.com/SkylineCommunications/SLC-AS-ManagePackages). <br/> Required for silent installation without network capability. |


## Running the script from another Automation Script

```cs
using Skyline.DataMiner.Automation;

/// <summary>
/// DataMiner Script Class.
/// </summary>
public class Script
{
	/// <summary>
	/// The Script entry point.
	/// </summary>
	/// <param name="engine">Link with SLAutomation process.</param>
	public void Run(Engine engine)
	{
			
		// Prepare a subscript
		SubScriptOptions subScript = engine.PrepareSubScript("NTP Setup");

		// Prepare Input param
		string input = $@"
{{
	""host"": ""10.1.100.101"",
	""username"": ""myUser"",
	""password"": ""myPassword"",
	""isSilent"": ""true"",
	""asServer"": ""false"",
	""server"": ""10.1.100.102"",
	""isOnline"": ""false"",
	""packageFolderPath"": ""C:\\Skyline DataMiner\\Documents\\DMA_COMMON_DOCUMENTS\\InstallPackages\\NTPSetup\\4.2.8""
}}
";

		subScript.SelectScriptParam("Input", input);

		// Launch the script
		subScript.StartScript();
	}
}
```
