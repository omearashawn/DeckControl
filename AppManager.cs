using Godot;
using System;
using System.Dynamic;
using System.Threading;
namespace DeckControl;
public partial class AppManager : Control
{	
	// Called when the node enters the scene tree for the first time.
	Node map_node;
	public override void _Ready()
	{
		// Input.MouseMode = Input.MouseModeEnum.Hidden;
		map_node = ResourceLoader.Load<PackedScene>("res://map_node.tscn").Instantiate();
		Engine.PhysicsTicksPerSecond = 1000;
		Engine.MaxPhysicsStepsPerFrame = 30;
		controller_global.Instance.set_axes(0,0,0,0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var controller_state = GetNode<Label>("controller_state");
		controller_state.Text = controller_global.Instance.ToString();
		// controller_state.Show();
		setUI(75);
	}
    public void setUI(int scale){
		var left_joy = GetNode<Sprite2D>("%Left_Joy");
		var right_joy = GetNode<Sprite2D>("%Right_Joy");
		
		left_joy.Offset = new Vector2(controller_global.Instance.left_x * scale, controller_global.Instance.left_y * scale);
		right_joy.Offset = new Vector2(controller_global.Instance.right_x * scale, controller_global.Instance.right_y * scale);

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
				controller_global.Instance.left_y = joystick.AxisValue;
			}
			else if (joystick.IsAction("horizontal_motion"))
			{
				controller_global.Instance.left_x = joystick.AxisValue;
			}
			else if (joystick.IsAction("lift_arm"))
			{
				controller_global.Instance.right_y = joystick.AxisValue;
			}
			else if (joystick.IsAction("tilt_arm"))
			{
				controller_global.Instance.right_x = joystick.AxisValue;
			}
			else{
				return;
				// GD.Print("Unmapped axis: " + joystick.AsText());
			}
			return;
		}
		else if(inputEvent is InputEventJoypadButton button){
			if(button.IsAction("engine_crank")){
				controller_global.Instance.engine_crank = button.IsPressed();
			}
		}
	}
	public void _on_engine_slider_value_changed(float value){
		controller_global.Instance.engineSpeed = value/100;
		GetNode<Label>("%Engine_label").Text = "Engine Speed = " + ((int)(controller_global.Instance.engineSpeed*100)).ToString() + "%";
	}
	public void _on_option_button_toggled(bool state){
		controller_global.Instance.operate = state;
	}
	public void _on_horn_button_button_down(){
		controller_global.Instance.horn = true;
	}
	public void _on_horn_button_button_up(){
		controller_global.Instance.horn = false;
	}
	public void _on_stop_button_toggled(bool state){
		controller_global.Instance.stop = state;
	}

	public void _on_mode_switch_toggled(bool state){
		if(state){
			AddChild(map_node);
		}else{
			RemoveChild(map_node);
		}
	}	
}

