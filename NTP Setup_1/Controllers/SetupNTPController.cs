namespace NTP_Setup_1.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using NTP_Setup_1.Steps;
    using NTP_Setup_1.Views;
    using Renci.SshNet;
    using Renci.SshNet.Common;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.CommunityLibrary.Linux;
    using Skyline.DataMiner.CommunityLibrary.Linux.Actions.ActionSteps;
    using Skyline.DataMiner.CommunityLibrary.Linux.Communication;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class SetupNTPController
    {
        private readonly SetupNTPView setupNTPView;
        private readonly NTPSetupModel model;

        public SetupNTPController(Engine engine, SetupNTPView view, NTPSetupModel model)
        {
            setupNTPView = view;
            this.model = model;
            Engine = engine;

            view.SetupNTPClientButton.Pressed += OnSetupNTPClientButtonPressed;
            view.SetupNTPServerButton.Pressed += OnSetupNTPServerButtonPressed;
            view.NextButton.Pressed += OnNextButtonPressed;
        }

        public event EventHandler<EventArgs> Next;

        public Engine Engine { get; set; }

        public void InitializeView()
        {
            switch (model.AsHost)
            {
                default:
                case true:
                    setupNTPView.InitializeServerSetup();
                    break;
                case false:
                    setupNTPView.InitializeClientSetup();
                    break;
            }
            
            setupNTPView.NextButton.IsEnabled = false;
        }

        public void EmptyView()
        {
            setupNTPView.Clear();
        }

        private void OnSetupNTPClientButtonPressed(object sender, EventArgs e)
        {
            try
            {
                setupNTPView.Feedback.Text = string.Empty;

                var steps = new List<ILinuxAction>();
                model.Server = setupNTPView.Server.Text;

                steps.Add(new SetupClient(model));

                int numberOfSteps = steps.Count();

                int i = 1;
                bool installSucceeded = true;

                foreach (var result in model.Linux.RunActions(steps))
                {
                    setupNTPView.StartInstalling();
                    installSucceeded &= result.Succeeded;
                    setupNTPView.AddInstallationFeedback($"({i}/{numberOfSteps}) {result.Result}");
                    i++;
                    if (result.Succeeded != true)
                    {
                        break;
                    }
                }

                setupNTPView.SetInstallationResult(installSucceeded);
            }
            catch
            {
                throw new Exception();
            }
        }

        private void OnSetupNTPServerButtonPressed(object sender, EventArgs e)
        {
            try
            {
                setupNTPView.Feedback.Text = string.Empty;

                var steps = new List<ILinuxAction>();

                steps.Add(new SetupServer(model));

                int numberOfSteps = steps.Count();

                int i = 1;
                bool installSucceeded = true;

                foreach (var result in model.Linux.RunActions(steps))
                {
                    setupNTPView.StartInstalling();
                    installSucceeded &= result.Succeeded;
                    setupNTPView.AddInstallationFeedback($"({i}/{numberOfSteps}) {result.Result}");
                    i++;
                    if (result.Succeeded != true)
                    {
                        break;
                    }
                }

                setupNTPView.SetInstallationResult(installSucceeded);
            }
            catch
            {
                throw new Exception();
            }
        }

        private void OnNextButtonPressed(object sender, EventArgs e)
        {
            Next?.Invoke(this, EventArgs.Empty);
        }
    }
}
