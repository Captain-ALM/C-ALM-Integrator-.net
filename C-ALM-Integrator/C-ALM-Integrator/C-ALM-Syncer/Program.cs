/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 27/08/2019
 * Time: 16:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace captainalm.integrator.syncer
{
	/// <summary>
	/// Main Execution Class.
	/// </summary>
	static class Program
	{
		public static StringArrayParser argsParser = null;
		public static String sourcePath1 = "";
		public static String sourcePath2 = "";
		private static String _sourcePath1 = "";
		private static String _sourcePath2 = "";
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
		public static List<diffObj> diffHolder = new List<diffObj>();
		public static modes transferMode = modes.Copy;
		
		public static void Main(string[] args)
		{
			Console.WriteLine("C-ALM Syncer : (C) Captain ALM 2019");
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
			if (argsParser.hasSwitchIgnoreCase("m")) {
				var switches = argsParser.get_argDataIgnoreCase("m");
				if (! object.ReferenceEquals(null, switches[0])) {
					transferMode = parseMode(switches[0]);
				} else {
					throw new ArgumentException("m");
				}
			} else if (argsParser.hasSwitchIgnoreCase("mode")) {
				var switches = argsParser.get_argDataIgnoreCase("mode");
				if (! object.ReferenceEquals(null, switches[0])) {
					transferMode = parseMode(switches[0]);
				} else {
					throw new ArgumentException("mode");
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
			if (argsParser.hasSwitchIgnoreCase("s1")) {
				var switches = argsParser.get_argDataIgnoreCase("s1");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath1 = Path.GetFullPath(switches[0]);
				} else {
					throw new ArgumentException("s1");
				}
			} else if (argsParser.hasSwitchIgnoreCase("source1")) {
				var switches = argsParser.get_argDataIgnoreCase("source1");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath1 = Path.GetFullPath(switches[0]);
				} else {
					throw new ArgumentException("source1");
				}
			} else if (argsParser.hasSwitchIgnoreCase("sd1")) {
				var switches = argsParser.get_argDataIgnoreCase("sd1");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath1 = Path.GetFullPath(switches[0]);
				} else {
					throw new ArgumentException("sd1");
				}
			} else if (argsParser.hasSwitchIgnoreCase("sourcedirectory1")) {
				var switches = argsParser.get_argDataIgnoreCase("sourcedirectory1");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath1 = Path.GetFullPath(switches[0]);
				} else {
					throw new ArgumentException("sourcedirectory1");
				}
			}
			if (argsParser.hasSwitchIgnoreCase("s2")) {
				var switches = argsParser.get_argDataIgnoreCase("s2");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath2 = Path.GetFullPath(switches[0]);
				} else {
					throw new ArgumentException("s2");
				}
			} else if (argsParser.hasSwitchIgnoreCase("source2")) {
				var switches = argsParser.get_argDataIgnoreCase("source2");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath2 = Path.GetFullPath(switches[0]);
				} else {
					throw new ArgumentException("source2");
				}
			} else if (argsParser.hasSwitchIgnoreCase("sd2")) {
				var switches = argsParser.get_argDataIgnoreCase("sd2");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath2 = Path.GetFullPath(switches[0]);
				} else {
					throw new ArgumentException("sd2");
				}
			} else if (argsParser.hasSwitchIgnoreCase("sourcedirectory2")) {
				var switches = argsParser.get_argDataIgnoreCase("sourcedirectory2");
				if (! object.ReferenceEquals(null, switches[0])) {
					sourcePath2 = Path.GetFullPath(switches[0]);
				} else {
					throw new ArgumentException("sourcedirectory2");
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
			//Directory.SetCurrentDirectory(sourcePath1); // : FIX needed for relative mode
			for (int i = 0; i < setup.Count; i++) {
				var c = setup[i];
				switch (c) {
					case setups.Create:
						create();
						break;
					case setups.Load:
						load();
						break;
					case setups.Update:
						update();
						break;
					case setups.Diff:
						diff();
						break;
					case setups.SyncTo1:
						syncTo1();
						break;
					case setups.SyncTo2:
						syncTo2();
						break;
					case setups.Sync:
						sync();
						break;
				}
			}
		}
		
		static void sync() {
			Console.WriteLine("Syncing...");
			var tv0 = new NominalRTM(_sourcePath1);
			var tv1 = new NominalRTM(_sourcePath2);
			for (int i = 0; i < diffHolder.Count; i++) {
				var cd = diffHolder[i];
				if (info) {Console.WriteLine("Syncing: " + cd.relPath);}
				var dopref = cd.pref;
				if (prompt) {
					Console.WriteLine("Sync: " + cd.relPath + " ?");
					var pref = (cd.pref == -1) ? "Skip" : cd.pref.ToString();
					Console.WriteLine("Prefered Block: " + pref);
					Console.WriteLine("[Else] Skip, [0] Sync To 0, [1] Sync To 1, [P] Sync From Prefered, [X] Exit");
					var rkey = Console.ReadKey();
					Console.WriteLine();
					switch (rkey.Key) {
						case ConsoleKey.D0:
							dopref = 0;
							break;
						case ConsoleKey.D1:
							dopref = 1;
							break;
						case ConsoleKey.P:
							dopref = cd.pref;
							break;
						case ConsoleKey.X:
							dopref = -1;
							Environment.Exit(0);
							break;
						default:
							dopref = -1;
							break;
					}
				}
				if ((! cd.same) && cd.exist0 && (cd.obj0.Type != FSOType.Directory && cd.obj0.Type != FSOType.RootDirectory) && tv0.shouldTrawl(cd.obj0) && dopref == 0) {
					if (transferMode == modes.Copy) {
						if (! Directory.Exists(Path.GetDirectoryName(cd.obj1.Path))) {Directory.CreateDirectory(Path.GetDirectoryName(cd.obj1.Path));}
						File.Copy(cd.obj0.Path, cd.obj1.Path, true);
						File.SetAttributes(cd.obj1.Path, cd.obj0.Attributes);
					} else if (transferMode == modes.Get) {
						if (! Directory.Exists(Path.GetDirectoryName(cd.obj1.Path))) {Directory.CreateDirectory(Path.GetDirectoryName(cd.obj1.Path));}
						using (var fsr = new FileStream(cd.obj0.Path, FileMode.Open, FileAccess.Read, FileShare.Read, 10485760)) {
							using (var fsw = new FileStream(cd.obj1.Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 10485760)) {
								var clenlft = fsr.Length;
								while (clenlft > 0) {
									var carr = new Byte[0];
									if (clenlft > 10485760) {
										carr = new Byte[10485760];
										fsr.Read(carr, 0, carr.Length);
										fsw.Write(carr,0,carr.Length);
										clenlft -= 10485760;
									} else {
										carr = new Byte[clenlft];
										fsr.Read(carr, 0, carr.Length);
										fsw.Write(carr,0,carr.Length);
										clenlft -= clenlft;
									}
								}
							}
						}
						File.SetAttributes(cd.obj1.Path, cd.obj0.Attributes);
					}
				}
				else if ((! cd.same) && cd.exist1 && (cd.obj1.Type != FSOType.Directory && cd.obj1.Type != FSOType.RootDirectory) && tv1.shouldTrawl(cd.obj1) && dopref == 1) {
					if (transferMode == modes.Copy) {
						if (! Directory.Exists(Path.GetDirectoryName(cd.obj0.Path))) {Directory.CreateDirectory(Path.GetDirectoryName(cd.obj0.Path));}
						File.Copy(cd.obj1.Path, cd.obj0.Path, true);
						File.SetAttributes(cd.obj0.Path, cd.obj1.Attributes);
					} else if (transferMode == modes.Get) {
						if (! Directory.Exists(Path.GetDirectoryName(cd.obj0.Path))) {Directory.CreateDirectory(Path.GetDirectoryName(cd.obj0.Path));}
						using (var fsr = new FileStream(cd.obj1.Path, FileMode.Open, FileAccess.Read, FileShare.Read, 10485760)) {
							using (var fsw = new FileStream(cd.obj0.Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 10485760)) {
								var clenlft = fsr.Length;
								while (clenlft > 0) {
									var carr = new Byte[0];
									if (clenlft > 10485760) {
										carr = new Byte[10485760];
										fsr.Read(carr, 0, carr.Length);
										fsw.Write(carr,0,carr.Length);
										clenlft -= 10485760;
									} else {
										carr = new Byte[clenlft];
										fsr.Read(carr, 0, carr.Length);
										fsw.Write(carr,0,carr.Length);
										clenlft -= clenlft;
									}
								}
							}
						}
						File.SetAttributes(cd.obj0.Path, cd.obj1.Attributes);
					}
				} else if (((! cd.same) && cd.exist0 && (cd.obj0.Type == FSOType.Directory || cd.obj0.Type == FSOType.RootDirectory) && tv0.shouldTrawl(cd.obj0) && dopref == 0)) {
					if (! Directory.Exists(cd.obj1.Path)) {Directory.CreateDirectory(cd.obj1.Path);}
				} else if (((! cd.same) && cd.exist1 && (cd.obj1.Type == FSOType.Directory || cd.obj1.Type == FSOType.RootDirectory) && tv1.shouldTrawl(cd.obj1) && dopref == 1)) {
					if (! Directory.Exists(cd.obj0.Path)) {Directory.CreateDirectory(cd.obj0.Path);}
				}
			}
			tv0 = null;
			tv1 = null;
		}
		
		static void syncTo1() {
			Console.WriteLine("Syncing from 1 to 0...");
			var tv1 = new NominalRTM(_sourcePath2);
			for (int i = 0; i < diffHolder.Count; i++) {
				var cd = diffHolder[i];
				var doit = true;
				if (info) {Console.WriteLine("Syncing: " + cd.relPath);}
				if ((! cd.same) && cd.exist1 && tv1.shouldTrawl(cd.obj1)) {
					if (prompt) {
						Console.WriteLine("Sync From 1 to 0: " + cd.relPath + " ?");
						Console.WriteLine("[Else] Yes, [N] No, [X] Exit");
						var rkey = Console.ReadKey();
						Console.WriteLine();
						if (rkey.Key == ConsoleKey.N)
						{
							doit = false;
						}
						else if (rkey.Key == ConsoleKey.X)
						{
							doit = false;
							Environment.Exit(0);
						}
					}
					if (transferMode == modes.Copy && doit && (cd.obj1.Type != FSOType.Directory && cd.obj1.Type != FSOType.RootDirectory)) {
						if (! Directory.Exists(Path.GetDirectoryName(cd.obj0.Path))) {Directory.CreateDirectory(Path.GetDirectoryName(cd.obj0.Path));}
						File.Copy(cd.obj1.Path, cd.obj0.Path, true);
						File.SetAttributes(cd.obj0.Path, cd.obj1.Attributes);
					} else if (transferMode == modes.Get && doit && (cd.obj1.Type != FSOType.Directory && cd.obj1.Type != FSOType.RootDirectory)) {
						if (! Directory.Exists(Path.GetDirectoryName(cd.obj0.Path))) {Directory.CreateDirectory(Path.GetDirectoryName(cd.obj0.Path));}
						using (var fsr = new FileStream(cd.obj1.Path, FileMode.Open, FileAccess.Read, FileShare.Read, 10485760)) {
							using (var fsw = new FileStream(cd.obj0.Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 10485760)) {
								var clenlft = fsr.Length;
								while (clenlft > 0) {
									var carr = new Byte[0];
									if (clenlft > 10485760) {
										carr = new Byte[10485760];
										fsr.Read(carr, 0, carr.Length);
										fsw.Write(carr,0,carr.Length);
										clenlft -= 10485760;
									} else {
										carr = new Byte[clenlft];
										fsr.Read(carr, 0, carr.Length);
										fsw.Write(carr,0,carr.Length);
										clenlft -= clenlft;
									}
								}
							}
						}
						File.SetAttributes(cd.obj0.Path, cd.obj1.Attributes);
					} else if (((cd.obj1.Type == FSOType.Directory || cd.obj1.Type == FSOType.RootDirectory) && doit)) {
						if (! Directory.Exists(cd.obj0.Path)) {Directory.CreateDirectory(cd.obj0.Path);}
					}
				}
			}
			tv1 = null;
		}
		
		static void syncTo2() {
			Console.WriteLine("Syncing from 0 to 1...");
			var tv0 = new NominalRTM(_sourcePath1);
			for (int i = 0; i < diffHolder.Count; i++) {
				var cd = diffHolder[i];
				var doit = true;
				if (info) {Console.WriteLine("Syncing: " + cd.relPath);}
				if ((! cd.same) && cd.exist0 && tv0.shouldTrawl(cd.obj0)) {
					if (prompt) {
						Console.WriteLine("Sync From 0 to 1: " + cd.relPath + " ?");
						Console.WriteLine("[Else] Yes, [N] No, [X] Exit");
						var rkey = Console.ReadKey();
						Console.WriteLine();
						if (rkey.Key == ConsoleKey.N)
						{
							doit = false;
						}
						else if (rkey.Key == ConsoleKey.X)
						{
							doit = false;
							Environment.Exit(0);
						}
					}
					if (transferMode == modes.Copy && doit && (cd.obj0.Type != FSOType.Directory && cd.obj0.Type != FSOType.RootDirectory)) {
						if (! Directory.Exists(Path.GetDirectoryName(cd.obj1.Path))) {Directory.CreateDirectory(Path.GetDirectoryName(cd.obj1.Path));}
						File.Copy(cd.obj0.Path, cd.obj1.Path, true);
						File.SetAttributes(cd.obj1.Path, cd.obj0.Attributes);
					} else if (transferMode == modes.Get && doit && (cd.obj0.Type != FSOType.Directory && cd.obj0.Type != FSOType.RootDirectory)) {
						if (! Directory.Exists(Path.GetDirectoryName(cd.obj1.Path))) {Directory.CreateDirectory(Path.GetDirectoryName(cd.obj1.Path));}
						using (var fsr = new FileStream(cd.obj0.Path, FileMode.Open, FileAccess.Read, FileShare.Read, 10485760)) {
							using (var fsw = new FileStream(cd.obj1.Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 10485760)) {
								var clenlft = fsr.Length;
								while (clenlft > 0) {
									var carr = new Byte[0];
									if (clenlft > 10485760) {
										carr = new Byte[10485760];
										fsr.Read(carr, 0, carr.Length);
										fsw.Write(carr,0,carr.Length);
										clenlft -= 10485760;
									} else {
										carr = new Byte[clenlft];
										fsr.Read(carr, 0, carr.Length);
										fsw.Write(carr,0,carr.Length);
										clenlft -= clenlft;
									}
								}
							}
						}
						File.SetAttributes(cd.obj1.Path, cd.obj0.Attributes);
					} else if (((cd.obj0.Type == FSOType.Directory || cd.obj0.Type == FSOType.RootDirectory) && doit)) {
						if (! Directory.Exists(cd.obj1.Path)) {Directory.CreateDirectory(cd.obj1.Path);}
					}
				}
			}
			tv0 = null;
		}
		
		static void diff() {
			Console.WriteLine("Processing Diff...");
			diffHolder.Clear();
			//File processor only supports 1 root directory per block (Unlike C-ALM Verifier)
			var indxs = side1.findElements(new FSOTypeElement(FSOType.RootDirectory));
			int[] indx0 = null;
			bool indx0d = false;
			bool indx1d = false;
			int[] indx1 = null;
			for (int i = 0; i < indxs.Length; i++) {
				var c = indxs[i];
				if (c[0] == 0 && ! indx0d) {
					indx0 = c;
					indx0d = true;
				} else if (c[0] == 1 && !indx1d) {
					indx1 = c;
					indx1d = true;
				}
			}
			if (! (indx0d) || ! (indx1d)) {throw new InvalidOperationException("One of the blocks does not have a root directory.");}
			var fbsdir0 = constructFSO(side1.get_block(indx0[0], indx0[1]));
			var fbsdir1 = constructFSO(side1.get_block(indx1[0], indx1[1]));
			_sourcePath1 = Path.GetFullPath(fbsdir0.Path);
			_sourcePath2 = Path.GetFullPath(fbsdir1.Path);
			fbsdir0 = new FSODirectory(_sourcePath1, FSOType.RootDirectory);
			fbsdir0.update();
			fbsdir1 = new FSODirectory(_sourcePath2, FSOType.RootDirectory);
			fbsdir1.update();
			side2 = unify(createSyncMap(_sourcePath1, _sourcePath1),createSyncMap(_sourcePath2, _sourcePath2), _sourcePath1, _sourcePath2);
			for (int i = 0; i < side2.rowCount; i++) {
				var obj0 = constructFSO(side2.get_block(0, i));
				var obj1 = constructFSO(side2.get_block(1, i));
				obj1.update();
				var rels = new List<string>();
				rels.AddRange(convertToRelative(new List<FSOBase>(new FSOBase[] {obj0}), _sourcePath1));
				rels.AddRange(convertToRelative(new List<FSOBase>(new FSOBase[] {obj1}), _sourcePath2));
				if (rels[0] != rels[1]) {throw new InvalidOperationException("The relative paths are not equal.");}
				var pref = -1;
				pref = (pref == -1) ? ((obj0.LastModified < obj1.LastModified) ? 1 : ((obj0.LastModified > obj1.LastModified) ? 0 : pref)) : pref;
				pref = (pref == -1) ? ((obj0.Size < obj1.Size) ? 1 : ((obj0.Size > obj1.Size) ? 0 : pref)) : pref;
				if (obj0.Hash == obj1.Hash) {pref = -1;}
				diffHolder.Add(new diffObj(rels[0], obj0, obj1, pref));
			}
		}
		
		static void create() {
			Console.WriteLine("Creating...");
			side1 = unify(createSyncMap(sourcePath1, sourcePath1),createSyncMap(sourcePath2, sourcePath2), sourcePath1, sourcePath2);
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
		
		static List<FSOBase> createSyncMap(String pathIn, String basePathIn) {
			if (info) {Console.WriteLine("Creating Sync Map for: " + pathIn);}
			var baseDir = new FSODirectory(pathIn, FSOType.RootDirectory) { Info = info };
			var subObjs = baseDir.trawlRecursively(new NominalRTM(prompt, basePathIn), false);
			baseDir.update();
			var objs = new List<FSOBase>();
			objs.Add(baseDir);
			objs.AddRange(subObjs);
			return objs;
		}
		
		static List<String> convertToRelative(List<FSOBase> fsobjsIn, String baseIn) {
			var toret = new List<String>();
			for (int i = 0; i < fsobjsIn.Count; i++) {
				if (info) {Console.WriteLine("Converting to relative: " + fsobjsIn[i].Path);}
				toret.Add(fsobjsIn[i].Path.Replace(baseIn, ""));
				if (toret[i].StartsWith("/") || toret[i].StartsWith("\\")) {toret[i] = toret[i].Substring(1, toret[i].Length - 1);}
			}
			return toret;
		}
		static Integrator unify(List<FSOBase> block0, List<FSOBase> block1, String basePath1, String basePath2) {
			var block0r = convertToRelative(block0, basePath1);
			var block1r = convertToRelative(block1, basePath2);
			var unityr = new List<string>();
			for (int i = 0; i < block0r.Count; i++) {
				if (info) {Console.WriteLine("Unifying: Phase 0: " + block0r[i]);}
				if(! unityr.Contains(block0r[i])) {
					unityr.Add(block0r[i]);
				}
			}
			for (int i = 0; i < block1r.Count; i++) {
				if (info) {Console.WriteLine("Unifying: Phase 1: " + block1r[i]);}
				if(! unityr.Contains(block1r[i])) {
					unityr.Add(block1r[i]);
				}
			}
			var INT = new Integrator(integratorTypes, 2, unityr.Count);
			for (int i = 0; i < unityr.Count; i++) {
				if (info) {Console.WriteLine("Unifying: Phase 2: " + unityr[i]);}
				var indx0 = block0r.IndexOf(unityr[i]);
				var indx1 = block1r.IndexOf(unityr[i]);
				if (indx0 == -1) {
					if (block1[indx1].Type == FSOType.Directory || block1[indx1].Type == FSOType.RootDirectory) {
						INT.set_block(0,i,new FSODirectory(Path.Combine(basePath1, unityr[i]), block1[indx1].Type).createElements());
					} else if(block1[indx1].Type == FSOType.File || block1[indx1].Type == FSOType.RootFile) {
						INT.set_block(0,i,new FSOFile(Path.Combine(basePath1, unityr[i]), block1[indx1].Type).createElements());
					}
					INT.set_block(1,i,block1[indx1].createElements());
				} else if(indx1 == -1) {
					if (block0[indx0].Type == FSOType.Directory || block0[indx0].Type == FSOType.RootDirectory) {
						INT.set_block(1,i,new FSODirectory(Path.Combine(basePath2, unityr[i]), block0[indx0].Type).createElements());
					} else if(block0[indx0].Type == FSOType.File || block0[indx0].Type == FSOType.RootFile) {
						INT.set_block(1,i,new FSOFile(Path.Combine(basePath2, unityr[i]), block0[indx0].Type).createElements());
					}
					INT.set_block(0,i,block0[indx0].createElements());
				} else {
					INT.set_block(0,i,block0[indx0].createElements());
					INT.set_block(1,i,block1[indx1].createElements());
				}
			}
			return INT;
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
			Console.WriteLine("C-ALM-Syncer.exe {[switch/=] [/arg] ...(Repeat)...}");
			Console.WriteLine("/ denotes optional part.");
			Console.WriteLine("Switches ; Argument Reference Link:");
			Console.WriteLine("?, u, usage, h or help : shows this message.");
			Console.WriteLine("set or setup : specifies the setup; %SETUPCODE%...(Repeat)...");
			Console.WriteLine("m or mode : specifies the mode of transfer; %TRANSFERCODE%");
			Console.WriteLine("i, int, integration, f, file : specifies the integration file; %PATH%");
			Console.WriteLine("s1, source1, sd1 or sourcedirectory1 : specifies the source directory for block 0; %PATH%");
			Console.WriteLine("s2, source1, sd2 or sourcedirectory2 : specifies the source directory for block 1; %PATH%");
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
			Console.WriteLine("D : Diff.");
			Console.WriteLine("S : Sync.");
			Console.WriteLine("1 : Sync to block 0.");
			Console.WriteLine("2 : Sync to block 1.");
			Console.WriteLine("%TRANSFERCODE%:");
			Console.WriteLine("C : Copy.");
			Console.WriteLine("G : Read into memory and then write from memory.");
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
					case 'l':
						toret.Add(setups.Load);
						break;
					case 'd':
						toret.Add(setups.Diff);
						break;
					case 's':
						toret.Add(setups.Sync);
						break;
					case '1':
						toret.Add(setups.SyncTo1);
						break;
					case '2':
						toret.Add(setups.SyncTo2);
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
		
		static modes parseMode(String theArg) {
			var toret = modes.Copy;
			var c = theArg.ToLower();
			switch (c) {
				case "c":
					toret = modes.Copy;
					break;
				case "g":
					toret = modes.Get;
					break;
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
		Update = 3,
		Diff = 4,
		Sync = 5,
		SyncTo1 = 6,
		SyncTo2 = 7
	}
	
	struct diffObj {
		public string relPath;
		public FSOBase obj0;
		public FSOBase obj1;
		public bool exist0;
		public bool exist1;
		public Int32 pref;
		public bool same;
		public diffObj(string pathIn,FSOBase o0, FSOBase o1, Int32 p) {
			relPath = pathIn;
			obj0 = o0;
			obj1 = o1;
			exist0 = o0.Exists;
			exist1 = o1.Exists;
			if (p >= -1 && p <= 1) {pref = p;} else {pref = -1;}
			same = (o0.Hash == o1.Hash);
		}
	}
	
	enum modes {
		Copy = 1,
		Get = 2
	}
}