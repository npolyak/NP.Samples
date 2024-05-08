import { dotnet } from './_framework/dotnet.js'

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

const { getConfig, runMain } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const config = getConfig();

// the first argument is the main assembly name
// the second argument is a string array of args to be passed to the
// C# Program.Main(string[] args)
// we pass to it only one argument containing the ASP.NET server URL
// The same argument is set to CommonData.Url static C# property and 
// used the address to create the gRPC channel in C#
await runMain(config.mainAssemblyName, [window.location.origin]);
