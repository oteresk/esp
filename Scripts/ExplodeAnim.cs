using Godot;
using System;
using System.Diagnostics;

public partial class ExplodeAnim : AnimatedSprite2D
{
    private AnimatedSprite2D anim;

    public override void _Ready()
    {
        anim = GetNode<AnimatedSprite2D>(".");
        anim.Play("Explode");
    }

    public void OnFinished()
    {
        //Debug.Print("Queue free:" + GetParent());
        GetParent().QueueFree();
    }
}
