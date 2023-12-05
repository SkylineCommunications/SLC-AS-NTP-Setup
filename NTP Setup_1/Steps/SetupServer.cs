namespace NTP_Setup_1.Steps
{
	using System;
	using System.IO;

	using Skyline.DataMiner.Utils.Linux;

	public class SetupServer : IInstallerAction
	{
		private NTPSetupModel model;

		public SetupServer(NTPSetupModel model)
		{
			this.model = model;
		}

		InstallationStepResult IInstallerAction.TryRunStep(ILinux linux)
		{
			try
			{
				string command;
				string res;

				command = $"sudo timedatectl set-ntp off";
				res = linux.Connection.RunCommand(command);

				if (model.IsOnline.Value)
				{
					OnlineSetup(linux);
				}
				else
				{
					OfflineSetup(linux);
				}

				command = $"sudo ufw allow ntp";
				res = linux.Connection.RunCommand(command);

				command = $"sudo systemctl restart ntp";
				res = linux.Connection.RunCommand(command);

				command = $"sudo systemctl enable ntp";
				res = linux.Connection.RunCommand(command);

				return new InstallationStepResult(true, $"Successfully installed NTP as host.");
			}
			catch (Exception e)
			{
				return new InstallationStepResult(false, $"Failed to setup NTP.{Environment.NewLine}{e}");
			}
		}

		private void OfflineSetup(ILinux linux)
		{
			string destination = $"/home/{model.Username}/NTPSetup/";
			linux.CreateDirectory(destination);

			var unzippedPackage = model.InstallPackage;
			var localPath = unzippedPackage.Path;

			foreach (var file in Directory.EnumerateFiles(localPath))
			{
				if (file.EndsWith(".deb"))
				{
					var fileNameStartIndex = file.LastIndexOf("\\") + 1;
					var fileName = file.Substring(fileNameStartIndex);
					var destinationFile = linux.UploadFile(file, destination + fileName);
					linux.SoftwareBundleManager.Install(destinationFile.Path);
				}
			}

			linux.Connection.RunCommand($"sudo rm -rf {destination}");
		}

		private void OnlineSetup(ILinux linux)
		{
			var command = $"sudo apt update";
			var res = linux.Connection.RunCommand(command);

			command = $"sudo apt-get install -y ntp";
			res = linux.Connection.RunCommand(command);
		}
	}
}