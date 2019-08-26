/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 22/08/2019
 * Time: 16:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Security.Cryptography;

namespace captainalm.integrator.verifier
{
	/// <summary>
	/// File System Object File.
	/// </summary>
	public class FSOFile : FSOBase
	{
		public FSOFile(String filePathIn) : base(filePathIn, FSOType.File) {}
		public FSOFile(String filePathIn, FSOType filTyp) : base(filePathIn, filTyp) {}
		public FSOFile(IElement[] elementsIn) : base(elementsIn) {}

		public override void update()
		{
			exists = File.Exists(path);
			if (exists) {base.update();}
		}
		
		#region implemented abstract members of FSOBase
		
		public override void updateLastModified()
		{
			var fi = new FileInfo(path);
			fi.Refresh();
			lastModified = fi.LastWriteTime;
			fi = null;
		}

		public override void updateHash()
		{
			var toret = new Byte[64];
			using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite, 10485760)) {
				var hp = new SHA512Managed();
				toret = hp.ComputeHash(fs);
				hp.Dispose();
				hp = null;
			}
			hash = BitConverter.ToString(toret, 0, toret.Length).Replace("-",String.Empty);
		}

		public override void updateSize()
		{
			var fi = new FileInfo(path);
			fi.Refresh();
			size = fi.Length;
			fi = null;
		}
		
		public override FileAttributes Attributes {
			get {
				if (exists) {
					var toret = FileAttributes.Normal;
					var fi = new FileInfo(path);
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
	}
}
