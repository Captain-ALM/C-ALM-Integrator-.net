/*
 * Created by SharpDevelop.
 * User: Alfred
 * Date: 20/08/2019
 * Time: 18:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace captainalm.integrator
{
	/// <summary>
	/// Implements integration saver functionality.
	/// </summary>
	public interface ISaver
	{
		/// <summary>
		/// Saves the integrator.
		/// </summary>
		/// <param name="toSave">The integrator to save</param>
		/// <remarks></remarks>
		void save(Integrator toSave);
	}
}
