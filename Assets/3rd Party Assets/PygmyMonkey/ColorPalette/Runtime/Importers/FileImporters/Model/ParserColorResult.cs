using UnityEngine;

public struct ParserColorResult
{
	public string name;
	public Color color;
	public bool success;
	
	public ParserColorResult(string name, Color color, bool success)
	{
		this.name = name;
		this.color = color;
		this.success = success;
	}
}
