using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// This is not used right now but could be a good class 
/// in case that we want to make fracsland multiplayer using
/// sockets.
/// </summary>
public class NetworkManager : MonoBehaviour {
	
	public string toSend;
	public float speed = 6.0f;
	string serverText;
	// Use this for initialization
	IPEndPoint serverAddress;
	Socket clientSocket;

	void Start () {
		serverAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1900);
		clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		clientSocket.Connect(serverAddress);//Connects our game to the server.
	}
	
	// Update is called once per frame
	void Update () {

		
	}

	public void SendPosition(){
		// Sending
		toSend = this.transform.position.ToString();
		int toSendLen = System.Text.Encoding.ASCII.GetByteCount(toSend);
		byte[] toSendBytes = System.Text.Encoding.ASCII.GetBytes(toSend);
		byte[] toSendLenBytes = System.BitConverter.GetBytes(toSendLen);
		clientSocket.Send(toSendLenBytes);
		clientSocket.Send(toSendBytes);	
	}

	public void recievePosition(){
		// Receiving
		byte[] rcvLenBytes = new byte[4];
		if(clientSocket.Available>0){
			clientSocket.Receive(rcvLenBytes);
			int rcvLen = System.BitConverter.ToInt32(rcvLenBytes, 0);
			byte[] rcvBytes = new byte[rcvLen];
			clientSocket.Receive(rcvBytes);
			String rcv = System.Text.Encoding.ASCII.GetString(rcvBytes);
			
			serverText =  rcv;
			ejecutarComando(rcv);
		}
	}

	public void ejecutarComando(string rcv){
		

	}

	public void closeConnection(){
		clientSocket.Close();
	}
	
	public void OnGUI(){
	}
	
}