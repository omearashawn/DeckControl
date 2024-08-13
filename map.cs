using Godot;
using System;

public partial class map : HttpRequest
{
	int zoom = 19;
	double moveAmount = 0.00005;
	double moveScale = 1;
	double latitude = 46.84838198789008;
	double longitude = -96.89891796403117;
	string requestString = "";
		public override void _Ready(){
		updateString();
		// Create an HTTP request node and connect its completion signal.
		var httpRequest = GetNode<HttpRequest>(".");
		// AddChild(httpRequest);
		httpRequest.RequestCompleted += HttpRequestCompleted;

		// Perform the HTTP request. The URL below returns a PNG image as of writing.
		Error error = httpRequest.Request(requestString);
		if (error != Error.Ok)
		{
			GD.PushError("An error occurred in the HTTP request.");
		}
	}
	public void updateString(){
		requestString = "https://maps.googleapis.com/maps/api/staticmap?center="+ latitude + ","+ longitude +"&zoom="+ zoom+"&size=1279x800&scale=1&maptype=satellite&key=AIzaSyChEEdeD5TdNNSUym-A1odT0Jn4M4durhw";
	}
	public void getMap(){
		updateString();
		var httpRequest = GetNode<HttpRequest>(".");
		// AddChild(httpRequest);
		// httpRequest.RequestCompleted += HttpRequestCompleted;
		httpRequest.CancelRequest();
		Error error = httpRequest.Request(requestString);
		if (error != Error.Ok)
		{
			GD.PushError("An error occurred in the HTTP request.");
		}
	}
	public void _on_home_pressed(){
		zoom = 19;
		moveAmount = 0.00005;
		// moveScale = 1;
		latitude = 46.84838198789008;
		longitude = -96.89891796403117;
		getMap();
	}
	// Called when the HTTP request is completed.
	private void HttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body){
		if (result != (long)HttpRequest.Result.Success)
		{
			GD.PushError("Image couldn't be downloaded. Try a different image.");
		}
		var image = new Image();
		Error error = image.LoadPngFromBuffer(body);
		if (error != Error.Ok)
		{
			GD.PushError("Couldn't load the image.");
		}

		var texture = ImageTexture.CreateFromImage(image);

		// Display the image in a TextureRect node.
		var textureRect = GetNode<TextureRect>("%mapRect");
		// AddChild(textureRect);
		textureRect.Texture = texture;
	}
	public void _on_zoom_in_pressed(){
		zoom += 1;
		// moveScale -= 0.5;
		getMap();
	}
	public void _on_zoom_out_pressed(){
		zoom -= 1;
		// moveScale += 0.5
		getMap();
	}
	public override void _Process(double delta){
	}

    public override void _Input(InputEvent inputEvent){
		if(inputEvent.IsAction("map_up")){
			latitude += moveAmount * moveScale;
		}
		else if(inputEvent.IsAction("map_down")){
			latitude -= moveAmount * moveScale;
		}
		else if(inputEvent.IsAction("map_left")){
			longitude -= moveAmount * moveScale;
		}
		else if(inputEvent.IsAction("map_right")){
			longitude += moveAmount * moveScale;
		}
		else if(inputEvent.IsAction("map_zoom_in")){
			_on_zoom_in_pressed();
			return;
		}
		else if(inputEvent.IsAction("map_zoom_out")){
			_on_zoom_out_pressed();
			return;
		}
		else{
			return;
		}
		getMap();
	}
}
