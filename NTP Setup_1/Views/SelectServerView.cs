namespace NTP_Setup_1.Views
{
    using System;
    using System.Net;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class SelectServerView : Dialog
    {
        private const int TextBoxWidth = 190;

        public SelectServerView(Engine engine) : base(engine)
        {
            Title = "Connect to Server";
            Ipaddress = new TextBox();
            Ipaddress.Width = TextBoxWidth;
            User = new TextBox();
            User.Width = TextBoxWidth;
            Password = new PasswordBox(true);
            Password.Width = TextBoxWidth;
            FeedbackConnection = new TextBox();
            FeedbackConnection.Width = TextBoxWidth + 150;
            FeedbackConnection.IsMultiline = true;
            FeedbackConnection.Height = 100;

            NextButton = new Button("Next");
            VerifyConnectionButton = new Button("Save Settings");
        }

        public TextBox Ipaddress { get; set; }

        public TextBox User { get; set; }

        public TextBox FeedbackConnection { get; set; }

        public PasswordBox Password { get; set; }

        public Button VerifyConnectionButton { get; set; }

        public Button NextButton { get; set; }

        public void InitializeScreen()
        {
            int row = 0;
            AddWidget(new Label("IP Address"), row, 0);
            AddWidget(Ipaddress, row, 1, 1, 2);

            row++;
            AddWidget(new Label(string.Empty), row++, 1);

            AddWidget(new Label("User"), row, 0);
            AddWidget(User, row, 1, 1, 2);

            row++;
            AddWidget(new Label("Password"), row, 0);
            AddWidget(Password, row, 1, 1, 2);

            row++;
            AddWidget(new Label(string.Empty), row++, 1);
            AddWidget(new Label(string.Empty), row++, 1);

            row++;
            AddWidget(VerifyConnectionButton, row, 0);
            row++;
            AddWidget(FeedbackConnection, row, 0, 1, 3);

            row++;
            AddWidget(new Label(string.Empty), row++, 1);
            AddWidget(new Label(string.Empty), row++, 1);

            AddWidget(NextButton, row, 2);

            SetColumnWidth(0, 150);
            SetColumnWidth(1, 95);
            SetColumnWidth(2, 110);
            SetColumnWidth(3, 80);
            Width = 600;
            Height = 400;
        }
    }
}
