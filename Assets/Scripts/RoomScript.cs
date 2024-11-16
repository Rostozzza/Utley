using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public class Room
    {
        public int length;
        public int height;
        public Room() { length = 2; height = 1; }
        public Room(int length, int height) { this.length = length; this.height = height; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
