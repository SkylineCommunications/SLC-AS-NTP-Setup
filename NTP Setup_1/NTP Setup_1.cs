namespace NTP_Setup_1
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using NTP_Setup_1.Controllers;
    using NTP_Setup_1.Steps;
    using NTP_Setup_1.Views;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents a DataMiner Automation script.
    /// </summary>
    public class Script
	{
        private InteractiveController controller;

        /// <summary>
        /// The script entry point.
        /// </summary>
        /// <param name="engine">Link with SLAutomation process.</param>
        public void Run(Engine engine)
		{
            // engine.ShowUI();
            engine.FindInteractiveClient("Launching NTP Script", 100, "user:" + engine.UserLoginName, AutomationScriptAttachOptions.AttachImmediately);
            controller = new InteractiveController(engine);
            engine.Timeout = new TimeSpan(1, 0, 0);

            string input = engine.GetScriptParam("Input").Value;
            var model = JsonConvert.DeserializeObject<NTPSetupModel>(input);

            try
            {
                if (model.IsSilent)
                {
                    SilentSetup(engine, model);
                }

                SelectServerView selectServerView = new SelectServerView(engine);
                SelectServerController selectServerController = new SelectServerController(engine, selectServerView, model);

                ConfigureNTPView configureNTPView = new ConfigureNTPView(engine);
                ConfigureNTPController configureNTPController = new ConfigureNTPController(engine, configureNTPView, model);

                SetupNTPView setupNTPView = new SetupNTPView(engine);
                SetupNTPController setupNFSController = new SetupNTPController(engine, setupNTPView, model);

                selectServerController.Next += (sender, args) =>
                {
					configureNTPController.Initialize();
					controller.ShowDialog(configureNTPView);
				};

                configureNTPController.Next += (sender, args) =>
                {
                    setupNFSController.InitializeView();
                    controller.ShowDialog(setupNTPView);
                };

                setupNFSController.Next += (sender, args) =>
                {
                    engine.ExitSuccess("Installation Completed.");
                };

                controller.Run(selectServerView);
            }
            catch (ScriptAbortException ex)
            {
                if (ex.Message.Contains("ExitFail"))
                {
                    HandleknownException(engine, ex);
                }
                else
                {
                    // Do nothing as it's an exitsuccess event
                }
            }
            catch (Exception ex)
            {
                HandleUnknownException(engine, ex);
            }
            finally
            {
                engine.AddScriptOutput("NTP_setup", "success");
            }

        }

        private void HandleUnknownException(Engine engine, Exception ex)
        {
            var message = "ERR| An unexpected error occurred, please contact skyline and provide the following information: \n" + ex;
            try
            {
                controller.Run(new ErrorView(engine, ex));
            }
            catch (Exception ex_two)
            {
                engine.GenerateInformation("ERR| Unable to show error message window: " + ex_two);
            }

            engine.GenerateInformation(message);
        }

        private void HandleknownException(Engine engine, Exception ex)
        {
            var message = "ERR| Script has been canceled because of the following error: \n" + ex;
            try
            {
                controller.Run(new ErrorView(engine, ex));
            }
            catch (Exception ex_two)
            {
                engine.GenerateInformation("ERR| Unable to show error message window: " + ex_two);
            }

            engine.GenerateInformation(message);
        }

        private void SilentSetup(Engine engine, NTPSetupModel model)
        {
            if (model.IsOffline)
            {
                engine.ExitFail("No network capability detected, this is required for NTP setup.");
            }

            model.Linux = UtilityFunctions.ConnectToLinuxServer(model.Host, model.Username, model.Password);

            var steps = new List<ILinuxAction>() { };

            if ((bool)model.AsHost)
            {
                steps.Add(new SetupServer(model));
            }
            else
            {
                steps.Add(new SetupClient(model));
            }

            int numberOfSteps = steps.Count();

            int i = 1;
            bool installSucceeded = true;

            foreach (var result in model.Linux.RunActions(steps))
            {
                installSucceeded &= result.Succeeded;
                engine.ShowProgress($"({i}/{numberOfSteps}) {result.Result}");
                engine.GenerateInformation($"{result.Result}");
                i++;
                if (result.Succeeded != true)
                {
                    engine.ExitFail("Setup failed.");
                    break;
                }
            }

            engine.ExitSuccess("Setup Completed.");
        }
    }
}