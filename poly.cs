using Godot;
using Godot.NativeInterop;
using System;
using System.Drawing;

public partial class poly : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public Vector2[] points;

	bool d = false;
	bool e = false;
	public void setPoints(Vector2[] poly){
		points = poly;
		GD.Print("Points: ");
		foreach(Vector2 p in points){
			GD.Print(p);
		}
		if(points.Length > 2){
			d = true;
			GD.Print(d);
		}
		QueueRedraw();
	}
	public override void _Ready()
	{
		GD.Print("Adding Node");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void drawPoly(){
		
	}
    public override void _Draw()
    {

		if(d && !points.IsEmpty()){
			try{
				Godot.Color[] orange = { new Godot.Color(Colors.Orange, 0.2f) }; // 20% opaque red.
				DrawPolygon(points, orange);
			}catch(Exception error){
				GD.Print("ERROR");
				points = null;
				d = false;
				e = true;
			}
		}
       
	   		foreach(Godot.Vector2 p in points){
					DrawCircle(p, 7, new Godot.Color(Colors.Black, 1.0f));
		}
        // Vector2 v = new Godot.Vector2(400f, 400f);
        // Vector2 s = new Godot.Vector2(50f, 50f);
        // Rect2 rect = new Rect2(v,s);
        // Godot.Color c = Godot.Color.Color8(0,0,0,255);
        // DrawRect(rect,c, true, 2);

    }

}