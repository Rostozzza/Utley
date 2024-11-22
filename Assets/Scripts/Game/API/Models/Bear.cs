using UnityEngine;

public enum Qualification
{
	researcher,
	bioengineer,
	builder,
	beekeeper,
	coder,
	creator
}

public class Bear
{
	public string Name { get; set; }
	public Qualification Qualification { get; set; }
	public int Level;
	public GameObject Fur {  get; set; }
	public GameObject Top { get; set; }
	public GameObject Bottom { get; set; }
}