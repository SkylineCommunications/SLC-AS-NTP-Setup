namespace NTP_Setup_1.Steps
{
    using Skyline.DataMiner.CommunityLibrary.Linux;
    using Skyline.DataMiner.CommunityLibrary.Linux.Actions.ActionSteps;
    using System;

    public class SetupServer : ILinuxAction
    {
        private NTPSetupModel model;

        public SetupServer(NTPSetupModel model)
        {
            this.model = model;
        }

        InstallationStepResult ILinuxAction.TryRunStep(ILinux linux)
        {
            try
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

                command = $"sudo ufw allow ntp";
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
