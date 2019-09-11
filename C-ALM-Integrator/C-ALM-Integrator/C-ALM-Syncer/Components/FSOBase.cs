/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 22/08/2019
 * Time: 08:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace captainalm.integrator.syncer
{
	/// <summary>
	/// File System Object Base.
	/// </summary>
	public abstract class FSOBase
	{
		protected String path = "";
		protected DateTime lastModified = DateTime.MinValue;
		protected String hash = "";
		protected Int64 size = 0;
		protected FSOType type = FSOType.None;
		protected Boolean exists = false;
        protected Boolean info = false;
		
		public FSOBase(String pathIn, FSOType typeIn)
		{
			path = pathIn;
			type = typeIn;
		}
		
		public FSOBase(IElement[] elementsIn)
		{
			if (elementsIn.Length > 0) {
				type = (FSOType)elementsIn[0].HeldElement;
				if (elementsIn.Length > 1) {
					path = elementsIn[1].HeldElement as string;
					if (elementsIn.Length > 2) {
						lastModified = (DateTime)elementsIn[2].HeldElement;
						if (elementsIn.Length > 3) {
							hash = elementsIn[3].HeldElement as string;
							if (elementsIn.Length > 4) {
								size = (Int64)elementsIn[4].HeldElement;
							}
						}
					}
				}
			}
		}
		
		public abstract void updateLastModified();
		
		public abstract void updateHash();
		
		public abstract void updateSize();
		
		public virtual void update() {
			updateLastModified();
			updateHash();
			updateSize();
		}
		
		public virtual String Path {
			get {
				return path;
			}
			set {
				path = value;
			}
		}
		
		public virtual DateTime LastModified {
			get {
				return lastModified;
			}
		}
		
		public virtual String Hash {
			get {
				return hash;
			}
		}
		
		public virtual Int64 Size {
			get {
				return size;
			}
		}
		
		public virtual FSOType Type {
			get {
				return type;
			}
		}

        public virtual Boolean Info
        {
            get {
                return info;
            }
            set {
                info = value;
            }
        }
		
		protected virtual Byte[] hashProcessor(Byte[] dataIn) {
			var datlst = new List<Byte[]>();
			var datlft = dataIn.Length;
			var cindx = 0;
			var toretlen = 0;
			while (datlft > 0) {
				if (datlft > 10485760) {
					var buff = new Byte[10485760];
					System.Buffer.BlockCopy(dataIn, cindx, buff, 0, buff.Length);
					datlst.Add(hashProcess(buff));
					toretlen += datlst[datlst.Count - 1].Length;
					datlft -= buff.Length;
					cindx += buff.Length;
				} else {
					var buff = new Byte[datlft];
					System.Buffer.BlockCopy(dataIn, cindx, buff, 0, buff.Length);
					datlst.Add(hashProcess(buff));
					toretlen += datlst[datlst.Count - 1].Length;
					datlft -= buff.Length;
					cindx += buff.Length;
					break;
				}
			}
			var toret = new Byte[toretlen];
			cindx = 0;
			for (int i = 0; i < datlst.Count; i++) {
				var cbts = datlst[i];
				System.Buffer.BlockCopy(cbts, 0, toret, cindx, cbts.Length);
				cindx += cbts.Length;
			}
			return hashProcess(toret);
		}
		
		protected virtual Byte[] hashProcess(Byte[] dataIn) {
            var hp = new SHA512Managed();
			var toret = hp.ComputeHash(dataIn);
			hp.Dispose();
			hp = null;
			return toret;
		}
		
		public virtual IElement[] createElements() {
			var toret = new List<IElement>();
			toret.Add(new FSOTypeElement(type));
			toret.Add(new StringElement(path));
			toret.Add(new DateTimeElement(lastModified));
			toret.Add(new StringElement(hash));
			toret.Add(new LongElement(size));
			return toret.ToArray();
		}
		
		public virtual Boolean Exists {
			get {
				return exists;
			}
        	set {
        		exists = value;
        	}
		}
		
		public abstract FileAttributes Attributes {get;}
	}
}
