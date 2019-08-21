/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 21/08/2019
 * Time: 11:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace captainalm.integrator
{
	/// <summary>
	/// Integrator XML Format Loader and Saver.
	/// </summary>
	public class XMLLoaderSaver : ISaver, ILoader
	{
		protected String _data = "";
		/// <summary>
		/// Creates a new instance of XMLLoaderSaver with no data.
		/// </summary>
		/// <remarks></remarks>
		public XMLLoaderSaver() {}
		/// <summary>
		/// Creates a new instance of XMLLoaderSaver with the specified data.
		/// </summary>
		/// <param name="dataIn">The data of an Integrator</param
		/// <remarks></remarks>
		public XMLLoaderSaver(String dataIn)
		{
			if (! object.ReferenceEquals(null, dataIn)) {
				_data = dataIn;
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}
		
		#endregion
		
		[XmlRootAttribute("Integration", Namespace="http://www.cpandl.com", IsNullable = false)]
		protected sealed class XMLIntegration {
			[XmlArray("rows")]
			public XRow[] rows;
			[XmlArray("types")]
			public String[] types;
			[XmlAttribute("rowCount")]
			public Int32 rowCount;
			[XmlAttribute("blockCount")]
			public Int32 blockCount;
			[XmlAttribute("indexCount")]
			public Int32 indexCount;
			public XMLIntegration() {}
			public XMLIntegration(Integrator iIn) {
				
			}
			
			public Integrator toIntegrator() {
				
			}
			
			public sealed class XRow {
				[XmlArray("blocks")]
				public XBlock[] blocks;
				
				public XRow() {}
				public XRow(List<List<IElement>> lleIn) {
					
				}
				
				public List<List<IElement>> toRow() {
					
				}
				
				public sealed class XBlock {
					[XmlArray("elements")]
					public XElement[] elements;
					
					public XBlock() {}
					public XBlock(List<IElement> leIn) {
						
					}
					
					public List<IElement> toBlock() {
						
					}
					
					public sealed class XElement {
						public string data;
						
						public XElement() {}
						public XElement(IElement eIn) {
							
						}
						
						public IElement toElement() {
							
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the integrator data held.
		/// </summary>
		/// <value>The integrator data</value>
		/// <returns>String</returns>
		/// <remarks></remarks>
		public virtual String data {
			get {
				return _data;
			} set {
				if (! object.ReferenceEquals(null, value)) {
					_data = value;
				} else {
					throw new ArgumentNullException("value");
				}
			}
		}
	}
}
