/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 22/08/2019
 * Time: 17:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace captainalm.integrator.verifier
{
	/// <summary>
	/// File System Object Directory.
	/// </summary>
	public class FSODirectory : FSOBase
	{
		protected List<FSOBase> subFSO = new List<FSOBase>();
		
		public FSODirectory(String dirPathIn) : base(dirPathIn, FSOType.Directory) {}
		public FSODirectory(String dirPathIn, FSOType dirTyp) : base(dirPathIn, dirTyp) {}
		public FSODirectory(IElement[] elementsIn) : base(elementsIn) {}

		public virtual FSOBase[] trawl(Boolean includeDirectories, Boolean doUpdates) {
			if (info) { Console.WriteLine("Trawling: " + path); }
			var toret = new List<FSOBase>();
			subFSO.Clear();
			var files = new List<String>(Directory.EnumerateFiles(path));
			for (int i = 0; i < files.Count; i++)
			{
				var c = new FSOFile(files[i]) { Info = this.info };
				if (doUpdates) { c.update(); }
				toret.Add(c);
			}
			if (includeDirectories) {
				var dirs = new List<String>(Directory.EnumerateDirectories(path));
				for (int i = 0; i < dirs.Count; i++) {
					var c = new FSODirectory(dirs[i]) { Info = this.info };
					if (doUpdates) { c.update(); }
					toret.Add(c);
				}
			}
			subFSO.AddRange(toret);
			return toret.ToArray();
		}
		
		public virtual FSOBase[] trawlRecursively(Boolean doUpdates) {
			if (info) { Console.WriteLine("Recursively Trawling: " + path); }
			var toret = new List<FSOBase>();
			subFSO.Clear();
			var localObjs = new List<FSOBase>();
			var files = new List<String>(Directory.EnumerateFiles(path));
			for (int i = 0; i < files.Count; i++)
			{
				var c = new FSOFile(files[i]) { Info = this.info };
				if (doUpdates) { c.update(); }
				toret.Add(c);
				localObjs.Add(c);
			}
			var dirs = new List<String>(Directory.EnumerateDirectories(path));
			for (int i = 0; i < dirs.Count; i++) {
				var c = new FSODirectory(dirs[i]) { Info = this.info };
				toret.Add(c);
				localObjs.Add(c);
				toret.AddRange(c.trawlRecursively(doUpdates));
				if (doUpdates) { c.update(); }
			}
			subFSO.AddRange(localObjs);
			return toret.ToArray();
		}

		public virtual FSOBase[] trawlRecursively(IRecursiveTrawlerManager rtm, Boolean doUpdates)
		{
			if (info) { Console.WriteLine("Recursively Trawling: " + path); }
			var toret = new List<FSOBase>();
			subFSO.Clear();
			var localObjs = new List<FSOBase>();
			var files = new List<String>(Directory.EnumerateFiles(path));
			for (int i = 0; i < files.Count; i++)
			{
				var c = new FSOFile(files[i]) { Info = this.info };
				if (rtm.shouldTrawl(c))
				{
					if (doUpdates) { c.update(); }
					toret.Add(c);
					localObjs.Add(c);
				}
			}
			var dirs = new List<String>(Directory.EnumerateDirectories(path));
			for (int i = 0; i < dirs.Count; i++)
			{
				var c = new FSODirectory(dirs[i]) { Info = this.info };
				if (rtm.shouldTrawl(c))
				{
					toret.Add(c);
					localObjs.Add(c);
					toret.AddRange(c.trawlRecursively(rtm, doUpdates));
					if (doUpdates) { c.update(); }
				}
			}
			subFSO.AddRange(localObjs);
			return toret.ToArray();
		}
		
		public override void update()
		{
			if (info) { Console.WriteLine("Updating: " + path); }
			exists = Directory.Exists(path);
			for (int i = 0; i < subFSO.Count; i++) {
				var c = subFSO[i];
				c.update();
			}
			if (exists) {base.update();}
		}
		
		#region implemented abstract members of FSOBase

		public override void updateLastModified()
		{
			var fi = new DirectoryInfo(path);
			fi.Refresh();
			lastModified = fi.LastWriteTime;
			fi = null;
		}

		public override void updateHash()
		{
			var str = "";
			for (int i = 0; i < subFSO.Count; i++) {
				var c = subFSO[i];
				str += c.Hash;
			}
			hash = BitConverter.ToString(hashProcessor(System.Text.Encoding.ASCII.GetBytes(str))).Replace("-",String.Empty);
		}

		public override void updateSize()
		{
			size = 0;
			for (int i = 0; i < subFSO.Count; i++) {
				var c = subFSO[i];
				size += c.Size;
			}
		}
		
		public override FileAttributes Attributes {
			get {
				if (Directory.Exists(path)) {
					var toret = FileAttributes.Normal;
					var fi = new DirectoryInfo(path);
					fi.Refresh();
					toret = fi.Attributes;
					fi = null;
					return toret;
				} else {
					return FileAttributes.Normal;
				}
			}
		}

		#endregion
		
		public virtual FSOBase[] contents {
			get {
				return subFSO.ToArray();
			}
		}
	}

	public interface IRecursiveTrawlerManager
	{
		Boolean shouldTrawl(FSOBase objIn);
	}

	public class NominalRTM : IRecursiveTrawlerManager
	{
		protected Boolean prompt = false;
		protected String basePath = "";
		protected List<String> includedBasePaths = new List<String>();

		public NominalRTM() { prompt = false; basePath = "";}
		public NominalRTM(String basePathIn) {prompt = false; basePath = basePathIn;}
		public NominalRTM(Boolean doPrompt) { prompt = doPrompt; basePath = "";}
		public NominalRTM(Boolean doPrompt, String basePathIn) { prompt = doPrompt; basePath = basePathIn;}

		public virtual bool shouldTrawl(FSOBase c)
		{
			var res = false;
			if (!isFlagSetContained(Program.excludedAttributes, c.Attributes))
			{
				res = Program.includedAttributes.Count <= 0 || isFlagSetContained(Program.includedAttributes, c.Attributes);
			}
			else
			{
				res = false;
			}
			if (res)
			{
				if (!isPathContained(Program.excludedPaths, c.Path))
				{
					res = Program.includedPaths.Count <= 0 || isPathContained(Program.includedPaths, c.Path);
				}
				else
				{
					res = false;
				}
			}
			else
			{
				res = Program.includedPaths.Count > 0 && isPathContained(Program.includedPaths, c.Path);
			}
			var skpp = false;
			if (res) {
				skpp = isPathContained(includedBasePaths, c.Path);
			}
			if (res && prompt && ! skpp)
			{
				Console.WriteLine("Include: " + c.Path + " ?");
				Console.WriteLine("[Else] Include, " + ((c.Type == FSOType.RootDirectory || c.Type == FSOType.Directory) ? "[I] Include All Sub Objects, " : "") + "[E] Exclude, [X] Exit");
				var rkey = Console.ReadKey();
				Console.WriteLine();
				if (rkey.Key == ConsoleKey.E)
				{
					res = false;
				}
				else if (rkey.Key == ConsoleKey.X)
				{
					res = false;
					Environment.Exit(0);
				} else if (rkey.Key == ConsoleKey.I && (c.Type == FSOType.RootDirectory || c.Type == FSOType.Directory)) {
					addIncludedBasePath(c.Path);
				}
			}
			return res;
		}
		
		public virtual void addIncludedBasePath(String pathIn) {
			var pathInFP = (IsFullPath(pathIn)) ? pathIn : ConvertRelativePathToFullPath(basePath, pathIn);
			if (! isPathContained(includedBasePaths, pathInFP)) {
				includedBasePaths.Add(pathInFP);
			}
		}
		
		public virtual void removeIncludedBasePath(String pathIn) {
			var pathInFP = (IsFullPath(pathIn)) ? pathIn : ConvertRelativePathToFullPath(basePath, pathIn);
			if (isPathContained(includedBasePaths, pathInFP)) {
				includedBasePaths.Remove(pathInFP);
			}
		}
		
		public virtual void clearIncludedBasePaths() {
			includedBasePaths.Clear();
		}
		
		protected virtual Boolean isPathContained(List<String> pathsIn, String pToCheck) {
			var toret = false;
			var pToCheckFP = (IsFullPath(pToCheck)) ? pToCheck : ConvertRelativePathToFullPath(basePath, pToCheck);
			for (int i = 0; i < pathsIn.Count; i++) {
				var c = pathsIn[i];
				var cfp = (IsFullPath(c)) ? c : ConvertRelativePathToFullPath(basePath, c);
				var cfpisfile = File.Exists(cfp);
				if (
					pToCheck.IndexOf(c, StringComparison.OrdinalIgnoreCase) >= 0 ||
					pToCheck.IndexOf(cfp, StringComparison.OrdinalIgnoreCase) >= 0 ||
					pToCheckFP.IndexOf(c,StringComparison.OrdinalIgnoreCase) >= 0 ||
					pToCheckFP.IndexOf(cfp,StringComparison.OrdinalIgnoreCase) >= 0
				)
				{
					toret = true;
					break;
				} else if ((cfpisfile) && pToCheckFP.StartsWith(Path.GetDirectoryName(cfp), StringComparison.OrdinalIgnoreCase)) {
					toret = true;
					break;
				} else if ((! cfpisfile) && pToCheckFP.StartsWith(cfp, StringComparison.OrdinalIgnoreCase)) {
					toret = true;
					break;
				}
			}
			return toret;
		}

		protected virtual Boolean isFlagSetContained(List<FileAttributes> attributesIn, FileAttributes attributeToCheck) {
			var toret = false;
			for (int i = 0; i < attributesIn.Count; i++)
			{
				var c = attributesIn[i];
				if ((attributeToCheck & c) == c)
				{
					toret = true;
					break;
				}
			}
			return toret;
		}
		
		protected bool IsFullPath(string pathIn)
		{
			if (string.IsNullOrWhiteSpace(pathIn) || pathIn.IndexOfAny(Path.GetInvalidPathChars()) != -1 || !Path.IsPathRooted(pathIn)) {return false;}
			string pathInRoot = Path.GetPathRoot(pathIn);
			if (pathInRoot.Length <= 2 && pathInRoot != "/") {return false;}
			if (pathInRoot[0] != '\\' || pathInRoot[1] != '\\') {return true;}
			return pathInRoot.Trim('\\').IndexOf('\\') != -1;
		}
		
		protected string ConvertRelativePathToFullPath(string basePathIn, string pathIn)
		{
			if (IsFullPath(pathIn)) {return pathIn;} else {return Path.GetFullPath(Path.Combine(basePathIn, pathIn));}
		}
	}

}