using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SocketServer : MonoBehaviour
{
    [SerializeField]
    int _port = 8080;

    private TcpListener _listener;
    private readonly List<TcpClient> _clients = new List<TcpClient>();

    // Start is called before the first frame update
    void Start()
    {
        // 接続中のIPアドレスを取得
        // var ipAddress = Network.player.ipAddress;
        // 指定したポートを開く
        Listen("0.0.0.0", _port);
    }

    // 終了処理
    protected virtual void OnApplicationQuit() {
        if (_listener == null){
            return;
        }

        if (_clients.Count != 0){
            foreach(var client in _clients){
                client.Close();
            }
        }
        _listener.Stop();
    }

    // ソケット接続準備、待機
    protected void Listen(string host, int port){
        Debug.Log("ipAddress:"+host+" port:"+port);
        var ip = IPAddress.Parse(host);
        _listener = new TcpListener(ip, port);
        _listener.Start();
        _listener.BeginAcceptSocket(DoAcceptTcpClientCallback, _listener);
    }

    // クライアントからの接続処理
    private void DoAcceptTcpClientCallback(IAsyncResult ar) {
        var listener = (TcpListener)ar.AsyncState;
        var client = listener.EndAcceptTcpClient(ar);
        _clients.Add(client);
        Debug.Log("Connect: "+client.Client.RemoteEndPoint);

        // 接続が確立したら次の人を受け付ける
        listener.BeginAcceptSocket(DoAcceptTcpClientCallback, listener);

        // 今接続した人とのネットワークストリームを取得
        var stream = client.GetStream();
        var reader = new StreamReader(stream,Encoding.UTF8);

        // 接続が切れるまで送受信を繰り返す
        while (client.Connected) {
            while (!reader.EndOfStream){
                // 一行分の文字列を受け取る
                var str = reader.ReadLine ();
                OnMessage(str);
            }

            // クライアントの接続が切れたら
            if (client.Client.Poll(1000, SelectMode.SelectRead) && (client.Client.Available == 0)) {
                Debug.Log("Disconnect: "+client.Client.RemoteEndPoint);
                client.Close();
                _clients.Remove(client);
                break;
            }
        }
    }

    // メッセージ受信
    protected virtual void OnMessage(string msg){
        Debug.Log(msg);
    }

}
