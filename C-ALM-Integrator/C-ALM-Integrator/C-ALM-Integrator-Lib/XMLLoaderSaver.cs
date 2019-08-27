/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 21/08/2019
 * Time: 11:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace captainalm.integrator
{
	/// <summary>
	/// Integrator XML Format Loader and Saver.
	/// </summary>
	public class XMLLoaderSaver : ISaver, ILoader
	{
		protected String _data = "";
		/// <summary>
		/// Creates a new instance of XMLLoaderSaver with no data.
		/// </summary>
		/// <remarks></remarks>
		public XMLLoaderSaver() {}
		/// <summary>
		/// Creates a new instance of XMLLoaderSaver with the specified data.
		/// </summary>
		/// <param name="dataIn">The data of an Integrator</param
		/// <remarks></remarks>
		public XMLLoaderSaver(String dataIn)
		{
			if (! object.ReferenceEquals(null, dataIn)) {
				_data = dataIn;
			} else {
				throw new ArgumentNullException("dataIn");
			}
		}
		
		#region ISaver implementation
		/// <summary>
		/// Saves the integrator.
		/// </summary>
		/// <param name="toSave">The integrator to save</param>
		/// <remarks></remarks>
		public virtual void save(Integrator toSave)
		{
			if (! object.ReferenceEquals(null, toSave)) {
				var ndat = "";
				var xf = new XmlSerializer(typeof(XMLIntegration));
				var ms = new MemoryStream();
				var sr = new StreamReader(ms);
				var sw = new StreamWriter(ms);
				try {
					var xtodo = new XMLIntegration(toSave);
					xf.Serialize(sw, xtodo);
					ms.Position = 0;
					ndat = sr.ReadToEnd();
				} catch (ObjectDisposedException e) {
					ndat = "";
					sr.Dispose();
					sr = null;
					sw.Dispose();
					sw = null;
					ms.Dispose();
					ms  = null;
					xf = null;
					throw e;
				} catch (IOException e) {
					ndat = "";
					sr.Dispose();
					sr = null;
					sw.Dispose();
					sw = null;
					ms.Dispose();
					ms  = null;
					xf = null;
					throw e;
				} catch (OutOfMemoryException e) {
					ndat = "";
					sr.Dispose();
					sr = null;
					sw.Dispose();
					sw = null;
					ms.Dispose();
					ms  = null;
					xf = null;
					throw e;
				} finally {
					if (! object.ReferenceEquals(null, sr)) {sr.Dispose();sr=null;}
					if (! object.ReferenceEquals(null, sw)) {sw.Dispose();sw=null;}
					if (! object.ReferenceEquals(null, ms)) {ms.Dispose();ms=null;}
					xf = null;
				}
				_data = ndat;
			} else {
				throw new ArgumentNullException("toSave");
			}
		}

		#endregion
		
		#region ILoader implementation
		/// <summary>
		/// Loads the integrator.
		/// </summary>
		/// <returns>Integrator</returns>
		/// <remarks></remarks>
		public virtual Integrator load()
		{
			if (object.ReferenceEquals(null, _data)) {throw new NullReferenceException("_data");}
			Integrator toret = null;
			var xf = new XmlSerializer(typeof(XMLIntegration));
			var ms = new MemoryStream();
			var sw = new StreamWriter(ms);
			try {
				XMLIntegration xtoret = null;
				sw.AutoFlush = true;
				for (int i = 0; i < _data.Length; i++) {
					sw.Write(_data[i]);
				}
				if (! sw.AutoFlush) {sw.Flush();}
				sw.AutoFlush = false;
				ms.Position = 0;
				xtoret = xf.Deserialize(ms) as XMLIntegration;
				toret = !object.ReferenceEquals(null, xtoret) ? xtoret.toIntegrator() : null;
			} catch (System.Text.EncoderFallbackException e) {
				toret = null;
				sw.Dispose();
				sw = null;
				ms.Dispose();
				ms  = null;
				xf = null;
				throw e;
			} catch (ObjectDisposedException e) {
				toret = null;
				sw.Dispose();
				sw = null;
				ms.Dispose();
				ms  = null;
				xf = null;
				throw e;
			} catch (IOException e) {
				toret = null;
				sw.Dispose();
				sw = null;
				ms.Dispose();
				ms  = null;
				xf = null;
				throw e;
			} catch (NotSupportedException e) {
				toret = null;
				sw.Dispose();
				sw = null;
				ms.Dispose();
				ms  = null;
				xf = null;
				throw e;
			} finally {
				if (! object.ReferenceEquals(null, sw)) {sw.Dispose();sw=null;}
				if (! object.ReferenceEquals(null, ms)) {ms.Dispose();ms=null;}
				xf = null;
			}
			return toret;
		}
		
		#endregion
		
		[XmlRootAttribute("Integration", Namespace="captainalm.integrator", IsNullable = false)]
		public sealed class XMLIntegration {
			[XmlArray("rows")]
			public XRow[] rows;
			public String types;
			[XmlAttribute("rowCount")]
			public Int32 rowCount;
			[XmlAttribute("blockCount")]
			public Int32 blockCount;
			[XmlAttribute("indexCount")]
			public Int32 indexCount;
			public XMLIntegration() {}
			public XMLIntegration(Integrator iIn) {
				rowCount = iIn.rowCount;
				blockCount = iIn.blockCount;
				indexCount = iIn.blockTypes.Length;
				types = System.Convert.ToBase64String(Serializer.serialize(iIn.blockTypes));
				rows = new XRow[iIn.rowCount];
				for (int i = 0; i < iIn.rowCount; i++) {
					var tr = new List<List<IElement>>();
					for (int j = 0; j < iIn.blockCount; j++) {
						tr.Add(new List<IElement>(iIn.get_block(j, i)));
					}
					rows[i] = new XRow(tr);
				}
			}
			
			public Integrator toIntegrator() {
				var tps = Serializer.deSerialize(System.Convert.FromBase64String(types)) as Type[];
				var toret = new Integrator(tps, blockCount, rowCount);
				var lstm = new List<List<List<IElement>>>();
				for (int i = 0; i < rows.Length; i++) {
					lstm.Add(rows[i].toRow(tps));
				}
				for (int i = 0; i < rowCount; i++) {
					for (int j = 0; j < blockCount; j++) {
						for (int k = 0; k < tps.Length; k++) {
							toret.set_element(j,i,k,lstm[i][j][k]);
						}
					}
				}
				return toret;
			}
			[XmlTypeAttribute("Row", Namespace="captainalm.integrator")]
			public sealed class XRow {
				[XmlArray("blocks")]
				public XBlock[] blocks;
				
				public XRow() {}
				public XRow(List<List<IElement>> lleIn) {
					blocks = new XBlock[lleIn.Count];
					for (int i = 0; i < lleIn.Count; i++) {
						blocks[i] = new XBlock(lleIn[i]);
					}
				}
				
				public List<List<IElement>> toRow(Type[] eTypesIn) {
					var toret = new List<List<IElement>>();
					for (int i = 0; i < blocks.Length; i++) {
						toret.Add(blocks[i].toBlock(eTypesIn));
					}
					return toret;
				}
				
				[XmlTypeAttribute("Block", Namespace="captainalm.integrator")]
				public sealed class XBlock {
					[XmlArray("elements")]
					public XElement[] elements;
					
					public XBlock() {}
					public XBlock(List<IElement> leIn) {
						elements = new XElement[leIn.Count];
						for (int i = 0; i < leIn.Count; i++) {
							elements[i] = new XElement(leIn[i]);
						}
					}
					
					public List<IElement> toBlock(Type[] eTypesIn) {
						var toret = new List<IElement>();
						for (int i = 0; i < elements.Length; i++) {
							toret.Add(elements[i].toElement(eTypesIn[i]));
						}
						return toret;
					}
					
					[XmlTypeAttribute("Element", Namespace="captainalm.integrator")]
					public sealed class XElement {
						public string data;
						
						public XElement() {}
						public XElement(IElement eIn) {
							String dat = "";
							if (! object.ReferenceEquals(null, eIn)) {
								var cons = eIn.GetType().GetConstructor(Type.EmptyTypes);
								if (object.ReferenceEquals(null, cons)) {
									dat = System.Convert.ToBase64String(Serializer.serialize(eIn));
								} else {
									if (eIn.CanBeConvertedToFromString) {
										dat = eIn.getElementAsString();
									} else if (eIn.CanBeConvertedToFromByteArray) {
										dat = System.Convert.ToBase64String(eIn.getElementAsByteArray());
									} else {
										dat = System.Convert.ToBase64String(Serializer.serialize(eIn));
									}
								}
							} else {
								dat = System.Convert.ToBase64String(Serializer.serialize(new NullElement()));
							}
							data = dat;
						}
						
						public IElement toElement(Type eTypeIn) {
							var cons = eTypeIn.GetConstructor(Type.EmptyTypes);
							IElement element = null;
							if (! object.ReferenceEquals(null, cons)) {
								element = cons.Invoke(null) as IElement;
								if (element.CanBeConvertedToFromString) {
									element.setElementAsString(data);
								} else if (element.CanBeConvertedToFromByteArray) {
									element.setElementAsByteArray(System.Convert.FromBase64String(data));
								} else {
									element = Serializer.deSerialize(System.Convert.FromBase64String(data)) as IElement;
								}
							} else {
								element = Serializer.deSerialize(System.Convert.FromBase64String(data)) as IElement;
							}
							if (element.GetType().Equals(typeof(NullElement))) {
								element = null;
							}
							return element;
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the integrator data held.
		/// </summary>
		/// <value>The integrator data</value>
		/// <returns>String</returns>
		/// <remarks></remarks>
		public virtual String data {
			get {
				return _data;
			} set {
				if (! object.ReferenceEquals(null, value)) {
					_data = value;
				} else {
					throw new ArgumentNullException("value");
				}
			}
		}
		
		public sealed class NullElement : IElement {
			public NullElement() {}

			#region IElement implementation

			public void setElementAsString(string elementData){}

			public string getElementAsString(){return null;}

			public void setElementAsByteArray(byte[] elementData){}

			public byte[] getElementAsByteArray(){return null;}

			public object HeldElement {
				get {
					return null;
				} set {}
			}

			public bool CanBeConvertedToFromString {
				get {
					return false;
				}
			}

			public bool CanBeConvertedToFromByteArray {
				get {
					return false;
				}
			}

			public Type HeldElementType {
				get {
					return typeof(void);
				}
			}

			#endregion
		}
	}
}
