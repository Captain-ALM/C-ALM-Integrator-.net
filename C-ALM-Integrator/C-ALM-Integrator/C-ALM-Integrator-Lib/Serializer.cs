/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 20/08/2019
 * Time: 20:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;

namespace captainalm.integrator
{
	/// <summary>
	/// The Binary Serializer Class.
	/// </summary>
	public static class Serializer
	{
		private static Object slockop = new Object();
		/// <summary>
		/// Serializes an Object to a Byte Array.
		/// </summary>
		/// <param name="obj">The object to serialize</param>
		/// <returns>Byte Array</returns>
		/// <remarks></remarks>
		public static Byte[] serialize(Object obj) {return serialize(new BinaryFormatter(),obj);}
		/// <summary>
		/// Deserializes an Object from a Byte Array.
		/// </summary>
		/// <param name="bts">The byte array to deserialize</param>
		/// <returns>Object</returns>
		/// <remarks></remarks>
		public static Object deSerialize(Byte[] bts) {return deSerialize(new BinaryFormatter(), bts);}
		/// <summary>
		/// Serializes an Object to a Byte Array.
		/// </summary>
		/// <param name="bf">The binary formatter</param>
		/// <param name="obj">The object to serialize</param>
		/// <returns>Byte Array</returns>
		/// <remarks></remarks>
		public static Byte[] serialize(BinaryFormatter bf, Object obj) {
			if (object.ReferenceEquals(null, bf)) {throw new ArgumentNullException("bf");}
			if (object.ReferenceEquals(null, obj)) {throw new ArgumentNullException("obj");}
			Byte[] toret = null;
			lock (slockop) {
				var ms = new MemoryStream();
				try {
					bf.Serialize(ms,obj);
					toret = ms.ToArray();
				} catch (ArgumentException e) {
					toret = null;
					ms.Dispose();
					ms  = null;
					throw e;
				} catch (SerializationException e) {
					toret = null;
					ms.Dispose();
					ms  = null;
					throw e;
				} catch (SecurityException e) {
					toret = null;
					ms.Dispose();
					ms  = null;
					throw e;
				} finally {
					if (! object.ReferenceEquals(null, ms)) {ms.Dispose();ms=null;}
				}
			}
			return toret;
		}
		/// <summary>
		/// Deserializes an Object from a Byte Array.
		/// </summary>
		/// <param name="bf">The binary formatter</param>
		/// <param name="bts">The byte array to deserialize</param>
		/// <returns>Object</returns>
		/// <remarks></remarks>
		public static Object deSerialize(BinaryFormatter bf, Byte[] bts) {
			if (object.ReferenceEquals(null, bf)) {throw new ArgumentNullException("bf");}
			if (object.ReferenceEquals(null, bts)) {throw new ArgumentNullException("bts");}
			if (bts.Length < 1) {throw new ArgumentException("bts has no data");}
			Object toret = null;
			lock (slockop) {
				var ms = new MemoryStream(bts);
				try {
					toret = bf.Deserialize(ms);
				} catch (ArgumentException e) {
					toret = null;
					ms.Dispose();
					ms  = null;
					throw e;
				} catch (SerializationException e) {
					toret = null;
					ms.Dispose();
					ms  = null;
					throw e;
				} catch (SecurityException e) {
					toret = null;
					ms.Dispose();
					ms  = null;
					throw e;
				} finally {
					if (! object.ReferenceEquals(null, ms)) {ms.Dispose();ms=null;}
				}
			}
			return toret;
		}
	}
}
