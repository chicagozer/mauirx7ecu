namespace MauiRX7;

public partial class App : Application
{
	public App()
	{Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzg4ODY0NEAzMjM5MmUzMDJlMzAzYjMyMzkzYmMyUDJ0MURRNzQxTUg5VUhTZ3g1YjBOSDVPODZvelpEMkc5ZmlSREJ3MjA9");
		
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		//return new Window(new AppShell());
		return new Window(new MainPage());
	}
}