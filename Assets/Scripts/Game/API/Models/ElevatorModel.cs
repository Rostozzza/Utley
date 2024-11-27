using System.Collections.Generic;

public class ElevatorModel
{
	public List<float> Coordinates { get; set; }
	public List<int> ConnectedElevators { get; set; }
	public List<int> ConnectedRooms { get; set; }
	public int BlocksUp { get; set; }
	public int BlocksDown { get; set; }
	public int Index { get; set; }
}