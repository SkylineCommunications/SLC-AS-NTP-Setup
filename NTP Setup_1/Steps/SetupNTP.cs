namespace NTP_Setup_1.Steps
{
	using System;
	using System.IO;

	using Skyline.DataMiner.Utils.Linux;

	public class SetupNTP : IInstallerAction
	{
		private NTPSetupModel model;

		private string description = "Installing NTP...";

		public SetupNTP(NTPSetupModel model)
		{
			this.model = model;
		}

		public string Description { get { return description; } }

		InstallationStepResult IInstallerAction.TryRunStep(ILinux linux)
		{
			try
			{
				if (model.IsOnline.Value)
				{
					OnlineSetup(linux);
				}
				else
				{
					OfflineSetup(linux);
				}

				return new InstallationStepResult(true, $"Successfully installed NTP.");
			}
			catch (Exception e)
			{
				return new InstallationStepResult(false, $"Failed to setup NTP.{Environment.NewLine}{e}");
			}
		}

		private void OfflineSetup(ILinux linux)
		{
			string destination = model.Username == "root"? "/root/NTPSetup" : $"/home/{model.Username}/NTPSetup/";
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