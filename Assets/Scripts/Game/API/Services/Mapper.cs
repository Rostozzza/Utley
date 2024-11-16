
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
			string result = $"{toConvert.Name} {toConvert.Qualification}";
			return result;
		}

		/// <summary>
		/// Returns an instance of class "Bear" from string
		/// </summary>
		/// <param name="toConvert"></param>
		/// <returns></returns>
		public Bear StringToBear(string toConvert)
		{
			string[] splittedString = toConvert.Split(' ');
			Bear result = new Bear
			{
				Name = splittedString[0],
				Qualification = (Qualification)int.Parse(splittedString[1])
			};
			return result;
		}
	}
}
