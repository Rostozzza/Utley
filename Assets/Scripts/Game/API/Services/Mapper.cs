
using Newtonsoft.Json;
using NUnit.Framework;
using Unity.VisualScripting;

namespace API.Sevices.Mapper
{
	public class Mapper
	{
		/// <summary>
		/// Returns string with bear information from an instance of class "Bear".
		/// </summary>
		/// <returns></returns>
		public string BearToString(Bear toConvert)
		{
			return JsonConvert.SerializeObject(toConvert);
		}

		/// <summary>
		/// Returns an instance of class "Bear" from string
		/// </summary>
		/// <param name="toConvert"></param>
		/// <returns></returns>
		public Bear StringToBear(string toConvert)
		{
			Bear result = JsonConvert.DeserializeObject<Bear>(toConvert);
			return result;
		}
	}
}
