using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
/*
 * 发送数据必须分为三段以 | 分割
 *	str ="内容1 | 内容2 |内容3"  内容1是头部header 2是内容  3是尾部footer
*/
//delegate string NumberChanger(string n);

public class lk_socket : MonoBehaviour {
	
    // Use this for initialization
    void Start() {

    }

    void Update() {

    }

    public string test(string str) {
        return str;
    }

    public void init(string address, string address_port) {
        this.address = address;
        this.address_port = address_port;
    }

    private String address = "127.0.0.1";   //默认地址
    private String address_port = "10888";   //默认端口
    public bool connect_success = false;    //连接是否成功
    public Socket socket = null;

    public bool connect() {
        try {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse(this.address);
            int port = int.Parse(this.address_port);
            IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, port);
            socket.Connect(ipEndpoint);
            this.connect_success = true;
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    //使用异步连接
    public void public_AsynConnect()
    {
        //端口及IP
        IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(this.address), int.Parse(this.address_port));
        //创建套接字
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //开始连接到服务器
        socket.BeginConnect(ipe, asyncResult =>
        {
            try
            {
                socket.EndConnect(asyncResult);
                print("连接上了，恭喜");
				connect_success = true;
            }
            catch (Exception ex) {
                print("没有连接上，请重试" );
            }
            //           socket.EndConnect(asyncResult);
            
            //向服务器发送消息
            /*
            AsynSend(client, "你好我是客户端");
            AsynSend(client, "第一条消息");
            AsynSend(client, "第二条消息");
            //接受消息
            AsynRecive(client);

            */
            //            AsynSend( "once\r");
            //            AsynRecive();
        }, null);
    }
    //    public static int tt = 0;
    //公共异步接收
    private string [] AsynRecive_buff;
    private socket_des soc_des = new socket_des();
    private byte[] receive_data = new byte[1024 * 20];
    public string public_AsynRecive(Socket socket)
    {
        string ret = "";
        
        try
        {
            
            //开始接收数据
            socket.BeginReceive(receive_data, 0, receive_data.Length, SocketFlags.None,
            asyncResult =>
            {
 //               lk_socket.tt += 1;
 //               print("" + tt + "|" + System.DateTime.Now.TimeOfDay.ToString());
                int length = socket.EndReceive(asyncResult);
                ret = Encoding.UTF8.GetString(receive_data , 0,length);
                if (ret != "") {
                    AsynRecive_buff = soc_des.get_data(ret);
                    public_body_socket.s_str[AsynRecive_buff[2]] = ret;
//                    print("type:" + AsynRecive_buff[2] +"|"+ ret);
                }
                
                //接收完了扔进去
                //                print("JIESHOU:" + ret);
//                print(public_body_socket.s_str[type]);
//                print("收到服务器消息:{0}" + Encoding.UTF8.GetString(data));
//                                Console.WriteLine(string.Format("收到服务器消息:{0}", Encoding.UTF8.GetString(data)));
                //                AsynRecive();
            }, null);
            return ret;
        }
        catch (Exception ex)
        {
            Console.WriteLine("异常信息：", ex.Message);
        }
        return ret;
    }
    //公共异步发送
    public void public_AsynSend(Socket socket, string type,string message)
    {
        if (socket == null || message == string.Empty) return;
        //        message += "\r";  //这个不用加，吗的，
        //编码
        byte[] data = Encoding.UTF8.GetBytes(message);
        try
        {
 //           print("" + tt + "|" + System.DateTime.Now.TimeOfDay.ToString());
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
            {
                //完成发送消息
                //                print("发送的消息:" + message);
                int length = socket.EndSend(asyncResult);
				//发送完了之后清除
		//		public_body_socket.f_str[type] = "";
        //        print("客户端发送消息:{0}" + message);
                //                Console.WriteLine(string.Format("客户端发送消息:{0}", message));
            }, null);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("异常信息：{0}", ex.Message);
        }
    }

    private int send_len = 8000;    //超过字符串上限会多次发送
    public string key = "";

    public string other = "-";

    private void send(Socket socket, string send_str) {
        send_str = send_str.Replace("\r", string.Empty).Replace("\n", string.Empty);
        int str_len = send_str.Length;
        //      byte[] byteArray = System.Text.Encoding.Default.GetBytes(send_str);
        byte[] bss = Encoding.UTF8.GetBytes(send_str + "\r");
        if (bss.Length < send_len)
        {
//            send_str = s1 + s2 + s1 + s3 + s1 + s4 + s1 + other + s1 + "1" + s1 + send_str;
            bss = Encoding.UTF8.GetBytes(send_str + "\r");
            socket.Send(bss, bss.Length, 0);
        }
        else
        {
            int for_len = 0;
            if (bss.Length % send_len == 0)
            {
                for_len = bss.Length / send_len;
            }
            else
            {
                for_len = bss.Length / send_len + 1;
            }
            //      string str = s1 + s3 + s1 + s4 + s1 + for_len + s1 + send_str;
            string str = send_str ; // + "\r";
            byte[] fbss = Encoding.UTF8.GetBytes(str);
            int offset = send_len;
            int size = send_len;
            int shengyu = fbss.Length;
            for (int i = 0; i < for_len; i++)
            {
                shengyu = fbss.Length - i * offset;
                if (shengyu < size && shengyu > 0) {
                    size = shengyu;
                }
                socket.Send(bss, i * offset, size, 0);
            }
        }
    }

    //带属性发送
    private void send(Socket socket, string send_str, string other)
    {
        send_str = send_str.Replace("\r", string.Empty).Replace("\n", string.Empty);
       
        int str_len = send_str.Length;
        //      byte[] byteArray = System.Text.Encoding.Default.GetBytes(send_str);
        byte[] bss = Encoding.UTF8.GetBytes(send_str + "\r");
        if (bss.Length < send_len)
        {
            bss = Encoding.UTF8.GetBytes(send_str + "\r");
            socket.Send(bss, bss.Length, 0);
        }
        else
        {
            int for_len = 0;
            if (bss.Length % send_len == 0)
            {
                for_len = bss.Length / send_len;
            }
            else
            {
                for_len = bss.Length / send_len + 1;
            }
            //      string str = s1 + s3 + s1 + s4 + s1 + for_len + s1 + send_str;
            string str = send_str + "\r";
            byte[] fbss = Encoding.UTF8.GetBytes(str);
            int offset = send_len;
            int size = send_len;
            int shengyu = fbss.Length;
            for (int i = 0; i < for_len; i++)
            {
                shengyu = fbss.Length - i * offset;
                if (shengyu < size && shengyu > 0)
                {
                    size = shengyu;
                }
                socket.Send(bss, i * offset, size, 0);
            }
        }
    }

    //分隔符取5位
    private string get_header(string str) {
        string a = new DES().encrypt(new DES().MD5(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), "abcdef").Substring(0, 5);
        int be = str.IndexOf(a);
        if (be == -1)
        {
            return a;
        }
        else
        {
            return this.get_header(str);
        }
    }

    //获取时间戳
    public string GetTimeStamp()
    {
        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }
    //	private static byte[] f_result_init = new byte[1024*8];	//接收数据大小 8K
    private static byte[] f_result_init = new byte[8192];   //接收数据大小 8K 好像是因为有回车所以是8194
    private static byte[] f_result = new byte[1024 * 8];    //接收数据大小 8K

    public string get_res() {
        string ret = "";
        string ret2 = "";
        int receiveLength = socket.Receive(f_result_init);  //接收数据//
        string str2 = Encoding.UTF8.GetString(f_result_init, 0, receiveLength);
        string fengefu = str2.Substring(0, 5);
        string s2 = str2.Substring(5, str2.Length - 5);
        string[] box = s2.Split(new[] { fengefu }, StringSplitOptions.None);
        int len = int.Parse(box[0]);
        if (len == 1) {
            return str2;
        }
        if (len > 1) {
            for (int i = 0; i < len; i++) {
                receiveLength = socket.Receive(f_result_init);  //接收数据//
                ret2 = Encoding.UTF8.GetString(f_result_init, 0, receiveLength);
                ret2 = ret2.Replace("\n", string.Empty).Replace("\r", string.Empty);    //去回车
                ret += ret2;
            }
        }

        return ret;
    }

    public int get_byte_len1() {
        int receiveLength = socket.Receive(f_result_init);  //接收数据//
        string str2 = Encoding.UTF8.GetString(f_result_init, 0, receiveLength);
        int len = int.Parse(str2);
        return len;
    }

    private string get_byte_l = "";

    public byte[] get_byte() {
        int len = socket.Receive(f_result); //接收数据//
        this.get_byte_l = Encoding.ASCII.GetString(f_result, 0, len);
        return f_result;
    }

    public string get_byte_len2() {
        return this.get_byte_l;
    }

    public void send_socket(string str, Socket s) {
        this.socket = s;
        this.connect_success = true;
        this.send(s, str);
    }

    public void send_socket(Socket s, string str) {
        this.socket = s;
        this.connect_success = true;
        this.send(s, str);
    }

    public void send_socket(string str) {
        if (this.connect_success) {
            this.send(this.socket, str);
        } else {
            error.message1("网络连接失败");
        }
    }
    
    public void send_socket(string str, string type)
    {
        if (this.connect_success)
        {
            this.send(this.socket, str, type);
        }
        else
        {
            error.message1("网络连接失败");
        }
    }

    public void close() {
        //        this.send_socket("end");
        string message = "end";
        byte[] data = Encoding.UTF8.GetBytes(message);
        try
        {
            print("关闭之前：" + message);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
            {
                //完成发送消息
                int length = socket.EndSend(asyncResult);
                print("关闭之前客户端发送消息:{0}" + message);
                //                Console.WriteLine(string.Format("客户端发送消息:{0}", message));
            }, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine("异常信息：{0}", ex.Message);
        }
        socket.Close();
    }
}
