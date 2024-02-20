using Godot;
using System;
using System.Diagnostics;

public partial class FogOfWar : Node2D
{
// using in a scene:
// drag FoWfog(Sprite2D) into fog field
// drag Art/Light.png into light field
// drag Player into player field
// fog size maxes out at about 13000x13000



[Export]
    public Sprite2D fog;

    [Export]
    public int fogWidth = 1000;

    [Export]
    public int fogHeight = 1000;

    [Export]
    public CompressedTexture2D LightTexture;

    [Export]
    public int lightWidth = 300;

    [Export]
    public int lightHeight = 300;

    [Export]
    public Area2D Player;

    [Export]
    public float debounce_time = 0.01f;

    private double time_since_last_fog_update = 0.0f;
    private Image fogImage;
    private Image lightImage;
    private Vector2 light_offset;
    private ImageTexture fogTexture;
    private Rect2I light_rect;

    // Called every time the node is added to the scene
    public override void _Ready()
    {
        float hfw=fogWidth/2;
        float hfh=fogHeight/2;
// position FoW node based on size
        Position=new Vector2(-hfw,-hfh);

        lightImage = LightTexture.GetImage();
        lightImage.Resize(lightWidth, lightHeight);

        light_offset = new Vector2(lightWidth / 2-hfw, lightHeight / 2-hfh);

        //fogImage = new Image();
		fogImage = Image.Create(fogWidth, fogHeight, false, Image.Format.Rgba8);
        fogImage.Fill(new Color(0, 0, 0, 2));
        fogTexture = new ImageTexture();
        fogTexture=ImageTexture.CreateFromImage(fogImage);
        fog.Texture = fogTexture;

        light_rect = new Rect2I(Vector2I.Zero, lightImage.GetSize());

        update_fog((Vector2I)Player.Position);
    }

    // Update the fog
    private void update_fog(Vector2I pos)
    {
        // offset light by a random amount
        float xOff = (float)GD.RandRange(0.0f, 5.0f);
        float yOff = (float)GD.RandRange(0.0f, 5.0f);
        Vector2 rndOffset = new Vector2(xOff, yOff);
        fogImage.BlendRect(lightImage, light_rect, (Vector2I)pos - (Vector2I)(light_offset+rndOffset));
        fogTexture.Update(fogImage);
    }

    // Main render loop
    public override void _Process(double delta)
    {
        time_since_last_fog_update += delta;
        if (time_since_last_fog_update >= debounce_time && Visible)
        {
            time_since_last_fog_update = 0.0f;
            update_fog((Vector2I)new Vector2(Player.Position.X/14, Player.Position.Y/14));
        }
    }

}
