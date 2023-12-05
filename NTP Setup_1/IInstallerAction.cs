namespace NTP_Setup_1
{
	using Skyline.DataMiner.Utils.Linux;

	public interface IInstallerAction
	{
		string Description { get; }
		InstallationStepResult TryRunStep(ILinux linux);
	}
}