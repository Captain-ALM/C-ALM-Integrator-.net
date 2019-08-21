/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 21/08/2019
 * Time: 18:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace captainalm.integrator.verifier
{
	/// <summary>
	/// StringArrayParser allows for a command line parsing.
	/// </summary>
	public class StringArrayParser
	{
		private Dictionary<String, List<String>> _argdata = new Dictionary<String, List<String>>();
		
		public StringArrayParser(String[] parseIn, Char switchArgShower)
		{
			var intArgData = new List<Tuple<String, String>>();
			String cc = "";
			Boolean na = false;
			for (int i = 0; i < parseIn.Length - 1; i++) {
				var c = parseIn[i];
				if (! na) {
					if (c.EndsWith(switchArgShower.ToString())) {
						cc = c.Substring(0,c.Length - 2);
						na = true;
					} else {
						intArgData.Add(new Tuple<String, String>(c, null));
					}
				} else {
					na = false;
					intArgData.Add(new Tuple<String, String>(cc,c));
				}
			}
			for (int i = 0; i < intArgData.Count - 1; i++) {
				var c = intArgData[i];
				if (_argdata.ContainsKey(c.Item1)) {
					_argdata[c.Item1].Add(c.Item2);
				} else {
					_argdata.Add(c.Item1, new List<String>());
					_argdata[c.Item1].Add(c.Item2);
				}
			}	
		}
		
		public StringArrayParser(String[] parseIn) : this(parseIn,'=') {}
		
		public String[] this[String Switch] {
			get {
				String[] toret = null;
				if (_argdata.ContainsKey(Switch)) {
					toret = _argdata[Switch].ToArray();
				}
				return toret;
			}
		}
		
		public String[] get_argData(String Switch) {
			String[] toret = null;
			if (_argdata.ContainsKey(Switch)) {
				toret = _argdata[Switch].ToArray();
			}
			return toret;
		}
	}
}
