/*
****************************************************************************
*  Copyright (c) 2024,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

06/08/2024	1.0.0.1		AMA, Skyline	Initial version
****************************************************************************
*/
using System;
using System.IO;

using Install_1;
using Install_1.DOM;
using Install_1.Images;
using Install_1.LCA;
using Install_1.Themes;

using Newtonsoft.Json;

using Skyline.AppInstaller;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net.AppPackages;

/// <summary>
/// DataMiner Script Class.
/// </summary>
internal class Script
{
	/// <summary>
	/// The script entry point.
	/// </summary>
	/// <param name="engine">Provides access to the Automation engine.</param>
	/// <param name="context">Provides access to the installation context.</param>
	[AutomationEntryPoint(AutomationEntryPointType.Types.InstallAppPackage)]
	public void Install(IEngine engine, AppInstallContext context)
	{
		try
		{
			engine.Timeout = new TimeSpan(0, 10, 0);
			engine.GenerateInformation("Starting installation");
			var installer = new AppInstaller(Engine.SLNetRaw, context);
			installer.InstallDefaultContent();

			// Custom installation logic can be added here for each individual install package.
			var installOptions = JsonConvert.DeserializeObject<InstallOptions>(File.ReadAllText(context.AppContentPath + Constants.InstallOptionsFileName));
			var domInstaller = new DomInstaller(Engine.SLNetRaw, context, installer.Log);
			domInstaller.InstallDefaultContent();

			// Install app themes
			var themeInstaller = new ThemeInstaller(Engine.SLNetRaw, context, installOptions, installer.Log);
			themeInstaller.InstallDefaultContent();

			// Create image installer
			var imageInstaller = new ImageInstaller(Engine.SLNetRaw, context, installOptions, installer.Log);
			imageInstaller.InstallDefaultContent();

			// Install the low code app
			var lcaInstaller = new LcaInstaller(Engine.SLNetRaw, context, installOptions, installer.Log);
			lcaInstaller.InstallDefaultContent();
		}
		catch (Exception e)
		{
			engine.ExitFail("Exception encountered during installation: " + e);
		}
	}
}