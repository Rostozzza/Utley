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
}