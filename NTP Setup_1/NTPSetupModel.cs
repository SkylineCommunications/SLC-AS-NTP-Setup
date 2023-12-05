namespace NTP_Setup_1
{
	using Skyline.DataMiner.Utils.Linux;
	using Skyline.DataMiner.Utils.SoftwareBundle;

	public class NTPSetupModel
	{
		/// <summary>
		/// Gets or sets the IP address of the Linux server.
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// Gets or sets the username to access the Linux server.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the password to access the Linux server.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the Linux server.
		/// </summary>
		public ILinux Linux { get; set; }

		/// <summary>
		/// Gets or sets the value that determines if the server should be setup as an NTP Host or Client.
		/// </summary>
		public bool? AsServer { get; set; }

		/// <summary>
		/// Gets or sets the ip address of the server to sync with.
		/// </summary>
		public string Server { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the server has network capability.
		/// </summary>
		public bool? IsOnline { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to carry out silent setup.
		/// </summary>
		public bool? IsSilent { get; set; }

		/// <summary>
		/// Gets or sets the folder path of the install package.
		/// </summary>
		public string PackageFolderPath { get; set; }

		/// <summary>
		/// Gets or sets the UnZippedSoftwareBundle object of the install package.
		/// </summary>
		public IUnZippedSoftwareBundle InstallPackage { get; set; }
	}
}