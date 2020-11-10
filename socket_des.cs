using UnityEngine;
using System;

public class socket_des : MonoBehaviour {
    
    public static String key = "";
    //传输到服务器的代码在这里处理好然后发送
    /*
    send_str 发送的内容
    other    发送的类型
    server_time 服务器时间
    */
    /*
    public String encrypt(String send_str, String type = "-") {
        String server_time = "0";
        send_str = send_str.Replace("\r", String.Empty).Replace("\n", String.Empty);
        String s1 = this.get_header(send_str);  //分隔符 5 位
        String s3 = this.GetTimeStamp();    //获取时间戳
        String s4 = new DES().MD5(send_str);
        String s2 = "-";
        if (socket_des.key != "")
        {
            s2 = new DES().encrypt(s4, socket_des.key);
        }
        //这里的1代表发送次数
        send_str = s1 + s2 + s1 + s3 + s1 + s4 + s1 + type + s1 + "1" + s1 + server_time + s1 + send_str + s1;
        //        byte[] bss = Encoding.UTF8.GetBytes(send_str + "\r");
        return send_str + "\r" ;
    }

    public String encrypt(String send_str, String type = "-", String server_time = "0") {
        send_str = send_str.Replace("\r", String.Empty).Replace("\n", String.Empty);
        String s1 = this.get_header(send_str);  //分隔符 5 位
        String s3 = this.GetTimeStamp();    //获取时间戳
        String s4 = new DES().MD5(send_str);
        String s2 = "-";
        if (socket_des.key != "")
        {
            s2 = new DES().encrypt(s4, socket_des.key);
        }
        //这里的1代表发送次数
        send_str = s1 + s2 + s1 + s3 + s1 + s4 + s1 + type + s1 + "1" + s1 + server_time + s1 + send_str + s1;
        //        byte[] bss = Encoding.UTF8.GetBytes(send_str + "\r");
        return send_str + "\r" ;
    }
    */
    /*
        send_str    //发送的数据
        type        //类型
        server_time //发送给服务器时间，没给
        send_num    //发送的次数，没给，0没有限制
        un_id       //发送的唯一id
    */
    public String encrypt(String send_str, String type = "-", String server_time = "0" , String send_num = "0" , String un_id = "-") {
        send_str = send_str.Replace("\r", String.Empty).Replace("\n", String.Empty);
        String s1 = this.get_header(send_str);  //分隔符 5 位
        String s3 = this.GetTimeStamp();    //获取时间戳
        String s4 = new DES().MD5(send_str);
        String s2 = "-";
        if (socket_des.key != "")
        {
            s2 = new DES().encrypt(s4, socket_des.key);
        }
        //这里的1代表发送次数
        send_str = s1 + s2 + s1 + s3 + s1 + s4 + s1 + type + s1 + send_num + s1 + server_time + s1 + send_str + s1 + un_id + s1;
        //        byte[] bss = Encoding.UTF8.GetBytes(send_str + "\r");
        return send_str + "\r" ;
    }

    //分隔符取5位
    private String get_header(String str)
    {
        String a = new DES().encrypt(new DES().MD5(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), "abcdef").Substring(0, 5);
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
    public String GetTimeStamp()
    {
        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        String c_time = (Convert.ToInt64(ts.TotalSeconds)-8*3600).ToString();   //修正了一下
//        print(c_time);
        return c_time;
    }

    /*
        把获取的数据切割一下
        例如 ：9_3iI00000000019_3iI2019-09-21 15:31:009_3iI9_3iI0

    */
    String recive_content;
    String [] recive_data;
    String fengefu;
    public String [] get_data(string  str) {
        fengefu = str.Substring(0, 5);
        String [] condition = { fengefu };
        recive_content = str.Substring(5, str.Length-5);
        recive_data = recive_content.Split(condition, StringSplitOptions.None);
        //data[0]   次数
        //data[1]   服务器返回时间
        //data[2]   服务器返回类型
        //data[3]   服务器返回内容
        return recive_data;
    }

//    public string get_content() {
//        return recive_data[3];
//    }
    public String get_un_id() {
        return recive_data[4];
    }

    public String get_content() {
        return recive_data[3];
    }

    public String type() {
        return recive_data[2];
    }

    public String get_server_time() {
        return recive_data[1];
    }
}
