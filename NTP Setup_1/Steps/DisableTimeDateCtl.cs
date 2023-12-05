namespace NTP_Setup_1.Steps
{
	using System;
	using System.IO;

	using Skyline.DataMiner.Utils.Linux;

	public class DisableTimeDateCtl : IInstallerAction
	{
		private string description = "Setting timedatectl NTP to off...";

		public string Description { get { return description; } }
		InstallationStepResult IInstallerAction.TryRunStep(ILinux linux)
		{
			try
			{
				string command;
				string res;

				command = $"sudo timedatectl set-ntp off";
				res = linux.Connection.RunCommand(command);

				return new InstallationStepResult(true, $"timedatectl ntp set to off.");
			}
			catch (Exception e)
			{
				return new InstallationStepResult(false, $"Unable to set ntp to off on timedatectl.{Environment.NewLine}{e}");
			}
		}
	}
}