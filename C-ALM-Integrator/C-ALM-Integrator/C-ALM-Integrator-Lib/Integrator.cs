﻿/*
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
	public sealed class Integrator : IDeserializationCallback, IDisposable
	{
		[NonSerialized]
		static Object slockop = new object();
		List<List<List<IElement>>> data = new List<List<List<IElement>>>();
		Type[] _blocktypes = null;
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
						var atrs = c.GetCustomAttributes(typeof(SerializableAttribute), false) as SerializableAttribute[];
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
			_blocktypes = types;
			if (blocks < 0) {throw new ArgumentOutOfRangeException("blocks");}
			_blocks = blocks;
			if (rows < 0) {throw new ArgumentOutOfRangeException("rows");}
			_rows = rows;
			for (int j = 1; j < _rows; j++) {
				var lstr = new List<List<IElement>>();
				for (int k = 1; k < _blocks; k++) {
					var lstb = new List<IElement>();
					for (int l = 1; l < _blocktypes.Length - 1; l++) {
						lstb.Add(null);
					}
					lstr.Add(lstb);
				}
				data.Add(lstr);
			}
		}

		#region IDisposable implementation
		/// <summary>
		/// Dispose of resources.
		/// </summary>
		/// <remarks></remarks>
		public void Dispose()
		{
			if (! object.ReferenceEquals(null, slockop)) {
				lock(slockop) {
					_blocks = 0;
					_rows = 0;
					if (! object.ReferenceEquals(null, _blocktypes)) {
						_blocktypes = null;
					}
					if (! object.ReferenceEquals(null, data)) {
						data.Clear();
						data = null;
					}
				}
				slockop = null;
			}
		}
		#endregion
		#region IDeserializationCallback implementation
		/// <summary>
		/// Deserialization callback.
		/// </summary>
		/// <param name="sender">The caller</param>
		/// <remarks></remarks>
		public void OnDeserialization(object sender)
		{
			slockop = new Object();
		}

		#endregion
		/// <summary>
		/// Returns the types used per block.
		/// </summary>
		/// <value>The block types used</value>
		/// <returns>Type Array</returns>
		/// <remarks></remarks>
		public Type[] blockTypes {
			get {
				return _blocktypes;
			}
		}
		/// <summary>
		/// Returns the number of integration rows.
		/// </summary>
		/// <value>The number of integration rows</value>
		/// <returns>Int32</returns>
		/// <remarks></remarks>
		public Int32 rowCount {
			get {
				return _rows;
			}
		}
		/// <summary>
		/// Returns the number of integration blocks.
		/// </summary>
		/// <value>The number of integration blocks</value>
		/// <returns>Int32</returns>
		/// <remarks></remarks>
		public Int32 blockCount {
			get {
				return _blocks;
			}
		}
		/// <summary>
		/// Returns the block data at the specified block and row.
		/// </summary>
		/// <param name="Block">The Index of the block</param>
		/// <param name="Row">The Index of the row</param>
		/// <returns>IElement Array</returns>
		/// <remarks>I'm so sorry but c# does not support parameterized properties.</remarks>
		public IElement[] get_block(Int32 Block, Int32 Row) {
			IElement[] toret = null;
			if (Block < 0 || Block > _blocks - 1) {throw new ArgumentOutOfRangeException("Block");}
			if (Row < 0 || Row > _rows - 1) {throw new ArgumentOutOfRangeException("Row");}
			lock (slockop) {
				toret = data[Row][Block].ToArray();
			}
			return toret;
		}
		/// <summary>
		/// Sets the block data at the specified block and row.
		/// </summary>
		/// <param name="Block">The Index of the block</param>
		/// <param name="Row">The Index of the row</param>
		/// <param name="value">The new IElement array value</param>
		/// <remarks>I'm so sorry but c# does not support parameterized properties.</remarks>
		public void set_block(Int32 Block, Int32 Row, IElement[] value) {
			if (Block < 0 || Block > _blocks - 1) {throw new ArgumentOutOfRangeException("Block");}
			if (Row < 0 || Row > _rows - 1) {throw new ArgumentOutOfRangeException("Row");}
			lock (slockop) {
				data[Row][Block] = new List<IElement>(value);
			}
		}
		/// <summary>
		/// Returns the element data at the specified block, row and index.
		/// </summary>
		/// <param name="Block">The Index of the block</param>
		/// <param name="Row">The Index of the row</param>
		/// <param name="Index">The Index of the element in the block</param>
		/// <returns>IElement</returns>
		/// <remarks>I'm so sorry but c# does not support parameterized properties.</remarks>
		public IElement get_element(Int32 Block, Int32 Row, Int32 Index) {
			IElement toret = null;
			if (Block < 0 || Block > _blocks - 1) {throw new ArgumentOutOfRangeException("Block");}
			if (Row < 0 || Row > _rows - 1) {throw new ArgumentOutOfRangeException("Row");}
			if (Index < 0 || Index > _blocktypes.Length - 1) {throw new ArgumentOutOfRangeException("Index");}
			lock (slockop) {
				toret = data[Row][Block][Index];
			}
			return toret;
		}
		/// <summary>
		/// Sets the block data at the specified block, row and index.
		/// </summary>
		/// <param name="Block">The Index of the block</param>
		/// <param name="Row">The Index of the row</param>
		/// <param name="Index">The Index of the element in the block</param>
		/// <param name="value">The new IElement array value</param>
		/// <remarks>I'm so sorry but c# does not support parameterized properties.</remarks>
		public void set_element(Int32 Block, Int32 Row, Int32 Index, IElement value) {
			if (Block < 0 || Block > _blocks - 1) {throw new ArgumentOutOfRangeException("Block");}
			if (Row < 0 || Row > _rows - 1) {throw new ArgumentOutOfRangeException("Row");}
			if (Index < 0 || Index > _blocktypes.Length - 1) {throw new ArgumentOutOfRangeException("Index");}
			lock (slockop) {
				data[Row][Block][Index] = value;
			}
		}
		/// <summary>
		/// Gets or sets the element at the specified block, row and index.
		/// <param name="Block">The Index of the block</param>
		/// <param name="Row">The Index of the row</param>
		/// <param name="Index">The Index of the element in the block</param>
		/// <value>The Element</value>
		/// <returns>IElement</returns>
		/// <remarks></remarks>
		/// </summary>
		public IElement this[Int32 Block, Int32 Row, Int32 Index] {
			get {
				IElement toret = null;
				if (Block < 0 || Block > _blocks - 1) {throw new ArgumentOutOfRangeException("Block");}
				if (Row < 0 || Row > _rows - 1) {throw new ArgumentOutOfRangeException("Row");}
				if (Index < 0 || Index > _blocktypes.Length - 1) {throw new ArgumentOutOfRangeException("Index");}
				lock (slockop) {
					toret = data[Row][Block][Index];
				}
				return toret;
			}
			set {
				if (Block < 0 || Block > _blocks - 1) {throw new ArgumentOutOfRangeException("Block");}
				if (Row < 0 || Row > _rows - 1) {throw new ArgumentOutOfRangeException("Row");}
				if (Index < 0 || Index > _blocktypes.Length - 1) {throw new ArgumentOutOfRangeException("Index");}
				lock (slockop) {
					data[Row][Block][Index] = value;
				}
			}
		}
	}
}
