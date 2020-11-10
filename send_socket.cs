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

class send_socket : MonoBehaviour{
	private Socket socket = null;
	public send_socket(string str , Socket s){
		this.socket = s;
		this.send(s , str);
	}
	public send_socket(Socket s , string str){
		this.socket = s;
		this.send(s , str);
	}

	private void send(Socket socket , string send_str){
		int send_len = 0;
		int send_maxlen = 8192;	
		if(send_str.Length % send_maxlen == 0){
			send_len = send_str.Length / send_maxlen ;
		}else{
			send_len = send_str.Length / send_maxlen + 1;
		}
		string send_str2 = "";
		int starts = 0;
		int end = 0;
		int max_len = send_str.Length;
		
		string send_str3 = "len:"+send_len;
		
		byte[] bss = Encoding.UTF8.GetBytes (send_str3 + "\r");
		socket.Send(bss,bss.Length,0);
		byte[] bs = new byte[8192];
		for(int i = 0 ; i < send_len ; i++){
			starts = i * send_maxlen;
			end = i * send_maxlen + send_maxlen;
			if(end > max_len){
				end = max_len;
				send_maxlen = end - starts;
			}
			send_str2 = send_str.Substring(starts,send_maxlen);
			bs = Encoding.UTF8.GetBytes (send_str2 + "\r");
			socket.Send(bs,bs.Length,0);
		} 
	}
//	private static byte[] f_result_init = new byte[1024*8];	//接收数据大小 8K
	private static byte[] f_result_init = new byte[8194];	//接收数据大小 8K

	public string get_res(){
		string ret = "";
		string ret2 = "";
		
		int receiveLength = socket.Receive (f_result_init);	//接收数据//
		string str2 = Encoding.UTF8.GetString (f_result_init,0,receiveLength);
		int len = int.Parse(str2);
		if(len > 0){
			for(int i = 0 ; i < len ; i++){
				receiveLength = socket.Receive (f_result_init);	//接收数据//
				ret2 = Encoding.UTF8.GetString (f_result_init,0,receiveLength );
				ret2 = ret2.Replace("\n", string.Empty).Replace("\r", string.Empty);	//去回车
				ret += ret2;
			}
		}
		return ret;
	}
}