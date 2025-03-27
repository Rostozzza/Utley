
using System.Collections.Generic;

public class LocalEvent
{
	public string name { get; set; }
	public string description { get; set; }
	public string icon_path { get; set; }
	public string start_date_time { get; set; }
	public int duration_in_minutes { get; set; }
	public Dictionary<string, float> multipliers { get; set; }
}
