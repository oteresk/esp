using Godot;
using System;
using System.Diagnostics;

public partial class ResourceDiscovery : Sprite2D
{
	[Export] public ResourceDiscoveryResource RDResource;
	public override void _Ready()
	{
		Texture2D tx=(Texture2D)RDResource.sprImage;
		this.Texture=tx;

	}

	public override void _Process(double delta)
	{
	}
}
