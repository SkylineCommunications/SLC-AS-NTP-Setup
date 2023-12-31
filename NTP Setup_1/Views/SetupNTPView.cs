﻿namespace NTP_Setup_1.Views
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	public class SetupNTPView : Dialog
	{
		private const int TextBoxWidth = 320;
		private const int TextBoxHeight = 150;
		private List<string> InstallationFeedback = new string[] { }.ToList();

		public SetupNTPView(Engine engine) : base(engine)
		{
			Title = "Setup NTP Host";
			Server = new TextBox();
			Server.Width = TextBoxWidth;

			Feedback = GetTextBox("Fill in the NTP server in the field above and press the setup button.");

			NextButton = new Button("Next");
			SetupNTPClientButton = new Button("Setup NTP");
			SetupNTPServerButton = new Button("Setup NTP");
		}

		public TextBox Server { get; set; }

		public TextBox Feedback { get; set; }

		public Button SetupNTPClientButton { get; set; }

		public Button SetupNTPServerButton { get; set; }

		public Button NextButton { get; set; }

		public void InitializeClientSetup()
		{
			int row = 0;
			AddWidget(new Label("NTP Server IP Address:"), row++, 0);
			AddWidget(Server, row++, 0, 1, 3);

			AddWidget(Feedback, row++, 0, 1, 3);

			AddWidget(SetupNTPClientButton, row++, 2, 1, 1);
			AddWidget(NextButton, row++, 2, 1, 1);

			SetColumnWidth(0, 150);
			SetColumnWidth(1, 95);
			SetColumnWidth(2, 110);
			SetColumnWidth(3, 80);
			Width = 600;
			Height = 400;
		}

		public void InitializeServerSetup()
		{
			int row = 0;
			AddWidget(Feedback, row++, 0, 1, 3);
			this.Feedback.Text = "Press the setup button.";

			AddWidget(SetupNTPServerButton, row++, 2, 1, 1);
			AddWidget(NextButton, row++, 2, 1, 1);

			SetColumnWidth(0, 150);
			SetColumnWidth(1, 95);
			SetColumnWidth(2, 110);
			SetColumnWidth(3, 80);
			Width = 600;
			Height = 400;
		}

		public void StartInstalling()
		{
			NextButton.IsEnabled = false;
			SetupNTPClientButton.IsEnabled = false;
			SetupNTPServerButton.IsEnabled = false;
		}

		public void AddInstallationFeedback(string feedback)
		{
			InstallationFeedback.RemoveAt(InstallationFeedback.Count - 1);
			InstallationFeedback.Add(feedback);
			Feedback.Text = string.Join(Environment.NewLine, InstallationFeedback);
			this.Show(false);
		}

		public void AddInstallationStep(string description)
		{
			InstallationFeedback.Add(description);
			Feedback.Text = string.Join(Environment.NewLine, InstallationFeedback);
			this.Show(false);
		}

		public void SetInstallationResult(bool succeeded)
		{
			if (succeeded)
			{
				AddInstallationStep("Setup succeeded.");
				SetupNTPClientButton.IsEnabled = false;
				SetupNTPServerButton.IsEnabled = false;
				NextButton.IsEnabled = true;
			}
			else
			{
				Feedback.Text += "Setup failed.";
			}
		}

		private static TextBox GetTextBox(string text, int width = TextBoxWidth, int height = TextBoxHeight, bool multiline = true)
		{
			var textbox = new TextBox(text);
			textbox.IsMultiline = multiline;
			textbox.Width = width;
			textbox.Height = height;
			return textbox;
		}
	}
}