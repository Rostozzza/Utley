using System;

[Serializable]
public enum Qualification
{
	researcher,
	bioengineer,
	builder,
	beekeeper,
	coder,
	creator
}

[Serializable]
public class Bear
{
	public string Name { get; set; }
	public Qualification Qualification { get; set; }
	public int Level;
	public string Fur {  get; set; }
	public string Top { get; set; }
	public string Bottom { get; set; }
}