/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 19/08/2019
 * Time: 10:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace captainalm.integrator
{
	[Serializable]
	/// <summary>
	/// The Sealed Integrator Class for integration processing.
	/// </summary>
	public sealed class Integrator : IDeserializationCallback
	{
		[NonSerialized]
		static Object slockop = new object();
		List<List<List<IElement>>> data = new List<List<List<IElement>>>();
		List<Type> blocktypes = new List<Type>();
		Int32 _rows = 0;
		Int32 _blocks = 0;
		/// <summary>
		/// Creates a new integrator class.
		/// </summary>
		/// <param name="types">The List of types in each block (Should all be IElement and be serializable)</param>
		/// <remarks></remarks>
		public Integrator(Type[] types)
		{
			Integrator(types,0,0);
		}
		/// <summary>
		/// Creates a new integrator class.
		/// </summary>
		/// <param name="types">The List of types in each block (Should all be IElement and be serializable)</param>
		/// <param name="blocks">The number of initial blocks</param>
		/// <param name="rows">The number of initial rows</param>
		/// <remarks></remarks>
		public Integrator(Type[] types,Int32 blocks, Int32 rows) {
			if (! object.ReferenceEquals(null, types)) {
				if (types.Length > 0) {
					for (int i = 0; i < types.Length; i++) {
						Type c = types[i];
						Boolean chk = false;
						Type[] ints = c.GetInterfaces();
						var intslst = new List<Type>(ints);
						chk = intslst.Contains(typeof(IElement));
						SerializableAttribute[] atrs = c.GetCustomAttributes(typeof(SetupMethodAttribute), false) as SerializableAttribute[];
						if (!object.ReferenceEquals(null, atrs)) {
							chk = chk && (atrs.Length > 0);
						} else {
							chk = false;
						}
						if (!chk) {
							throw new ArgumentException("The type at index " + i + " in the types parameter does not implement IElement or have the SerializableAttribute");
						}
					}
				}
			} else {
				throw new ArgumentNullException("types");
			}
			if (blocks < 0) {throw new ArgumentOutOfRangeException("blocks");}
			_blocks = blocks;
			if (rows < 0) {throw new ArgumentOutOfRangeException("rows");}
			_rows = rows;
		}

		#region IDeserializationCallback implementation
		/// <summary>
		/// Deserialization callback.
		/// </summary>
		/// <param name="sender">The caller</param>
		public void OnDeserialization(object sender)
		{
			slockop = new Object();
		}

		#endregion
	}
}
