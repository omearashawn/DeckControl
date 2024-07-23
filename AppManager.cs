using Godot;
using System;
using System.Dynamic;
using System.Threading;
namespace DeckControl;
public partial class AppManager : Control
{	
	int counter = 0;
	Controller controller = new Controller();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Engine.PhysicsTicksPerSecond = 10000000;
		Engine.MaxPhysicsStepsPerFrame = 1000;
		controller.set_axes(0,0,0,0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		counter = 0;
		var controller_state = GetNode<Label>("/root/Control/controller_state");
		controller_state.Text = controller.ToString();
		controller_state.Show();
		controller.print_controller();
		setUI(20);
	}
    public override void _PhysicsProcess(double delta)
    {
        GD.Print(counter++);
    }
    public void setUI(int scale){
		var left_joy = GetNode<Sprite2D>("/root/Control/%Left_Joy");
		var right_joy = GetNode<Sprite2D>("/root/Control/%Right_Joy");
		
		left_joy.Offset = new Vector2(controller.left_x * scale, controller.left_y * scale);
		right_joy.Offset = new Vector2(controller.right_x * scale, controller.right_y * scale);

		left_joy.Show();
		right_joy.Show();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if(inputEvent is InputEventJoypadMotion joystick){
			if(Math.Abs(joystick.AxisValue) <= 0.15)
				joystick.AxisValue = 0;

			if (joystick.IsAction("vertical_motion"))
			{
				controller.left_y = joystick.AxisValue;
			}
			else if (joystick.IsAction("horizontal_motion"))
			{
				controller.left_x = joystick.AxisValue;
			}
			else if (joystick.IsAction("lift_arm"))
			{
				controller.right_y = joystick.AxisValue;
			}
			else if (joystick.IsAction("tilt_arm"))
			{
				controller.right_x = joystick.AxisValue;
			}
			else{
				return;
				// GD.Print("Unmapped axis: " + joystick.AsText());
			}
		}
	}

}

public class Controller{
		public float left_x = 0;
		public float left_y = 0;
		public float right_x = 0;

		public float right_y = 0;

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
}