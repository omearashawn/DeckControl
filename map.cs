using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class map : HttpRequest
{
	int zoom = 19;
	double moveAmount = 0.00005;
	double moveScale = 1;
	double latitude = 46.84838198789008;
	double longitude = -96.89891796403117;
	string requestString = "";
	int screenx = 1280;
	int screeny = 800;
	bool drawing = false;
	public List<Vector2> polygon = new List<Vector2>();
	Node poly_node;
	public override void _Ready(){
		poly_node = ResourceLoader.Load<PackedScene>("res://poly_node.tscn").Instantiate();
		AddChild(poly_node);
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
		requestString = "https://maps.googleapis.com/maps/api/staticmap?center="+ latitude + ","+ longitude 
		+"&zoom="+ zoom+"&size="+ screenx +"x"+screeny
		+"&scale=1&maptype=satellite&key=AIzaSyChEEdeD5TdNNSUym-A1odT0Jn4M4durhw";
	}

	public void _on_draw_path_toggled(bool toogle){
		drawing = toogle;
	}
	public double getGroundResolutionMeters(){
		//returns in meters how many meters a pixel wide a pixel is at a lat. 
		//ground resolution = cos(latitude * pi/180) * earth circumference / map width
		return Math.Cos(latitude * (Math.PI / 180.0)) * (2.0 * Math.PI * 6378137.0) / (512 * Math.Pow(2, zoom));
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
		double gr = getGroundResolutionMeters();
		GD.Print("Ground Resolution is " + gr + "m ");
		GD.Print("Area is: " + (gr * 1280) + "m x " + (gr * 800) + "m");
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

    public override void _Input(InputEvent inputEvent){
		if(inputEvent is InputEventMouseButton mouseButton && mouseButton.Pressed){
			Vector2 click = mouseButton.Position;
			getClickGPS(click);
			if(drawing){
				polygon.Add(click);
				GD.Print("adding point: "+ click);
				try{
				poly_node.Call("setPoints", polygon.ToArray());
				}catch{
					GD.Print("error");
					polygon.Clear();
					poly_node.Call("setPoints", polygon.ToArray());
				}
			}
			return;
		}
		else if(inputEvent.IsAction("map_up")){
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

	public void _on_reset_pressed(){
		polygon.Clear();
		poly_node.Call("setPoints", polygon.ToArray());
	}
    private void getClickGPS(Vector2 click)
    {
        GD.Print("Click at " + click[0] + ", " + click[1]);
		double clickLat = (((double)click[0] - (screenx/2.0)) * getGroundResolutionMeters());
		double clickLong = (((double)click[1] - (screeny/2.0)) * getGroundResolutionMeters());
		PixelXYToLatLong((int)click[0], (int)click[1]);
	}
	public void PixelXYToLatLong(int pixelX, int pixelY)  
        {  
			LatLongToGlobalPixelXY(out int centerX, out int centerY);
			pixelX = centerX - pixelX;
			pixelY = centerY - pixelY;
            double mapSize = MapSize();  
            double x = (pixelX / mapSize) - 0.5;  
            double y = 0.5 - (pixelY / mapSize);  
  
            double clickLatitude = 90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI;  
            double clcikLongitude = 360 * x;  

			GD.Print("Latitude: " + clickLatitude + " Longitude: " + clcikLongitude);
        }
	public void LatLongToGlobalPixelXY(out int pixelX, out int pixelY)  
        {  
            double x = (longitude + 180) / 360;   
            double sinLatitude = Math.Sin(latitude * Math.PI / 180);  
            double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);  
  
            int mapSize = MapSize();  
            pixelX = (int) (x * mapSize + 0.5);  
            pixelY = (int) (y * mapSize + 0.5);  
        } 
    private int MapSize()
    {
        return 512 << zoom;
    }

}
