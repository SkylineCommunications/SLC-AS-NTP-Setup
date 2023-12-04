namespace NTP_Setup_1.Views
{
    using System;
    using System.Net;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class ConfigureNTPView : Dialog
    {
        private const int TextBoxWidth = 190;

        public ConfigureNTPView(Engine engine) : base(engine)
        {
            Title = "Configure NTP Installation";

            NextButton = new Button("Next");
            AsHostDropDown = new DropDown(new[] { "Client", "Server" });
            PackagesDropDown = new DropDown();
            ManagePackagesButton = new Button("Manage Packages");
        }

        public DropDown AsHostDropDown { get; set; }

        public DropDown PackagesDropDown { get; set; }

        public Button NextButton { get; set; }

        public Button ManagePackagesButton { get;set; }

        public void InitializeView(NTPSetupModel model)
        {
            int row = 0;
            AddWidget(new Label("Choose Installation Mode:"), row++, 0);
            AddWidget(AsHostDropDown, row++, 0, 1, 3);
            AddWidget(new Label("Select 'Client' to connect to an established NTP server or 'Server' to setup an NTP server."), row++, 0, 1, 3);

            if (model.IsOffline.Value)
            {
				this.AddWidget(new Label("Select NTP package to install:"), row++, 0);
				this.AddWidget(this.PackagesDropDown, row++, 0, 1, 4);
				this.PackagesDropDown.Width = 350;
				this.AddWidget(this.ManagePackagesButton, row, 0, 1, 2);
			}

			AddWidget(NextButton, row++, 2, 1, 1);

			SetColumnWidth(0, 150);
            SetColumnWidth(1, 95);
            SetColumnWidth(2, 110);
            SetColumnWidth(3, 80);
            Width = 600;
            Height = 400;
        }
    }
}
