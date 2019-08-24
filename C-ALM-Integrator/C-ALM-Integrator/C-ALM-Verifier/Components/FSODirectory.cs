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
		protected Boolean exists = false;
		protected List<FSOBase> subFSO = new List<FSOBase>();
		
		public FSODirectory(String dirPathIn) : base(dirPathIn, FSOType.Directory) {}
		public FSODirectory(String dirPathIn, FSOType dirTyp) : base(dirPathIn, dirTyp) {}

		public virtual FSOBase[] trawl(Boolean includeDirectories) {
			var toret = new List<FSOBase>();
			if (includeDirectories) {
				var dirs = new List<String>(Directory.EnumerateFiles(path));
				for (int i = 0; i < dirs.Count - 1; i++) {
					var c = new FSODirectory(dirs[i]);
					c.update();
					toret.Add(c);
				}
			}
			var files = new List<String>(Directory.EnumerateFiles(path));
			for (int i = 0; i < files.Count - 1; i++) {
				var c = new FSOFile(files[i]);
				c.update();
				toret.Add(c);
			}
			subFSO.AddRange(toret);
			return toret.ToArray();
		}
		
		public virtual FSOBase[] trawlRecursively() {
			var toret = new List<FSOBase>();
			var dirs = new List<String>(Directory.EnumerateFiles(path));
			for (int i = 0; i < dirs.Count - 1; i++) {
				var c = new FSODirectory(dirs[i]);
				toret.AddRange(c.trawlRecursively());
				c.update();
				toret.Add(c);
			}
			var files = new List<String>(Directory.EnumerateFiles(path));
			for (int i = 0; i < files.Count - 1; i++) {
				var c = new FSOFile(files[i]);
				c.update();
				toret.Add(c);
			}
			subFSO.AddRange(toret);
			return toret.ToArray();
		}
		
		public override void update()
		{
			exists = Directory.Exists(path);
			for (int i = 0; i < subFSO.Count - 1; i++) {
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
			for (int i = 0; i < subFSO.Count - 1; i++) {
				var c = subFSO[i];
				str += c.Hash;
			}
			hash = BitConverter.ToString(hashProcessor(System.Text.Encoding.ASCII.GetBytes(str))).Replace("-",String.Empty);
		}

		public override void updateSize()
		{
			size = 0;
			for (int i = 0; i < subFSO.Count - 1; i++) {
				var c = subFSO[i];
				size += c.Size;
			}
		}

		#endregion
		
		public virtual Boolean Exists {
			get {
				return exists;
			}
		}
		
		public virtual FileAttributes Attributes {
			get {
				if (exists) {
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
		
		public virtual FSOBase[] contents {
			get {
				return subFSO.ToArray();
			}
		}
	}
}
