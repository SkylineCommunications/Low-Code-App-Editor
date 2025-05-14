/*
****************************************************************************
*  Copyright (c) 2025,  Skyline Communications NV  All Rights Reserved.    *
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

10/04/2025	1.0.0.1		ArneMA, Skyline	Initial version
****************************************************************************
*/

using System;
using System.Diagnostics;
using System.IO;

using Low_Code_App_Editor_Package;

using Skyline.AppInstaller;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net.AppPackages;

/// <summary>
/// DataMiner Script Class.
/// </summary>
internal class Script
{
	private const string WebApiLib_ProtocolScripts_Path = @"C:\Skyline DataMiner\ProtocolScripts\WebApiLib.dll";
	private const string WebApiLib_ProtocolScripts_DllImport_Path = @"C:\Skyline DataMiner\ProtocolScripts\DllImport\WebApiLib.dll";
	private const string WebApiLib_WebPages_Path = @"C:\Skyline DataMiner\Webpages\API\bin\WebApiLib.dll";

	/// <summary>
	///     The script entry point.
	/// </summary>
	/// <param name="engine">Provides access to the Automation engine.</param>
	/// <param name="context">Provides access to the installation context.</param>
	[AutomationEntryPoint(AutomationEntryPointType.Types.InstallAppPackage)]
	public void Install(Engine engine, AppInstallContext context)
	{
		try
		{
			engine.Timeout = new TimeSpan(0, 10, 0);
			engine.GenerateInformation("Starting installation");
			var installer = new AppInstaller(Engine.SLNetRaw, context);
			installer.InstallDefaultContent();

			// Custom installation logic can be added here for each individual install package.

			// Create a symbolic link to the WebApiLib.dll
			Action<string> logger = installer.Log;
			Engine.SLNetRaw.CreateSymbolicLink(WebApiLib_ProtocolScripts_Path, WebApiLib_WebPages_Path, logger);
			Engine.SLNetRaw.CreateSymbolicLink(WebApiLib_ProtocolScripts_DllImport_Path, WebApiLib_WebPages_Path, logger);
		}
		catch (Exception e)
		{
			engine.ExitFail("Exception encountered during installation: " + e);
		}
	}
}