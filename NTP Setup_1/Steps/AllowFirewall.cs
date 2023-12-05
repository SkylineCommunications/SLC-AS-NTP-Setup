namespace NTP_Setup_1.Steps
{
	using System;
	using System.IO;

	using Skyline.DataMiner.Utils.Linux;

	public class AllowFirewall : IInstallerAction
	{
		private string description = "Allowing NTP through firewall...";

		public string Description { get { return description; } }
		InstallationStepResult IInstallerAction.TryRunStep(ILinux linux)
		{
			try
			{
				string command;
				string res;

				command = $"sudo ufw allow ntp";
				res = linux.Connection.RunCommand(command);

				return new InstallationStepResult(true, $"NTP added to UFW firewall exceptions.");
			}
			catch (Exception e)
			{
				return new InstallationStepResult(false, $"Failed to add NTP to UFW firewall exceptions.{Environment.NewLine}{e}");
			}
		}
	}
}