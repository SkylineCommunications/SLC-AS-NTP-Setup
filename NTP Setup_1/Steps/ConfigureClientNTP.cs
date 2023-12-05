namespace NTP_Setup_1.Steps
{
	using System;
	using System.IO;

	using Skyline.DataMiner.Utils.Linux;

	public class ConfigureClientNTP : IInstallerAction
	{
		private NTPSetupModel model;

		public ConfigureClientNTP(NTPSetupModel model)
		{
			this.model = model;
		}

		InstallationStepResult IInstallerAction.TryRunStep(ILinux linux)
		{
			try
			{
				string command;
				string res;

				command = $@"echo ""server {model.Server} prefer iburst"" | sudo tee /etc/ntp.conf";
				res = linux.Connection.RunCommand(command);

				return new InstallationStepResult(true, $"Successfully configured client NTP settings.");
			}
			catch (Exception e)
			{
				return new InstallationStepResult(false, $"Failed to configure client NTP settings.{Environment.NewLine}{e}");
			}
		}
	}
}