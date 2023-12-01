namespace NTP_Setup_1
{
    using Skyline.DataMiner.CommunityLibrary.Linux.Communication;
    using Skyline.DataMiner.CommunityLibrary.Linux;
    using System;

    public static class UtilityFunctions
    {
        public static ILinux ConnectToLinuxServer(string host, string username, string password)
        {
            ConnectionSettings settings = new ConnectionSettings(host, username, password);
            ISshConnection connections = SshConnectionFactory.GetSshConnection(settings);
            var linux = LinuxFactory.GetLinux(connections);
            linux.Connection.Connect();

            if (string.IsNullOrWhiteSpace(linux.Connection.RunCommand("whoami")))
            {
                throw new Exception("Connection to server failed, please try again.");
            }

            return linux;
        }
    }
}
