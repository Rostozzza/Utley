using System.Collections.Generic;

public class Log
{
	public string comment { get; set; }
	public string player_name { get; set; }
	public string shop_name {  get; set; }
	public Dictionary<string, float> resources_changed { get; set; }
}