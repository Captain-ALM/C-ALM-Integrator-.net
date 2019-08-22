/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 22/08/2019
 * Time: 15:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using captainalm.integrator;

namespace captainalm.integrator.verifier
{
	[Serializable]
	/// <summary>
	/// StringElement.
	/// </summary>
	public class StringElement : Element
	{
		public StringElement() : base("") {}
		public StringElement(String dataIn) : base(dataIn) {}

		#region implemented abstract members of Element

		public override void setElementAsString(string elementData)
		{
			_held = elementData;
		}

		public override string getElementAsString()
		{
			return _held as string;
		}

		public override void setElementAsByteArray(byte[] elementData)
		{
			_held = Serializer.deSerialize(elementData) as String;
		}

		public override byte[] getElementAsByteArray()
		{
			return Serializer.serialize(_held);
		}

		public override bool CanBeConvertedToFromString {
			get {
				return true;
			}
		}

		public override bool CanBeConvertedToFromByteArray {
			get {
				return true;
			}
		}

		#endregion
	}
}
