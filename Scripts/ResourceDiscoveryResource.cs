using Godot;
using System;
using System.Net;

[GlobalClass]
public partial class ResourceDiscoveryResource : Resource
{
	[Export] public Texture2D sprImage;
	[Export] public float timeToCapture;
	[Export] public ResourceType resourceType;

	[Export] public float amount;
	[Export] public float freq;
	[Export] public RecoverType recoverType;
	[Export] public float spriteScale;

	public Sprite2D spr;
	
}



public enum ResourceType{
	Iron,
	Mana,
	Gold,
	Wood
}

public enum RecoverType{
	Capture,
	Attack
}
