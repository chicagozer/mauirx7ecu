namespace MauiRX7;

public partial class App : Application
{
	public App()
	{
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzg5MTgyNUAzMjM5MmUzMDJlMzAzYjMyMzkzYldBcnlaOEdIWjFISkVNWTRQVVI0ZHhnK1FtMk1UdDVwazJZZnJsa0J6SDg9");

		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		//return new Window(new AppShell());
		return new Window(new MainPage());
	}
}