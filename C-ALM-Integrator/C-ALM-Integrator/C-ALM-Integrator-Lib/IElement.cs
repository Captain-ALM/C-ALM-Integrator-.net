/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 19/08/2019
 * Time: 09:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.Serialization;

namespace captainalm.integrator
{
	/// <summary>
	/// Provides a interface for integratable elements.
	/// Make sure to implement the serializable attribute.
	/// </summary>
	public interface IElement
	{
		/// <summary>
		/// Gets or sets the held element.
		/// </summary>
		/// <value>The Object Contained</value>
		///<returns>Object</returns>
		/// <remarks></remarks>
		Object HeldElement { get; set; }
		/// <summary>
		/// Gets whether the element can be converted to and from a string.
		/// </summary>
		/// <value>Whether the element can be converted to and from a string</value>
		///<returns>Boolean</returns>
		/// <remarks></remarks>
		Boolean CanBeConvertedToFromString { get; }
		/// <summary>
		/// Gets whether the element can be converted to and from a byte array.
		/// </summary>
		/// <value>Whether the element can be converted to and from a byte array</value>
		///<returns>Boolean</returns>
		/// <remarks></remarks>
		Boolean CanBeConvertedToFromByteArray { get; }
		/// <summary>
		/// Sets the element data from the string provided.
		/// </summary>
		/// <param name="elementData">The element data in string form</param>
		/// <remarks></remarks>
		void setElementAsString(String elementData);
		/// <summary>
		/// Gets the string representation of the element data.
		/// </summary>
		/// <returns>String</returns>
		/// <remarks></remarks>
		String getElementAsString();
		/// <summary>
		/// Sets the element data from the byte array provided.
		/// </summary>
		/// <param name="elementData">The element data in byte array form</param>
		/// <remarks></remarks>
		void setElementAsByteArray(Byte[] elementData);
		/// <summary>
		/// Gets the byte array representation of the element data.
		/// </summary>
		/// <returns>Byte Array</returns>
		/// <remarks></remarks>
		Byte[] getElementAsByteArray();
		/// <summary>
		/// Gets the type of the held element.
		/// </summary>
		/// <value>The type of the held element</value>
		///<returns>Type</returns>
		/// <remarks></remarks>
		Type HeldElementType { get; }
	}
}
