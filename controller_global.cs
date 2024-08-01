using Godot;
using System;

public partial class controller_global : Node
{
	public static controller_global Instance {get; set;}
	public float left_x = 0;
	public float left_y = 0;
	public float right_x = 0;
	public float right_y = 0;
	public byte counter = 0;
	public float engineSpeed = 0;
	public bool operate = false;
	public bool horn = false;

	public bool engine_crank = false;
	public bool stop = false;

	public void print_controller(){
		GD.Print(left_x.ToString("0.##") +  " "  + left_y.ToString("0.##") + " " + right_x.ToString("0.##") + " " + right_y.ToString("0.##"));
	}

	new public string ToString(){
		return left_x.ToString("0.##") +  " "  + left_y.ToString("0.##") + " " + right_x.ToString("0.##") + " " + right_y.ToString("0.##");
	}
	public void set_axes(float lx , float ly, float rx, float ry){
		left_x = lx;
		right_x = rx;
		left_y = ly;
		right_y = ry;
	}
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

}
