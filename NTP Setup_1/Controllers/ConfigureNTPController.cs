﻿namespace NTP_Setup_1.Controllers
{
    using System;
    using System.Collections.Generic;
	using System.IO;
	using System.Linq;
    using System.Net.Sockets;
    using NTP_Setup_1.Steps;
    using NTP_Setup_1.Views;
	using Renci.SshNet;
    using Renci.SshNet.Common;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.SoftwareBundle;

    public class ConfigureNTPController
    {
        private readonly ConfigureNTPView configureNTPView;
        private readonly NTPSetupModel model;
		private readonly string folderPath = @"C:\Skyline DataMiner\Documents\DMA_COMMON_DOCUMENTS\InstallPackages";
		private Dictionary<string, IUnZippedSoftwareBundle> packages;

		public ConfigureNTPController(Engine engine, ConfigureNTPView view, NTPSetupModel model)
        {
            configureNTPView = view;
            this.model = model;
            Engine = engine;

            view.ManagePackagesButton.Pressed += OnManagePackagesButtonPressed;
            view.NextButton.Pressed += OnNextButtonPressed;
        }

        public event EventHandler<EventArgs> Next;

        public Engine Engine { get; set; }

        public void Initialize()
        {
            configureNTPView.InitializeView(model);
            
            configureNTPView.NextButton.IsEnabled = false;
        }

        public void EmptyView()
        {
			configureNTPView.Clear();
        }

        private void OnManagePackagesButtonPressed(object sender, EventArgs e)
        {
            try
            {
				// Call subscript
				// Prepare a subscript
				SubScriptOptions subScript = Engine.PrepareSubScript("ManageInstallPackages");

				// Prepare Input param
				string input = $@"
{{
	""name"":""NTPSetup"",
	""version"":""0.0.0"",
	""system"":""{model.Linux.OsInfo.OsType}"",
	""arch"":""{model.Linux.Arch}""
}}
";

				Engine.GenerateInformation($"Starting script with {input}");

				subScript.SelectScriptParam("Filter", input);

				// Launch the script
				subScript.StartScript();

				// Refresh the dropdown list
				UpdateView();
			}
            catch
            {
                throw new Exception();
            }
        }

		private void UpdateView()
		{
			UpdateDropDown();

			if (configureNTPView.PackagesDropDown.Options.Any())
			{
				configureNTPView.NextButton.IsEnabled = true;
			}
			else
			{
				configureNTPView.NextButton.IsEnabled = false;
			}
		}
		private void UpdateDropDown()
		{
			var dropdown = configureNTPView.PackagesDropDown;

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			packages = GetPackages(folderPath, model);

			var dropdownOptions = packages.Keys.ToList();

			dropdown.SetOptions(dropdownOptions);
		}

		internal static Dictionary<string, IUnZippedSoftwareBundle> GetPackages(string folderPath, NTPSetupModel model)
		{
			var packages = new Dictionary<string, IUnZippedSoftwareBundle>();
			string[] packageNameFolders = Directory.GetDirectories(folderPath);
			foreach (string packageFolder in packageNameFolders)
			{
				string[] packageVersionFolders = Directory.GetDirectories(packageFolder);
				foreach (var packageVersionFolder in packageVersionFolders)
				{
					try
					{
						var unzippedPackage = SoftwareBundles.GetUnZippedSoftwareBundle(packageVersionFolder);
						var packageInfo = unzippedPackage.SoftwareBundleInfo;
						if (packageInfo.Name == "NTPSetup")
						{
							packages[packageInfo.Version.ToString()] = unzippedPackage;
						}
					}
					catch
					{
						continue;
					}
				}
			}

			return packages;
		}

		private void OnNextButtonPressed(object sender, EventArgs e)
        {
			if (model.IsOffline)
			{
				model.InstallPackage = packages[configureNTPView.PackagesDropDown.Selected];
			}

			model.AsHost = configureNTPView.AsHostDropDown.Selected == "Client"? false : true;

            Next?.Invoke(this, EventArgs.Empty);
        }

    }
}