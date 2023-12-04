namespace NTP_Setup_1
{
	using NTP_Setup_1.Steps;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.Linux;
	using Skyline.DataMiner.Utils.Linux.Communication;
	using Skyline.DataMiner.Utils.Linux.Debian;
	using Skyline.DataMiner.Utils.Linux.OperatingSystems;

	using System;
	using System.Collections.Generic;

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

		internal static IEnumerable<IInstallerAction> GetInstallationSteps(Engine engine, NTPSetupModel model)
		{
			List<IInstallerAction> steps = new List<IInstallerAction>();

			steps.AddRange(DownloadAndInstallSteps(model));

			return steps;
		}

		internal static IEnumerable<IInstallerAction> DownloadAndInstallSteps(NTPSetupModel model)
		{
			List<IInstallerAction> steps = new List<IInstallerAction>();

			switch (model.AsHost.Value)
			{
				default:
				case true:
					steps.Add(new SetupServer(model));
					break;
				case false:
					steps.Add(new SetupClient(model));
					break;
			}

			return steps;
		}
	}
}
