/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 21/08/2019
 * Time: 13:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using captainalm.integrator;

namespace captainalm.integrator.verifier
{
	/// <summary>
	/// Main Execution Class.
	/// </summary>
	static class Program
	{
		static StringArrayParser argsParser = null;
		static String sourcePath = "";
		static String integrationFile = "";
		static List<String> includedPaths = new List<String>();
		static List<String> excludedPaths = new List<String>();
		static List<FileAttributes> includedAttributes = new List<FileAttributes>();
		static List<FileAttributes> excludedAttributes = new List<FileAttributes>();
		static Boolean prompt = false;
		static Boolean serializeIntegration = false;
		static List<setups> setup = new List<setups>();
		
		public static void Main(string[] args)
		{
			Console.WriteLine("C-ALM Verifier : (C) Captain ALM 2019");
			Console.WriteLine("Version: " + typeof(Program).Assembly.GetName().Version.ToString());
			Console.WriteLine("Powered By C-ALM Integrator Version: " + typeof(Integrator).Assembly.GetName().Version.ToString());
			argsParser = new StringArrayParser(args);
			if (argsParser.count == 0) {
				displayHelp();
				Environment.Exit(0);
			} else {
				if (argsParser.hasSwitchIgnoreCase("p") || argsParser.hasSwitchIgnoreCase("prompt")) {prompt = true;}
				if (argsParser.hasSwitchIgnoreCase("ser") || argsParser.hasSwitchIgnoreCase("serialize")) {serializeIntegration = true;}
				if (argsParser.hasSwitch("?") || argsParser.hasSwitchIgnoreCase("h") || argsParser.hasSwitchIgnoreCase("help") || argsParser.hasSwitchIgnoreCase("u") || argsParser.hasSwitchIgnoreCase("usage")) {
					displayHelp();
					Environment.Exit(0);
				} else {
					try {
						processArgs();
						runtime();
						if (prompt) {
							Console.Write("Press any key to exit . . . ");
							Console.ReadKey(true);
							Console.WriteLine("");
						}
						Environment.Exit(0);
					} catch (System.Exception ex) {
						Console.WriteLine("Exception: " + ex.GetType().ToString() + " has occured.");
						Console.WriteLine("Message: " + ex.Message);
						Console.WriteLine("Stack Trace: ");
						Console.WriteLine(ex.StackTrace);
						if (prompt) {
							Console.Write("Press any key to exit . . . ");
							Console.ReadKey(true);
							Console.WriteLine("");
						}
						//Environment.Exit(ex.HResult); I would like to use this but its friend access *-*
						Environment.Exit(1);
					}
				}
			}
		}
		
		static void processArgs() {
			if (argsParser.hasSwitchIgnoreCase("set")) {
				var switches = argsParser.get_argDataIgnoreCase("set");
				if (! object.ReferenceEquals(null, switches[0])) {
					setup = parseSetups(switches[0]);
				} else {
					throw new ArgumentException("set");
				}
			} else if (argsParser.hasSwitchIgnoreCase("setup")) {
				var switches = argsParser.get_argDataIgnoreCase("setup");
				if (! object.ReferenceEquals(null, switches[0])) {
					setup = parseSetups(switches[0]);
				} else {
					throw new ArgumentException("setup");
				}
			}
			if (argsParser.hasSwitchIgnoreCase("i")) {
				var switches = argsParser.get_argDataIgnoreCase("i");
				if (! object.ReferenceEquals(null, switches[0])) {
					integrationFile = switches[0];
				} else {
					throw new ArgumentException("i");
				}
			} else if (argsParser.hasSwitchIgnoreCase("int")) {
				var switches = argsParser.get_argDataIgnoreCase("int");
				if (! object.ReferenceEquals(null, switches[0])) {
					integrationFile = switches[0];
				} else {
					throw new ArgumentException("int");
				}
			} else if (argsParser.hasSwitchIgnoreCase("integration")) {
				var switches = argsParser.get_argDataIgnoreCase("integration");
				if (! object.ReferenceEquals(null, switches[0])) {
					integrationFile = switches[0];
				} else {
					throw new ArgumentException("integration");
				}
			} else if (argsParser.hasSwitchIgnoreCase("f")) {
				var switches = argsParser.get_argDataIgnoreCase("f");
				if (! object.ReferenceEquals(null, switches[0])) {
					integrationFile = switches[0];
				} else {
					throw new ArgumentException("f");
				}
			} else if (argsParser.hasSwitchIgnoreCase("file")) {
				var switches = argsParser.get_argDataIgnoreCase("file");
				if (! object.ReferenceEquals(null, switches[0])) {
					integrationFile = switches[0];
				} else {
					throw new ArgumentException("file");
				}
			}
			if (argsParser.hasSwitchIgnoreCase("s")) {
				var switches = argsParser.get_argDataIgnoreCase("s");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath = switches[0];
				} else {
					throw new ArgumentException("s");
				}
			} else if (argsParser.hasSwitchIgnoreCase("source")) {
				var switches = argsParser.get_argDataIgnoreCase("source");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath = switches[0];
				} else {
					throw new ArgumentException("source");
				}
			} else if (argsParser.hasSwitchIgnoreCase("sd")) {
				var switches = argsParser.get_argDataIgnoreCase("sd");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath = switches[0];
				} else {
					throw new ArgumentException("sd");
				}
			} else if (argsParser.hasSwitchIgnoreCase("sourcedirectory")) {
				var switches = argsParser.get_argDataIgnoreCase("sourcedirectory");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath = switches[0];
				} else {
					throw new ArgumentException("sourcedirectoryss");
				}
			}
			if (argsParser.hasSwitchIgnoreCase("ip")) {
				var switches = argsParser.get_argDataIgnoreCase("ip");
				for (int i = 0; i < switches.Length - 1; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						includedPaths.Add(switches[i]);
					} else {
						throw new ArgumentException("ip " + i);
					}
				}
			} else if (argsParser.hasSwitchIgnoreCase("includepath")) {
				var switches = argsParser.get_argDataIgnoreCase("includepath");
				for (int i = 0; i < switches.Length - 1; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						includedPaths.Add(switches[i]);
					} else {
						throw new ArgumentException("includepath " + i);
					}
				}
			}
			if (argsParser.hasSwitchIgnoreCase("ep")) {
				var switches = argsParser.get_argDataIgnoreCase("ep");
				for (int i = 0; i < switches.Length - 1; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						excludedPaths.Add(switches[i]);
					} else {
						throw new ArgumentException("ep " + i);
					}
				}
			} else if (argsParser.hasSwitchIgnoreCase("excludepath")) {
				var switches = argsParser.get_argDataIgnoreCase("excludepath");
				for (int i = 0; i < switches.Length - 1; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						excludedPaths.Add(switches[i]);
					} else {
						throw new ArgumentException("excludepath " + i);
					}
				}
			}
			if (argsParser.hasSwitchIgnoreCase("ia")) {
				var switches = argsParser.get_argDataIgnoreCase("ia");
				for (int i = 0; i < switches.Length - 1; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						includedAttributes.Add(parseAttributes(switches[i]));
					} else {
						throw new ArgumentException("ia " + i);
					}
				}
			} else if (argsParser.hasSwitchIgnoreCase("includeattributes")) {
				var switches = argsParser.get_argDataIgnoreCase("includeattributes");
				for (int i = 0; i < switches.Length - 1; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						includedAttributes.Add(parseAttributes(switches[i]));
					} else {
						throw new ArgumentException("includeattributes " + i);
					}
				}
			}
			if (argsParser.hasSwitchIgnoreCase("ea")) {
				var switches = argsParser.get_argDataIgnoreCase("ea");
				for (int i = 0; i < switches.Length - 1; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						excludedAttributes.Add(parseAttributes(switches[i]));
					} else {
						throw new ArgumentException("ea " + i);
					}
				}
			} else if (argsParser.hasSwitchIgnoreCase("excludeattributes")) {
				var switches = argsParser.get_argDataIgnoreCase("excludeattributes");
				for (int i = 0; i < switches.Length - 1; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						excludedAttributes.Add(parseAttributes(switches[i]));
					} else {
						throw new ArgumentException("excludeattributes " + i);
					}
				}
			}
			
		}
		
		static void runtime() {
			
		}
		
		static void displayHelp() {
			Console.WriteLine("Usage:");
			Console.WriteLine("C-ALM-Verifier.exe {[switch/=] [/arg] ...(Repeat)...}");
			Console.WriteLine("/ denotes optional part.");
			Console.WriteLine("Switches ; Argument Reference Link:");
			Console.WriteLine("?, u, usage, h or help : shows this message.");
			Console.WriteLine("set or setup : specifies the setup; %SETUPCODE%...(Repeat)...");
			Console.WriteLine("i, int, integration, f, file : specifies the integration file; %PATH%");
			Console.WriteLine("s, source, sd or sourcedirectory : specifies the source directory; %PATH%");
			Console.WriteLine("p or prompt : enables prompting.");
			Console.WriteLine("ser or serialize : enables integration serialization.");
			Console.WriteLine("ip or includepath : specifies a path to include; %PATH%");
			Console.WriteLine("ep or excludepath : specifies a path to exclude; %PATH%");
			Console.WriteLine("ia or includeattributes : specifies an attribute set to include; %ATTRIBUTE%...(Repeat)...");
			Console.WriteLine("ea or excludeattributes : specifies am attribute set to exclude; %ATTRIBUTE%...(Repeat)...");
			Console.WriteLine("Argument Reference:");
			Console.WriteLine("%SETUPCODE%:");
			Console.WriteLine("C : Create.");
			Console.WriteLine("U : Update.");
			Console.WriteLine("V : Verify.");
			Console.WriteLine("%PATH%:");
			Console.WriteLine("* : specifies a path to a folder or file.");
			Console.WriteLine("%ATTRIBUTE%:");
			Console.WriteLine("1 : Archive.");
			Console.WriteLine("2 : Compressed.");
			Console.WriteLine("3 : Device.");
			Console.WriteLine("4 : Directory.");
			Console.WriteLine("5 : Encrypted.");
			Console.WriteLine("6 : Hidden.");
			Console.WriteLine("7 : Normal.");
			Console.WriteLine("8 : Not Content Indexed.");
			Console.WriteLine("9 : Offline.");
			Console.WriteLine("0 : Read Only.");
			Console.WriteLine("a : Reparse Point.");
			Console.WriteLine("b : Sparse File.");
			Console.WriteLine("c : System.");
			Console.WriteLine("d : Temporary.");
			if (prompt) {
				Console.Write("Press any key to exit . . . ");
				Console.ReadKey(true);
				Console.WriteLine("");
			}
		}
		
		static List<setups> parseSetups(String theArg) {
			var toret = new List<setups>();
			for (int i = 0; i < theArg.Length - 1; i++) {
				var c = theArg.ToLower()[i];
				if (c == 'c') {
					toret.Add(setups.Create);
				} else if (c == 'u') {
					toret.Add(setups.Update);
				} else if (c == 'v') {
					toret.Add(setups.Verify);
				}
			}
			return toret;
		}
		
		static FileAttributes parseAttributes(String theArg) {
			var toret = (FileAttributes) 0;
			for (int i = 0; i < theArg.Length - 1; i++) {
				var c = theArg.ToLower()[i];
				addAnotherAttribute(toret, c);
			}
			return toret;
		}
		
		static FileAttributes addAnotherAttribute(FileAttributes cfa, Char code) {
			if (code == '1') {
				return cfa | FileAttributes.Archive;
			} else if (code == '2') {
				return cfa | FileAttributes.Compressed;
			} else if (code == '3') {
				return cfa | FileAttributes.Device;
			} else if (code == '4') {
				return cfa | FileAttributes.Directory;
			} else if (code == '5') {
				return cfa | FileAttributes.Encrypted;
			} else if (code == '6') {
				return cfa | FileAttributes.Hidden;
			} else if (code == '7') {
				return cfa | FileAttributes.Normal;
			} else if (code == '8') {
				return cfa | FileAttributes.NotContentIndexed;
			} else if (code == '9') {
				return cfa | FileAttributes.Offline;
			} else if (code == '0') {
				return cfa | FileAttributes.ReadOnly;
			} else if (code == 'a') {
				return cfa | FileAttributes.ReparsePoint;
			} else if (code == 'b') {
				return cfa | FileAttributes.SparseFile;
			} else if (code == 'c') {
				return cfa | FileAttributes.System;
			} else if (code == 'd') {
				return cfa | FileAttributes.Temporary;
			}
			return cfa;
		}
	}
	
	enum setups {
		Create = 1,
		Verify = 2,
		Update = 3
	}
}