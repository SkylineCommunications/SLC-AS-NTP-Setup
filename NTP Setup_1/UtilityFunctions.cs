namespace NTP_Setup_1
{
	using System;
	using System.Collections.Generic;

	using NTP_Setup_1.Steps;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.Linux;
	using Skyline.DataMiner.Utils.Linux.Communication;
	using SLDataGateway.API.Collections.Linq;

	public static class UtilityFunctions
	{
		public static ILinux ConnectToLinuxServer(string host, string username, string password)
		{
			ConnectionSettings settings = new ConnectionSettings(host, username, password);
			ISshConnection connections = SshConnectionFactory.GetSshConnection(settings);
			var linux = LinuxFactory.GetLinux(connections);
			linux.Connection.Connect();

			if (string.IsNullOrWhiteSpace(linux.Connection.RunCommand("whoami")))
			{
				throw new Exception("Connection to server failed, please try again.");
			}

			return linux;
		}

		public static bool NetworkCheck(NTPSetupModel model)
		{
			//var networkCheckResult = model.Linux.Connection.RunCommand($"timeout 0.2 ping -c 1 8.8.8.8 >/dev/null 2>&1 ; echo $?");
			// "0" -> online
			//return networkCheckResult == "0" ? true : false;
			
			var networkCheckResult = model.Linux.Connection.RunCommand("curl -v google.com").Contains("Connected to google.com");
			return networkCheckResult;
		}

		internal static IEnumerable<IInstallerAction> GetInstallationSteps(Engine engine, NTPSetupModel model)
		{
			List<IInstallerAction> steps = new List<IInstallerAction>()
			{
				new DisableTimeDateCtl(),
				new SetupNTP(model),
			};

			switch (model.AsServer.Value)
			{
				default:
				case true:
					steps.Add(new AllowFirewall());
					break;

				case false:
					steps.Add(new ConfigureClientNTP(model));
					break;
			}

			steps.Add(new RestartNTPService());
			steps.Add(new EnableNTPService());

			return steps;
		}
	}
}