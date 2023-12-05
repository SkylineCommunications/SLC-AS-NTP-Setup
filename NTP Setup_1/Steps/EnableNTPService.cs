namespace NTP_Setup_1.Steps
{
	using System;
	using System.IO;

	using Skyline.DataMiner.Utils.Linux;

	public class EnableNTPService : IInstallerAction
	{
		InstallationStepResult IInstallerAction.TryRunStep(ILinux linux)
		{
			try
			{
				string command;
				string res;

				command = $"sudo systemctl enable ntp";
				res = linux.Connection.RunCommand(command);

				return new InstallationStepResult(true, $"Successfully enabled NTP service.");
			}
			catch (Exception e)
			{
				return new InstallationStepResult(false, $"Failed to enabled NTP service.{Environment.NewLine}{e}");
			}
		}
	}
}