// note that it expects to load dotnet.js 
// (and wasm files) from _framework folder
import { dotnet } from './_framework/dotnet.js'

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

// get the dotnetRuntime containing all the methods
// and objects for invoking C# from JavaScript and
// vice versa.
const dotnetRuntime = await dotnet.create();

// config contains the web-site configurations
const config = dotnetRuntime.getConfig();

// call Program.Main(string[] args) method from JavaScript
// passing to it an array of arguments "arg1", "arg2" and "arg3"
await dotnetRuntime.runMain(config.mainAssemblyName, ["arg1", "arg2", "arg3"]);

