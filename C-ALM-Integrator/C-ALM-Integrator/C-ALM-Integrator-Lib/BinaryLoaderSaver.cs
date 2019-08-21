/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 21/08/2019
 * Time: 09:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace captainalm.integrator
{
	/// <summary>
	/// Integrator Binary Format Loader and Saver.
	/// </summary>
	public class BinaryLoaderSaver : ISaver, ILoader
	{
		protected Byte[] _data = null;
		/// <summary>
		/// Creates a new instance of BinaryLoaderSaver with no data.
		/// </summary>
		/// <remarks></remarks>
		public BinaryLoaderSaver() {}
		/// <summary>
		/// Creates a new instance of BinaryLoaderSaver with the specified data.
		/// </summary>
		/// <param name="dataIn">The data of an Integrator</param
		/// <remarks></remarks>
		public BinaryLoaderSaver(Byte[] dataIn)
		{
			if (! object.ReferenceEquals(null, dataIn)) {
				if (dataIn.Length > 0) {
					_data = dataIn;
				} else {
					throw new ArgumentException("dataIn has no data");
				}
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
				var tps = toSave.blockTypes;
				var lstts = new List<List<List<Byte[]>>>();
				for (int i = 0; i < toSave.rowCount - 1; i++) {
					var lsti = new List<List<Byte[]>>();
					for (int j = 0; j < toSave.blockCount - 1; j++) {
						var lstj = new List<Byte[]>();
						for (int k = 0; k < toSave.blockTypes.Length - 1; k++) {
							Byte[] btsk = null;
							var ce = toSave.get_element(j, i, k);
							if (! object.ReferenceEquals(null, ce)) {
								var cons = ce.GetType().GetConstructor(Type.EmptyTypes);
								if (object.ReferenceEquals(null, cons)) {
									btsk = Serializer.serialize(ce);
								} else {
									if (ce.CanBeConvertedToFromByteArray) {
										btsk = ce.getElementAsByteArray();
									} else {
										btsk = Serializer.serialize(ce);
									}
								}
							} else {
								btsk = Serializer.serialize(new NullElement());
							}
							lstj.Add(btsk);
						}
						lsti.Add(lstj);
					}
					lstts.Add(lsti);
				}
				var ndat = new Tuple<Type[], List<List<List<Byte[]>>>>(tps,lstts);
				_data = Serializer.serialize(ndat);
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
			var dst = Serializer.deSerialize(_data) as Tuple<Type[], List<List<List<Byte[]>>>>;
			if (! object.ReferenceEquals(null, dst)) {
				var rows = dst.Item2.Count;
				var blocks = 0;
				if (rows < 1) {blocks = 0;} else {blocks = dst.Item2[0].Count;}
				var blockSize = dst.Item1.Length;
				var toret = new Integrator(dst.Item1, blocks,rows);
				for (int i = 0; i < blocks - 1; i++) {
					for (int j = 0; j < rows - 1; j++) {
						for (int k = 0; k < blockSize - 1; k++) {
							var ctype = dst.Item1[k];
							var cons = ctype.GetConstructor(Type.EmptyTypes);
							IElement element = null;
							if (! object.ReferenceEquals(null, cons)) {
								element = cons.Invoke(null) as IElement;
								if (element.CanBeConvertedToFromByteArray) {
									element.setElementAsByteArray(dst.Item2[j][i][k]);
								} else {
									element = Serializer.deSerialize(dst.Item2[j][i][k]) as IElement;
								}
							} else {
								element = Serializer.deSerialize(dst.Item2[j][i][k]) as IElement;
							}
							if (element.GetType().Equals(typeof(NullElement))) {
								toret.set_element(i,j,k,null);
							} else {
								toret.set_element(i,j,k,element);
							}
						}
					}
				}
				return toret;
			} else {
				return null;
			}
		}

		#endregion
		/// <summary>
		/// Gets or sets the integrator data held.
		/// </summary>
		/// <value>The integrator data</value>
		/// <returns>Byte Array</returns>
		/// <remarks></remarks>
		public virtual Byte[] data {
			get {
				return _data;
			} set {
				if (! object.ReferenceEquals(null, value)) {
					if (value.Length > 0) {
						_data = value;
					} else {
						throw new ArgumentException("value has no data");
					}
				} else {
					throw new ArgumentNullException("value");
				}
			}
		}
		
		protected sealed class NullElement : IElement {
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
