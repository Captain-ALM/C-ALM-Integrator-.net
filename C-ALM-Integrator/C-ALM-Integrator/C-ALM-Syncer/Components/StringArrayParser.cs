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

namespace captainalm.integrator.syncer
{
	/// <summary>
	/// StringArrayParser allows for a command line parsing.
	/// </summary>
	public sealed class StringArrayParser
	{
		private Dictionary<String, List<String>> _argdata = new Dictionary<String, List<String>>();
		
		public StringArrayParser(String[] parseIn, Char switchArgShower)
		{
			var intArgData = new List<Tuple<String, String>>();
			String cc = "";
			Boolean na = false;
			for (int i = 0; i < parseIn.Length; i++) {
				var c = parseIn[i];
				if (! na) {
					if (c.EndsWith(switchArgShower.ToString())) {
						cc = c.Substring(0,c.Length - 1);
						na = true;
					} else {
						intArgData.Add(new Tuple<String, String>(c, null));
					}
				} else {
					na = false;
					intArgData.Add(new Tuple<String, String>(cc,c));
				}
			}
			for (int i = 0; i < intArgData.Count; i++) {
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
		
		public String[] get_argDataIgnoreCase(String Switch) {
			String[] toret = null;
			var bconvt = new String[_argdata.Keys.Count];
			_argdata.Keys.CopyTo(bconvt,0);
			var convt = new Dictionary<String, List<String>>();
			for (int i = 0; i < bconvt.Length; i++) {
				convt.Add(bconvt[i].ToLower(),_argdata[bconvt[i]]);
			}
			if (convt.ContainsKey(Switch.ToLower())) {
				toret = _argdata[Switch].ToArray();
			}
			return toret;
		}
		
		public String[] get_argData(String Switch) {
			String[] toret = null;
			if (_argdata.ContainsKey(Switch)) {
				toret = _argdata[Switch].ToArray();
			}
			return toret;
		}
		
		public Int32 count {
			get {
				return _argdata.Count;
			}
		}
		
		public String[] Switches {
			get {
				var toret = new String[_argdata.Count];
				_argdata.Keys.CopyTo(toret,0);
				return toret;
			}
		}
		
		public Boolean hasSwitch(String Switch) {
			return _argdata.ContainsKey(Switch);
		}
		
		public Boolean hasSwitchIgnoreCase(String Switch) {
			var bconvt = new String[_argdata.Keys.Count];
			_argdata.Keys.CopyTo(bconvt,0);
			var convt = new List<String>();
			for (int i = 0; i < bconvt.Length; i++) {
				convt.Add(bconvt[i].ToLower());
			}
			return convt.Contains(Switch.ToLower());
		}
	}
}
