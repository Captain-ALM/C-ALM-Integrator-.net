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
				toret.AddRange(c.trawlRecursively(doUpdates));
                if (doUpdates) { c.update(); }
				toret.Add(c);
				localObjs.Add(c);
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
                    toret.AddRange(c.trawlRecursively(rtm, doUpdates));
                    if (doUpdates) { c.update(); }
                    toret.Add(c);
                    localObjs.Add(c);
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

        public NominalRTM() { prompt = false; }
        public NominalRTM(Boolean doPrompt) { prompt = doPrompt; }

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
            if (res && prompt)
            {
                Console.WriteLine("Include: " + c.Path + " ?");
                Console.WriteLine("[Else] Include, [E] Exclude, [X] Exit");
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
                }
            }
            return res;
        }
        
        protected virtual Boolean isPathContained(List<String> pathsIn, String pToCheck) {
			var toret = false;
            var pToCheckFP = Path.GetFullPath(pToCheck);
			for (int i = 0; i < pathsIn.Count; i++) {
				var c = pathsIn[i];
                var cfp = Path.GetFullPath(c);
                if (
                    pToCheck.IndexOf(c, StringComparison.OrdinalIgnoreCase) >= 0 || 
                    pToCheck.IndexOf(cfp, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    pToCheckFP.IndexOf(c,StringComparison.OrdinalIgnoreCase) >= 0 ||
                    pToCheckFP.IndexOf(cfp,StringComparison.OrdinalIgnoreCase) >= 0
                    )
                {
					toret = true;
					break;
				} else if (pToCheckFP.StartsWith(Path.GetDirectoryName(cfp), StringComparison.OrdinalIgnoreCase)) {
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
    }

}
