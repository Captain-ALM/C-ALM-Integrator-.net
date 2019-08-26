/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 20/08/2019
 * Time: 19:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace captainalm.integrator
{
	[Serializable]
	/// <summary>
	/// The Element Base Class.
	/// </summary>
	public abstract class Element : IElement
	{
		protected Object _held = null;
		/// <summary>
		/// Instantiates a new element base class.
		/// </summary>
		/// <param name="toHold">The object to hold (Must have the SerializableAttribute).</param>
		/// <remarks></remarks>
		public Element(Object toHold)
		{
			if (! object.ReferenceEquals(null, toHold)) {
				Type tht = toHold.GetType();
				var atrs = tht.GetCustomAttributes(typeof(SerializableAttribute), false) as SerializableAttribute[];
				if (!object.ReferenceEquals(null, atrs)) {
					if (atrs.Length > 0) {
						_held = toHold;
					} else {
						throw new ArgumentException("toHold does not have an object with the SerializableAttribute");
					}
				} else {
					throw new ArgumentException("toHold does not have an object with the SerializableAttribute");
				}
			} else {
				throw new ArgumentNullException("toHold");
			}
		}

		#region IElement implementation
		/// <summary>
		/// Sets the element data from the string provided.
		/// </summary>
		/// <param name="elementData">The element data in string form</param>
		/// <remarks></remarks>
		public abstract void setElementAsString(string elementData);
		/// <summary>
		/// Gets the string representation of the element data.
		/// </summary>
		/// <returns>String</returns>
		/// <remarks></remarks>
		public abstract string getElementAsString();
		/// <summary>
		/// Sets the element data from the byte array provided.
		/// </summary>
		/// <param name="elementData">The element data in byte array form</param>
		/// <remarks></remarks>
		public abstract void setElementAsByteArray(byte[] elementData);
		/// <summary>
		/// Gets the byte array representation of the element data.
		/// </summary>
		/// <returns>Byte Array</returns>
		/// <remarks></remarks>
		public abstract byte[] getElementAsByteArray();
		/// <summary>
		/// Gets or sets the held element.
		/// </summary>
		/// <value>The Object Contained</value>
		///<returns>Object</returns>
		/// <remarks></remarks>
		public virtual object HeldElement {
			get {
				return _held;
			}
			set {
				if (! object.ReferenceEquals(null, value)) {
					_held = value;
				} else {
					throw new ArgumentNullException("value");
				}
			}
		}
		/// <summary>
		/// Gets whether the element can be converted to and from a string.
		/// </summary>
		/// <value>Whether the element can be converted to and from a string</value>
		///<returns>Boolean</returns>
		/// <remarks></remarks>
		public abstract bool CanBeConvertedToFromString {get;}
		/// <summary>
		/// Gets whether the element can be converted to and from a byte array.
		/// </summary>
		/// <value>Whether the element can be converted to and from a byte array</value>
		///<returns>Boolean</returns>
		/// <remarks></remarks>
		public abstract bool CanBeConvertedToFromByteArray {get;}
		/// <summary>
		/// Gets the type of the held element.
		/// </summary>
		/// <value>The type of the held element</value>
		///<returns>Type</returns>
		/// <remarks></remarks>
		public virtual Type HeldElementType {
			get {
				return _held.GetType();
			}
		}

		#endregion
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			Element other = obj as Element;
				if (other == null)
					return false;
						return object.Equals(this._held, other._held);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(Element lhs, Element rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Element lhs, Element rhs) {
			return !(lhs == rhs);
		}

		#endregion
	}
}
