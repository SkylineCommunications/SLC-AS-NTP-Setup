namespace NTP_Setup_1.Controllers
{
	using System;
	using System.Linq;

	using NTP_Setup_1.Views;

	using Skyline.DataMiner.Automation;

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
			switch (model.AsServer.Value)
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
			if (string.IsNullOrEmpty(setupNTPView.Server.Text))
			{
				setupNTPView.Feedback.Text = "Server field is required.";
				return;
			}

			try
			{
				setupNTPView.Feedback.Text = string.Empty;
				model.Server = setupNTPView.Server.Text;

				var steps = UtilityFunctions.GetInstallationSteps(Engine, model);

				int numberOfSteps = steps.Count();

				int i = 1;
				bool installSucceeded = true;

				setupNTPView.StartInstalling();

				foreach (var step in steps)
				{
					setupNTPView.AddInstallationStep(step.Description);
					var result = step.TryRunStep(model.Linux);
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
			catch (Exception exception)
			{
				throw new Exception(exception.Message);
			}
		}

		private void OnSetupNTPServerButtonPressed(object sender, EventArgs e)
		{
			try
			{
				setupNTPView.Feedback.Text = string.Empty;

				var steps = UtilityFunctions.GetInstallationSteps(Engine, model);

				int numberOfSteps = steps.Count();

				int i = 1;
				bool installSucceeded = true;

				setupNTPView.StartInstalling();

				foreach (var step in steps)
				{
					setupNTPView.AddInstallationStep(step.Description);
					var result = step.TryRunStep(model.Linux);
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