namespace NTP_Setup_1.Steps
{
    using SLDataGateway.API.Collections.Linq;
    using Skyline.DataMiner.Utils.Linux;
	using System.IO;
	using Skyline.DataMiner.Utils.Linux.FileSystem;
	using System.Linq;

	public class SetupClient : IInstallerAction
    {
        private NTPSetupModel model;

        public SetupClient(NTPSetupModel model)
        {
            this.model = model;
        }

        InstallationStepResult IInstallerAction.TryRunStep(ILinux linux)
        {
            string command;
            string res;

			command = $"sudo timedatectl set-ntp off";
			res = linux.Connection.RunCommand(command);

			if (model.IsOffline.Value)
            {
                OfflineSetup(linux);
            }
            else
            {
                OnlineSetup(linux);
            }

            command = $@"echo ""server {model.Server} prefer iburst"" | sudo tee /etc/ntp.conf";
            res = linux.Connection.RunCommand(command);

            command = $"sudo systemctl restart ntp";
            res = linux.Connection.RunCommand(command);

			command = $"sudo systemctl enable ntp";
			res = linux.Connection.RunCommand(command);

			return new InstallationStepResult(true, $"Successfully installed NTP as client.");
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
                    var fileNameStartIndex = file.LastIndexOf("\\")+1;
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
