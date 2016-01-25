﻿/*using System.Collections;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using System.IO;
using System.Diagnostics;

public class TCPClientManager
{   

    public event EventHandler<CommandEventArgs> CommandReceivedEvent;
    public event EventHandler<ConnectEventArgs> ConnectEvent;
    public event EventHandler<DisconnectEventArgs> DisconnectEvent;


	private TcpClient socket;
    private Thread runThread;
	private string buffer;
    
    private bool _conectado = false;
    public bool conectado
    {
        get
        {
            return _conectado;
        }
    }

    private string ipServidor;
    private int portServidor;

    private string separadorDeComandos;



    private float tRetry = 4;
    private StreamWriter streamWriter;
    bool run = true;
   
    public void iniciar(string ip, int port, string separadorDeComandos = ";")
    {
        Console.WriteLine("Conectando con el servidor");
        run = true;
        if (runThread == null)
        {
            runThread = new Thread(new ThreadStart(Update));
            runThread.Start();
        }
        disconnectTest = Encoding.UTF8.GetBytes("0"+separadorDeComandos);
        ipServidor = ip;
        portServidor = port;

        this.separadorDeComandos = separadorDeComandos;

        //Debug.Log("Iniciando cliente TCP en " + ip + ":" + port);

        _conectado = false;

        try
        {
            socket = new TcpClient(ip, port);
            netStream = socket.GetStream();
            _conectado = true;
            Console.WriteLine("Concetado");
            onConnect();

        }
        catch (Exception e)
        {
            timer = new Timer(ReintentarConexionAhora, null, 3000, Timeout.Infinite);
        }
    }
    Timer timer;

    private void ReintentarConexionAhora(object state)
    {
        timer.Dispose();
        if (!_conectado) iniciar(ipServidor, portServidor);
    }
    private void ReintentarConexionAhora()
    {
        if (!_conectado) iniciar(ipServidor, portServidor);
    }

    NetworkStream netStream;
    DateTime t;
    void Update()
    {
        t = DateTime.Now;
        while (run)
        {
            if (_conectado && netStream.CanRead)
            {
                byte[] myReadBuffer = new byte[1024];
                StringBuilder myCompleteMessage = new StringBuilder();
                int numberOfBytesRead = 0;
                while (netStream.DataAvailable)
                {
                    numberOfBytesRead = netStream.Read(myReadBuffer, 0, myReadBuffer.Length);

                    myCompleteMessage.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));

                }

                string mensajeCompleto = myCompleteMessage.ToString();
                if (mensajeCompleto != "")
                {
                    OnData(mensajeCompleto);
                }
            }

            if (_conectado && netStream.CanWrite)
            {
                if(DateTime.Now.Subtract(t).TotalSeconds > tRetry)
                {
                    t = DateTime.Now;

                    if (!IsConnected)
                    {
                        _conectado = false;
                        onDisconnect();
                        ReintentarConexionAhora();
                    }
                }
            }



        }
    }

    private bool IsConnected
    {           
        get{
            try
            {
                if (socket != null && socket.Client != null && socket.Client.Connected)
                {
                    // Detect if client disconnected
                    if (socket.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (socket.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            // Client disconnected
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
}
    }

    byte[] disconnectTest;
    static byte[] GetBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }
    public void SendData(string data)
    {
        if(_conectado)
        {
            Console.WriteLine("Envio: " + data);
            Stream s = socket.GetStream();
            byte [] d = Encoding.UTF8.GetBytes(data+separadorDeComandos);
            s.Write(d, 0, d.Length);
            s.Flush();
        }
    }
    void onConnect()
    {
        EventHandler<ConnectEventArgs> newEvent = ConnectEvent;
        ConnectEventArgs _arg = new ConnectEventArgs();
        if (newEvent != null) newEvent(this, _arg);
    }
    void onDisconnect()
    {
        EventHandler<DisconnectEventArgs> newEvent = DisconnectEvent;
        DisconnectEventArgs arg = new DisconnectEventArgs();
        if (newEvent != null) newEvent(this, arg);
    }
    private void enviarEventoComando(string comando) {
        EventHandler<CommandEventArgs> newEvent = CommandReceivedEvent;
        CommandEventArgs _arg = new CommandEventArgs( comando );
        if (newEvent != null) newEvent(this, _arg);
    }

	void OnData(string data)
	{
		data = buffer + data;
		//data += buffer;
        //Debug.Log("Recibida data en el buffer: "+data);
		string[] commands = data.Split (new string[] {separadorDeComandos}, StringSplitOptions.None);
		buffer = commands[commands.Length-1];

        //Debug.Log("Se lograron extraer "+(commands.Length - 1)+" comandos finalizados con '"+separadorDeComandos+"'");

		for (int j = 0; j < commands.Length-1; j++)
        {
            if(commands[j]=="")
            {
                continue;
            }

            //Debug.Log("Enviando evento de comando: "+commands[j]);
            enviarEventoComando(commands[j]);

		}

        //Debug.Log("Quedo en el buffer: '" + buffer + "'");

	}
	void OnConnect()
	{
		//Debug.Log ("Connected");
        streamWriter = new StreamWriter(socket.GetStream());
	}
	void OnApplicationQuit()
	{
		if(socket != null)
		{
			socket.Close ();          
		}

	}

}

public class CommandEventArgs : EventArgs
{
    public string comando = "";

    public CommandEventArgs(string comando)
    {
        this.comando = comando;
    }
}
public class ConnectEventArgs : EventArgs
{
    //public string comando = "";

    public ConnectEventArgs()
    {
        //this.comando = comando;
    }
}
public class DisconnectEventArgs : EventArgs
{
    //public string comando = "";

    public DisconnectEventArgs()
    {
        //this.comando = comando;
    }
}
*/