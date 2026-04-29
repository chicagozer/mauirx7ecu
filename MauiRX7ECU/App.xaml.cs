namespace MauiRX7;
using System.IO;

public partial class App : Application
{
	public App()
	{
		var licenseKey = LoadSyncfusionLicenseKey();
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		//return new Window(new AppShell());
		return new Window(new MainPage());
	}

	private static string LoadSyncfusionLicenseKey()
	{
		const string envVarName = "SYNCFUSION_LICENSE_KEY";
		var fromEnv = Environment.GetEnvironmentVariable(envVarName);
		if (!string.IsNullOrWhiteSpace(fromEnv))
		{
			return fromEnv;
		}

		var licenseFilePath = Path.Combine(AppContext.BaseDirectory, "syncfusion-license.key");
		if (File.Exists(licenseFilePath))
		{
			var fromFile = File.ReadAllText(licenseFilePath).Trim();
			if (!string.IsNullOrWhiteSpace(fromFile))
			{
				return fromFile;
			}
		}

		throw new InvalidOperationException(
			$"Syncfusion license key not found. Set {envVarName} or place syncfusion-license.key next to the app executable.");
	}
}