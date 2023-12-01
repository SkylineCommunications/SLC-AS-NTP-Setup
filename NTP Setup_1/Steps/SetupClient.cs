namespace NTP_Setup_1.Steps
{
    using SLDataGateway.API.Collections.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SetupClient : ILinuxAction
    {
        private NTPSetupModel model;

        public SetupClient(NTPSetupModel model)
        {
            this.model = model;
        }

        InstallationStepResult ILinuxAction.TryRunStep(ILinux linux)
        {
            string command;
            string res;

            if (model.IsOffline)
            {
                OfflineSetup(linux);
            }
            else
            {
                OnlineSetup(linux);
            }

            command = $"sudo timedatectl set-ntp off";
            res = linux.Connection.RunCommand(command);

            command = $@"echo ""server {model.Server} prefer iburst"" | sudo tee /etc/ntp.conf";
            res = linux.Connection.RunCommand(command);

            command = $"sudo systemctl restart ntp";
            res = linux.Connection.RunCommand(command);

            return new InstallationStepResult(true, $"Successfully installed NTP as client.");
        }

        private void OfflineSetup(ILinux linux)
        {
            var filePath = "opensearch/ntp_4.2.8p15+dfsg-1ubuntu2_amd64.deb";
            var command = $"sudo dpkg -i {filePath}";
            var res = linux.Connection.RunCommand(command);
        }

        private void OnlineSetup(ILinux linux)
        {
            var command = $"sudo apt-get install -y ntp";
            var res = linux.Connection.RunCommand(command);
        }
    }
}
