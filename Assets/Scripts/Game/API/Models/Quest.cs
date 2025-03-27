
using System.Collections.Generic;

public class Quest
{
	public string name { get; set; }
	public string description { get; set; }
	public int timeLimit {  get; set; }
	public KeyValuePair<string, int> condition {  get; set; }
}
