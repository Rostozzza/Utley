using System.Collections.Generic;

public class Room
{
	public string Type { get; set; }
	public int Level { get; set; }
	public List<float> Coordinates { get; set; }
	public List<int> ConnectedElevators { get; set; }
	public int Index { get; set; }
}