/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 22/08/2019
 * Time: 16:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace captainalm.integrator.verifier
{
	[Serializable]
	/// <summary>
	/// DateTimeElement.
	/// </summary>
	public class DateTimeElement : Element
	{
		public DateTimeElement() : base(DateTime.MinValue) {}
		public DateTimeElement(DateTime dataIn) : base(dataIn) {}

		#region implemented abstract members of Element

		public override void setElementAsString(string elementData)
		{
			_held = DateTime.Parse(elementData);
		}

		public override string getElementAsString()
		{
			return (_held as DateTime).ToString();
		}

		public override void setElementAsByteArray(byte[] elementData)
		{
			_held = DateTime.Parse(Serializer.deSerialize(elementData) as String);
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
