using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using static System.Buffers.Binary.BinaryPrimitives;
using DeckControl;
using System.Runtime.InteropServices;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Buffers;
using System.Net.Sockets;
using System.Net;
using System.Collections;

public partial class Network : Node
{
	Byte[] packet = new Byte[31];
	Socket s; 
	const int PORT =8080;
	// const string IP = "192.168.1.255";
	const string IP = "172.20.228.255";
	// const string IP = "127.0.0.1";
	IPEndPoint endPoint;
	public override void _Ready()
	{	
		s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        endPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	//Need to convert all information into a BigEndian byte array to send over UDP
	public override void _Process(double delta)
	{
		
	}
	//runs well at 20ms about 0.05% late rate at 10ms about 6% late rate
    public override void _PhysicsProcess(double delta)
    {
		Byte[] packet = packetBuilder();
		s.SendTo(packet, endPoint);
    }

	public Byte[] packetBuilder(){
		Byte [] time_bytes = BitConverter.GetBytes(Time.GetUnixTimeFromSystem()); //UTC time in sec. 8 bytes
		GD.Print(Time.GetUnixTimeFromSystem());
		// time_bytes.Reverse();  <- reversing doesn't change outcome at server side?
		Byte[] lx_bytes = BitConverter.GetBytes(controller_global.Instance.left_x); //4 bytes
		Byte[] ly_bytes = BitConverter.GetBytes(controller_global.Instance.left_y);
		Byte[] rx_bytes = BitConverter.GetBytes(controller_global.Instance.right_x);
		Byte[] ry_bytes = BitConverter.GetBytes(controller_global.Instance.right_y);
		Byte[] engine_bytes = BitConverter.GetBytes(controller_global.Instance.engineSpeed);
		Byte state_byte = setStateByte();
		// packet = lx_bytes.Concat(lx_bytes).Concat(ly_bytes).Concat(rx_bytes).Concat(ry_Bytes).ToArray();
		int pos = 0;
		packet[pos] = controller_global.Instance.counter++; //insert byte counter and advance by 1
		pos++;
		Buffer.BlockCopy(time_bytes,0,packet,pos,time_bytes.Length); //insert data into desired place in packet buffer 
		pos += time_bytes.Length; //advance count my length of data
		Buffer.BlockCopy(engine_bytes,0,packet,pos,engine_bytes.Length);
		pos += engine_bytes.Length;
		Buffer.BlockCopy(lx_bytes, 0, packet, pos ,4);
		pos += lx_bytes.Length;
		Buffer.BlockCopy(ly_bytes, 0, packet, pos, 4);
		pos += ly_bytes.Length;
		Buffer.BlockCopy(lx_bytes, 0, packet, pos, 4);
		pos += rx_bytes.Length;
		Buffer.BlockCopy(lx_bytes, 0, packet, pos, 4);
		pos += ry_bytes.Length;
		packet[pos] = state_byte;
		return packet;
	}

	public byte setStateByte(){
		BitArray b = new BitArray(8, false);
		b[0] = controller_global.Instance.operate;
		b[1] = controller_global.Instance.horn;
		byte[] bytes = new byte[1];
    	b.CopyTo(bytes, 0);
    	return bytes[0];
	}
	
}
