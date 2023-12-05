namespace NTP_Setup_1.Steps
{
	using System;
	using System.IO;

	using Skyline.DataMiner.Utils.Linux;

	public class RestartNTPService : IInstallerAction
	{
		private string description = "Restarting NTP service...";

		public string Description { get { return description; } }
		InstallationStepResult IInstallerAction.TryRunStep(ILinux linux)
		{
			try
			{
				string command;
				string res;

				command = $"sudo systemctl restart ntp";
				res = linux.Connection.RunCommand(command);

				return new InstallationStepResult(true, $"Successfully restarted NTP service.");
			}
			catch (Exception e)
			{
				return new InstallationStepResult(false, $"Failed to restart NTP service.{Environment.NewLine}{e}");
			}
		}
	}
}