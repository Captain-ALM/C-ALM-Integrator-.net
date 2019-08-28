/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 22/08/2019
 * Time: 15:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using captainalm.integrator;

namespace captainalm.integrator.syncer
{
	[Serializable]
	/// <summary>
	/// LongElement.
	/// </summary>
	public class LongElement : Element
	{
		public LongElement() : base(0) {}
		public LongElement(Int64 dataIn) : base(dataIn) {}

		#region implemented abstract members of Element

		public override void setElementAsString(string elementData)
		{
			_held = Int64.Parse(elementData);
		}

		public override string getElementAsString()
		{
			return ((Int64)_held).ToString();
		}

		public override void setElementAsByteArray(byte[] elementData)
		{
			_held = Int64.Parse(Serializer.deSerialize(elementData) as String);
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
