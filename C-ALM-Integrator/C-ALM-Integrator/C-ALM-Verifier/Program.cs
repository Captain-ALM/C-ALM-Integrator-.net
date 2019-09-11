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
		public static StringArrayParser argsParser = null;
        public static String sourcePath = "";
        public static String integrationFile = "";
        public static List<String> includedPaths = new List<String>();
        public static List<String> excludedPaths = new List<String>();
        public static List<FileAttributes> includedAttributes = new List<FileAttributes>();
        public static List<FileAttributes> excludedAttributes = new List<FileAttributes>();
        public static Boolean prompt = false;
        public static Boolean serializeIntegration = false;
        public static List<setups> setup = new List<setups>();
        public static Integrator side1 = null;
        public static Integrator side2 = null;
        public static Boolean info = false;
        public static Type[] integratorTypes = new Type[] { typeof(FSOTypeElement), typeof(StringElement), typeof(DateTimeElement), typeof(StringElement), typeof(LongElement) };
		
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
				prompt |= argsParser.hasSwitchIgnoreCase("p") || argsParser.hasSwitchIgnoreCase("prompt");
				serializeIntegration |= argsParser.hasSwitchIgnoreCase("ser") || argsParser.hasSwitchIgnoreCase("serialize");
                info |= argsParser.hasSwitchIgnoreCase("info") || argsParser.hasSwitchIgnoreCase("information");
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
					sourcePath = Path.GetFullPath(switches[0]);
				} else {
					throw new ArgumentException("s");
				}
			} else if (argsParser.hasSwitchIgnoreCase("source")) {
				var switches = argsParser.get_argDataIgnoreCase("source");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath = Path.GetFullPath(switches[0]);
				} else {
					throw new ArgumentException("source");
				}
			} else if (argsParser.hasSwitchIgnoreCase("sd")) {
				var switches = argsParser.get_argDataIgnoreCase("sd");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath = Path.GetFullPath(switches[0]);
				} else {
					throw new ArgumentException("sd");
				}
			} else if (argsParser.hasSwitchIgnoreCase("sourcedirectory")) {
				var switches = argsParser.get_argDataIgnoreCase("sourcedirectory");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath = Path.GetFullPath(switches[0]);
				} else {
					throw new ArgumentException("sourcedirectory");
				}
			}
			if (argsParser.hasSwitchIgnoreCase("ip")) {
				var switches = argsParser.get_argDataIgnoreCase("ip");
				for (int i = 0; i < switches.Length; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						includedPaths.Add(switches[i]);
					} else {
						throw new ArgumentException("ip " + i);
					}
				}
			} else if (argsParser.hasSwitchIgnoreCase("includepath")) {
				var switches = argsParser.get_argDataIgnoreCase("includepath");
				for (int i = 0; i < switches.Length; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						includedPaths.Add(switches[i]);
					} else {
						throw new ArgumentException("includepath " + i);
					}
				}
			}
			if (argsParser.hasSwitchIgnoreCase("ep")) {
				var switches = argsParser.get_argDataIgnoreCase("ep");
				for (int i = 0; i < switches.Length; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						excludedPaths.Add(switches[i]);
					} else {
						throw new ArgumentException("ep " + i);
					}
				}
			} else if (argsParser.hasSwitchIgnoreCase("excludepath")) {
				var switches = argsParser.get_argDataIgnoreCase("excludepath");
				for (int i = 0; i < switches.Length; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						excludedPaths.Add(switches[i]);
					} else {
						throw new ArgumentException("excludepath " + i);
					}
				}
			}
			if (argsParser.hasSwitchIgnoreCase("ia")) {
				var switches = argsParser.get_argDataIgnoreCase("ia");
				for (int i = 0; i < switches.Length; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						includedAttributes.Add(parseAttributes(switches[i]));
					} else {
						throw new ArgumentException("ia " + i);
					}
				}
			} else if (argsParser.hasSwitchIgnoreCase("includeattributes")) {
				var switches = argsParser.get_argDataIgnoreCase("includeattributes");
				for (int i = 0; i < switches.Length; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						includedAttributes.Add(parseAttributes(switches[i]));
					} else {
						throw new ArgumentException("includeattributes " + i);
					}
				}
			}
			if (argsParser.hasSwitchIgnoreCase("ea")) {
				var switches = argsParser.get_argDataIgnoreCase("ea");
				for (int i = 0; i < switches.Length; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						excludedAttributes.Add(parseAttributes(switches[i]));
					} else {
						throw new ArgumentException("ea " + i);
					}
				}
			} else if (argsParser.hasSwitchIgnoreCase("excludeattributes")) {
				var switches = argsParser.get_argDataIgnoreCase("excludeattributes");
				for (int i = 0; i < switches.Length; i++) {
					if (! object.ReferenceEquals(null, switches[i])) {
						excludedAttributes.Add(parseAttributes(switches[i]));
					} else {
						throw new ArgumentException("excludeattributes " + i);
					}
				}
			}
		}
		
		static void runtime() {
			//Directory.SetCurrentDirectory(sourcePath); // : FIX needed for relative mode
			for (int i = 0; i < setup.Count; i++) {
				var c = setup[i];
				switch (c) {
					case setups.Create:
						create();
						break;
					case setups.Load:
						load();
						break;
					case setups.Verify:
						verify();
						break;
					case setups.Update:
						update();
						break;
				}
			}
		}
		
		static void create() {
			Console.WriteLine("Creating...");
            var baseDir = new FSODirectory(sourcePath, FSOType.RootDirectory) { Info = info };
			var subObjs = baseDir.trawlRecursively(new NominalRTM(prompt, sourcePath), false);
			baseDir.update();
			var objs = new List<FSOBase>();
			objs.Add(baseDir);
			objs.AddRange(subObjs);
			side1 = new Integrator(integratorTypes, 1, objs.Count);
			for (int i = 0; i < objs.Count; i++) {
				side1.set_block(0,i,objs[i].createElements());
			}
			if (serializeIntegration) {
				var sav = new BinaryLoaderSaver();
				side1.save(sav);
				File.WriteAllBytes(integrationFile, sav.data);
			} else {
				var sav = new XMLLoaderSaver();
				side1.save(sav);
				File.WriteAllText(integrationFile, sav.data);
			}
		}
		
		static void load() {
			Console.WriteLine("Loading...");
			if (serializeIntegration) {
				var dat = File.ReadAllBytes(integrationFile);
				var lod = new BinaryLoaderSaver(dat);
				side1 = new Integrator(integratorTypes, 0, 0);
				side1.load(lod);
			} else {
				var dat = File.ReadAllText(integrationFile);
				var lod = new XMLLoaderSaver(dat);
				side1 = new Integrator(integratorTypes, 0, 0);
				side1.load(lod);
			}
			
		}
		
		static void verify() {
			Console.WriteLine("Processing...");
			var indxs = side1.findElements(new FSOTypeElement(FSOType.RootDirectory));
			var indxs2 = side1.findElements(new FSOTypeElement(FSOType.RootFile));
			var bsObjs = new List<FSODirectory>();
			var bsObjs2 = new List<FSOFile>();
			for (int i = 0; i < indxs.Length; i++) {
				var cindxs = indxs[i];
				var fpth = Path.GetFullPath(new FSODirectory(side1.get_block(cindxs[0], cindxs[1])).Path);
                bsObjs.Add(new FSODirectory(fpth ,FSOType.RootDirectory) { Info = info });
			}
			for (int i = 0; i < indxs2.Length; i++) {
				var cindxs = indxs2[i];
				var fpth = Path.GetFullPath(new FSODirectory(side1.get_block(cindxs[0], cindxs[1])).Path);
                bsObjs2.Add(new FSOFile(fpth, FSOType.RootFile) { Info = info });
			}
			var objs2Add = new List<FSOBase>();
			for (int i = 0; i < bsObjs.Count; i++) {
				objs2Add.AddRange(bsObjs[i].trawlRecursively(new NominalRTM(prompt, bsObjs[i].Path), false));
				bsObjs[i].update();
			}
			for (int i = 0; i < bsObjs2.Count; i++) {
				bsObjs2[i].update();
			}
			var objs = new List<FSOBase>();
			objs.AddRange(bsObjs);
			objs.AddRange(bsObjs2);
			objs.AddRange(objs2Add);
			side2 = new Integrator(integratorTypes, 1, objs.Count);
			for (int i = 0; i < objs.Count; i++) {
				side2.set_block(0,i,objs[i].createElements());
			}
			Console.WriteLine("Verifying...");
			var verstat = true;
			for (int i = 0; i < side1.rowCount; i++) {
				var ceblock = side1.get_block(0, i);
				var csindx = findElementBlock(side2, (String)((StringElement)ceblock[1]).HeldElement);
				if (csindx[0] == -1 || csindx[1] == -1) {
					var cfso = constructFSO(ceblock);
					Console.WriteLine("Detect Failed: " + cfso.Path);
					verstat = verstat && false;
				} else {
					var ceblockupd = side2.get_block(csindx[0], csindx[1]);
					var cfso = constructFSO(ceblock);
					var cfsoupd = constructFSO(ceblockupd);
					Console.WriteLine("Detect Succeeded: " + cfso.Path);
					var lmb = cfso.LastModified == cfsoupd.LastModified;
					Console.WriteLine("Last Modified DateTime Equal: " + lmb);
					var hb = cfso.Hash == cfsoupd.Hash;
					Console.WriteLine("Hash Equal: " + hb);
					var sb = cfso.Size == cfsoupd.Size;
					Console.WriteLine("Size Equal: " + sb);
					var tsb = lmb && hb && sb;
					Console.WriteLine("Verified Equal: " + tsb);
					verstat = verstat && tsb;
				}
			}
			for (int i = 0; i < side2.rowCount; i++) {
				var ceblock = side2.get_block(0, i);
				var csindx = findElementBlock(side1, (String)((StringElement)ceblock[1]).HeldElement);
				if (csindx[0] == -1 || csindx[1] == -1) {
					var cfso = constructFSO(ceblock);
					Console.WriteLine("Detect Other: " + cfso.Path);
					verstat = verstat && false;
				}
			}
			Console.WriteLine("Verify Status: " + verstat);
		}
		
		static void update() {
			Console.WriteLine("Updating...");
			if (serializeIntegration) {
				var sav = new BinaryLoaderSaver();
				side2.save(sav);
				File.WriteAllBytes(integrationFile, sav.data);
			} else {
				var sav = new XMLLoaderSaver();
				side2.save(sav);
				File.WriteAllText(integrationFile, sav.data);
			}
		}
		
		static FSOBase constructFSO(IElement[] elementsIn) {
			var fsot = (FSOType)((FSOTypeElement)elementsIn[0]).HeldElement;
			if (fsot == FSOType.RootDirectory || fsot == FSOType.Directory) {
				var ex = Directory.Exists((String)((StringElement)elementsIn[1]).HeldElement);
				return new FSODirectory(elementsIn) { Info = info , Exists = ex };
			} else if (fsot == FSOType.RootFile || fsot == FSOType.File) {
				var ex = File.Exists((String)((StringElement)elementsIn[1]).HeldElement);
				return new FSOFile(elementsIn) { Info = info , Exists = ex };
			}
			return null;
		}
		
		static int[] findElementBlock(Integrator INT,String path) {
			var pse = new StringElement(path);
			var findx = INT.findElement(pse);
			if (findx[0] == -1 || findx[1] == -1 || findx[2] == -1) {
				return new int[] {-1,-1};
			} else {
				return new int[] {findx[0], findx[1]};
			}
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
            Console.WriteLine("info or information : enables the display of info.");
			Console.WriteLine("p or prompt : enables prompting.");
			Console.WriteLine("ser or serialize : enables integration serialization.");
			Console.WriteLine("ip or includepath : specifies a path to include; %PATH%");
			Console.WriteLine("ep or excludepath : specifies a path to exclude; %PATH%");
			Console.WriteLine("ia or includeattributes : specifies an attribute set to include; %ATTRIBUTE%...(Repeat)...");
			Console.WriteLine("ea or excludeattributes : specifies am attribute set to exclude; %ATTRIBUTE%...(Repeat)...");
			Console.WriteLine("Argument Reference:");
			Console.WriteLine("%SETUPCODE%:");
			Console.WriteLine("C : Create.");
			Console.WriteLine("L : Load.");
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
			for (int i = 0; i < theArg.Length; i++) {
				var c = theArg.ToLower()[i];
				switch (c) {
					case 'c':
						toret.Add(setups.Create);
						break;
					case 'u':
						toret.Add(setups.Update);
						break;
					case 'v':
						toret.Add(setups.Verify);
						break;
                    case 'l':
                        toret.Add(setups.Load);
                        break;
				}
			}
			return toret;
		}
		
		static FileAttributes parseAttributes(String theArg) {
			var toret = (FileAttributes) 0;
			for (int i = 0; i < theArg.Length; i++) {
				var c = theArg.ToLower()[i];
				toret = addAnotherAttribute(toret, c);
			}
			return toret;
		}
		
		static FileAttributes addAnotherAttribute(FileAttributes cfa, Char code) {
            var toret = cfa;
			switch (code) {
				case '1':
					toret = (cfa | FileAttributes.Archive);
                    break;
				case '2':
					toret = (cfa | FileAttributes.Compressed);
                    break;
				case '3':
					toret = (cfa | FileAttributes.Device);
                    break;
				case '4':
					toret = (cfa | FileAttributes.Directory);
                    break;
				case '5':
					toret = (cfa | FileAttributes.Encrypted);
                    break;
				case '6':
                    toret = (cfa | FileAttributes.Hidden);
                    break;
				case '7':
					toret = (cfa | FileAttributes.Normal);
                    break;
				case '8':
					toret = (cfa | FileAttributes.NotContentIndexed);
                    break;
				case '9':
					toret = (cfa | FileAttributes.Offline);
                    break;
				case '0':
					toret = (cfa | FileAttributes.ReadOnly);
                    break;
				case 'a':
					toret = (cfa | FileAttributes.ReparsePoint);
                    break;
				case 'b':
					toret = (cfa | FileAttributes.SparseFile);
                    break;
				case 'c':
					toret = (cfa | FileAttributes.System);
                    break;
				case 'd':
					toret = (cfa | FileAttributes.Temporary);
                    break;
                default:
                    toret = cfa;
                    break;
			}
            return toret;
		}
	}
	
	enum setups {
		Create = 1,
		Load = 2,
		Verify = 3,
		Update = 4
	}
}