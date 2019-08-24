/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 23/08/2019
 * Time: 08:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace captainalm.integrator.verifier
{
	/// <summary>
	/// FSOTypeElement.
	/// </summary>
	public class FSOTypeElement : Element
	{
		public FSOTypeElement() : base(FSOType.None) {}
		public FSOTypeElement(FSOType dataIn) : base(dataIn) {}

		#region implemented abstract members of Element

		public override void setElementAsString(string elementData)
		{
			_held = (FSOType) Enum.Parse(typeof(FSOType),elementData);
		}

		public override string getElementAsString()
		{
			return ((FSOType)_held).ToString();
		}

		public override void setElementAsByteArray(byte[] elementData)
		{
			_held = (FSOType) Enum.Parse(typeof(FSOType),Serializer.deSerialize(elementData) as String);
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
	
	public enum FSOType {
		None = 0,
		File = 1,
		Directory = 2,
		RootDirectory = 3,
		RootFile = 4
	}
}
