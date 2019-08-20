/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 20/08/2019
 * Time: 18:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace captainalm.integrator
{
	/// <summary>
	/// Implements integration loader functionality.
	/// </summary>
	public interface ILoader
	{
		/// <summary>
		/// Loads the integrator.
		/// </summary>
		/// <returns>Integrator</returns>
		/// <remarks></remarks>
		Integrator load();
	}
}
