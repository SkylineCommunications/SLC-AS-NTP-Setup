namespace NTP_Setup_1
{
	using System.Collections.Generic;

	using Skyline.DataMiner.Utils.Linux;

	public static class InstallNTP
	{
		public static IEnumerable<InstallationStepResult> TryInstallNTP(this ILinux linux, IEnumerable<IInstallerAction> steps)
		{
			foreach (var step in steps)
			{
				var result = step.TryRunStep(linux);
				yield return result;
				if (!result.Succeeded)
				{
					// Don't continue if one step failed.
					break;
				}
			}
		}
	}
}