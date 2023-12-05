namespace NTP_Setup_1.Controllers
{
	using System;
	using System.Net.Sockets;

	using NTP_Setup_1.Views;

	using Renci.SshNet.Common;

	using Skyline.DataMiner.Automation;

	public class SelectServerController
	{
		private readonly SelectServerView selectServerView;
		private readonly NTPSetupModel model;

		public SelectServerController(Engine engine, SelectServerView view, NTPSetupModel model)
		{
			selectServerView = view;
			this.model = model;
			Engine = engine;

			InitializeView();

			view.VerifyConnectionButton.Pressed += OnVerifyConnectionPressed;
			view.NextButton.Pressed += OnNextButtonPressed;
		}

		public event EventHandler<EventArgs> Next;

		public Engine Engine { get; set; }

		public void InitializeView()
		{
			selectServerView.InitializeScreen();
			selectServerView.NextButton.IsEnabled = false;

			PrefillServer(selectServerView, model);
		}

		public void EmptyView()
		{
			selectServerView.Clear();
		}

		private void OnVerifyConnectionPressed(object sender, EventArgs e)
		{
			try
			{
				var linux = UtilityFunctions.ConnectToLinuxServer(selectServerView.Ipaddress.Text, selectServerView.User.Text, selectServerView.Password.Password);

				string whoami = linux.Connection.RunCommand("whoami");

				model.Host = selectServerView.Ipaddress.Text;
				model.Username = selectServerView.User.Text;
				model.Password = selectServerView.Password.Password;

				// Connection unsuccessful
				if (string.IsNullOrWhiteSpace(whoami))
				{
					selectServerView.FeedbackConnection.Text = "Server connection was unsuccessful.";
					return;
				}

				// Connection successful
				selectServerView.FeedbackConnection.Text = @"Connection successful." + Environment.NewLine + "Changing above settings will only take affect after saving again!";
				model.Linux = linux;

				// Network Check
				try
				{
					var networkAvailable = UtilityFunctions.NetworkCheck(model);
					if (networkAvailable || model.IsOnline.Value)
					{
						model.IsOnline = true;
						selectServerView.FeedbackConnection.Text += Environment.NewLine + "Network available, proceeding with setup.";
					}
					else
					{
						model.IsOnline = false;
						selectServerView.FeedbackConnection.Text += Environment.NewLine + "Network unavailable, proceeding with offline setup.";
					}

					selectServerView.NextButton.IsEnabled = true;
				}
				catch (Exception ex)
				{
					selectServerView.FeedbackConnection.Text = ex.ToString();
				}
			}
			catch (SshAuthenticationException)
			{
				selectServerView.FeedbackConnection.Text = "Invalid credentials.";
			}
			catch (SocketException)
			{
				selectServerView.FeedbackConnection.Text = "Host not reachable.";
			}
			catch (Exception ex)
			{
				selectServerView.FeedbackConnection.Text = ex.ToString();
			}
		}

		private void OnNextButtonPressed(object sender, EventArgs e)
		{
			Next?.Invoke(this, EventArgs.Empty);
		}

		private void PrefillServer(SelectServerView view, NTPSetupModel model)
		{
			view.Ipaddress.Text = model.Host;
			view.User.Text = model.Username;
			view.Password.Password = model.Password;
		}
	}
}