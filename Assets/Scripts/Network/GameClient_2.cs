using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameClient_2 : MonoBehaviour
{
    private static Socket clientSocket;
    private static string serverIp = "127.0.0.1";  // 服务器IP地址
    private static int serverPort = 12345;  // 服务器端口
    private bool isConnected = false;

    // Start is called before the first frame update
    void Start() 
    {
        ConnectToServer();
    }

    int index = 1;
    void Update()
    {
        // 可以在这里添加更多的逻辑，例如按键发送消息
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("---------------");
            index *= 2;
            SendMessage__("Hello from Unity Client ! "   + index );
        }
    }


    // 连接到服务器
    public void ConnectToServer()
    {
        try
        {
            // 创建TCP连接
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(serverIp, serverPort, new AsyncCallback(OnConnectCallback), clientSocket);
        }
        catch (Exception ex)
        {
            Debug.LogError("连接到服务器时发生错误: " + ex.Message);
        }
    }

    // 连接回调函数
    private void OnConnectCallback(IAsyncResult ar)
    {
        try
        {
            clientSocket.EndConnect(ar);
            isConnected = true;
            Debug.Log("已成功连接到服务器。");

            // 接收服务器分配的ID
            byte[] buffer = new byte[1024];
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallback), buffer);
        }
        catch (Exception ex)
        {
            Debug.LogError("连接回调时发生错误: " + ex.Message);
        }
    }

    // 接收回调函数
    private void OnReceiveCallback(IAsyncResult ar)
    {
        try
        {
            byte[] buffer = (byte[])ar.AsyncState;
            int bytesRead = clientSocket.EndReceive(ar);

            if (bytesRead > 0)
            {
                string clientId = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log($"您的客户端ID是: {clientId}");

                // 启动接收服务器消息的 Task
                ReceiveMessages().ConfigureAwait(false);  // 使用 ConfigureAwait(false) 防止死锁
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("接收数据时发生错误: " + ex.Message);
        }
    }

    // 发送消息到服务器
    public void SendMessage__(string message)
    {
        try
        {
            // 创建包头（固定字符串或协议）
            byte[] header = Encoding.UTF8.GetBytes("HEADER");

            // 将消息转换为字节数组
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // 获取消息体的长度
            byte[] lengthBytes = BitConverter.GetBytes(messageBytes.Length);

            // 合并包头、包体长度和消息体
            byte[] combinedMessage = new byte[header.Length + lengthBytes.Length + messageBytes.Length];
            Array.Copy(header, 0, combinedMessage, 0, header.Length);
            Array.Copy(lengthBytes, 0, combinedMessage, header.Length, lengthBytes.Length);
            Array.Copy(messageBytes, 0, combinedMessage, header.Length + lengthBytes.Length, messageBytes.Length);

            // 发送完整的数据包
            clientSocket.Send(combinedMessage);
            Debug.Log($"已发送消息: {message}");
        }
        catch (Exception ex)
        {
            Debug.LogError("发送消息时发生错误: " + ex.Message);
        }
    }

    // 使用 Task 接收来自服务器的消息
    public async Task ReceiveMessages()
    {
        try
        {
            byte[] buffer = new byte[1024];

            while (isConnected)
            {
                // 先接收包头（固定长度）
                int bytesRead = clientSocket.Receive(buffer, 0, 6, SocketFlags.None); // 读取包头6个字节
                if (bytesRead == 0)
                {
                    Debug.Log("服务器断开连接。");
                    break; // 服务器断开连接
                }

                string header = Encoding.UTF8.GetString(buffer, 0, 6);  // 读取包头

                if (header != "HEADER")
                {
                    Debug.Log("收到无效的包头。");
                    continue; // 跳过无效包
                }

                // 接收包体长度（接下来的4个字节是包体长度）
                bytesRead = clientSocket.Receive(buffer, 0, 4, SocketFlags.None);
                int messageLength = BitConverter.ToInt32(buffer, 0);  // 获取消息体长度

                // 接收包体
                byte[] messageBuffer = new byte[messageLength];
                int totalReceived = 0;

                while (totalReceived < messageLength)
                {
                    bytesRead = clientSocket.Receive(messageBuffer, totalReceived, messageLength - totalReceived, SocketFlags.None);
                    totalReceived += bytesRead;
                }

                string receivedMessage = Encoding.UTF8.GetString(messageBuffer);
                Debug.Log($"收到来自服务器的消息: {receivedMessage}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("接收消息时发生错误: " + ex.Message);
        }
        finally
        {
            Debug.Log("连接已关闭。");
            clientSocket.Close();  // 关闭连接
        }
    }

    // 在关闭应用时断开连接
    private void OnDestroy() 
    {
        if (clientSocket != null)
        {
            clientSocket.Close();
        }
    }
}
