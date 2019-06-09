using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;//引入串口操作的命名空间
using System.Threading;//引入线程的命名空间

namespace SerialCom
{
    class RFIDHelper
    {
        public static SerialPort sp = new SerialPort();//实例化串口
        public static string[] comNumber = SerialPort.GetPortNames();//获取当前计算机的串行端口名称数据

        //定义主机命令数据帧数组
        //设备控制类命令
        public static byte[] GetDvcInfo = { 0x06, 0x01, 0x41, 0x00, 0x00, 0x03 };//定义读设备信息字节数组（Cmd = A）
        public static byte[] PCDConfig = { 0x06, 0x01, 0x42, 0x00, 0x00, 0x03 };//定义配置读卡芯片字节数组（Cmd = B）
        public static byte[] PCDClose = { 0x06, 0x01, 0x43, 0x00, 0x00, 0x03 };//定义关闭读卡芯片字节数组（Cmd = C）

        //ISO14443A类命令
        public static byte[] PiccRequest = { 0x07, 0x02, 0x41, 0x01, 0x52, 0x00, 0x03 };//定义请求字节数组（Cmd = A）
        public static byte[] PiccAnticoll = { 0x08, 0x02, 0x42, 0x02, 0x93, 0x00, 0x00, 0x03 };//定义防碰撞字节数组（Cmd = B）

        public static byte[] MacthingCardID = new byte[4];

        public static byte[] PiccSelect = { 0x0b, 0x02, 0x43, 0x05, 0x93, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x03 };//定义选择字节数组（Cmd = C）
        public static byte[] PiccHalt = { 0x06, 0x02, 0x44, 0x00, 0x00, 0x03 };//定义暂停字节数组（Cmd = D）
        public static byte[] PiccAuthKey = { 0x12, 0x02, 0x46, 0x0c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x03 };//定义直接密码证实字节数据（Cmd = F）
        public static byte[] PiccRead_Block04 = { 0x07, 0x02, 0x47, 0x01, 0x00, 0x00, 0x03 };//定义读块04字节数组（Cmd = G）
        public static byte[] PiccRead_Block05 = { 0x07, 0x02, 0x47, 0x01, 0x00, 0x00, 0x03 };//定义读块05字节数组（Cmd = G）
        public static byte[] PiccWrite_Block04 = { 0x17, 0x02, 0x48, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03 };//定义写块04字节数组（Cmd = H）
        public static byte[] PiccWrite_Block05 = { 0x17, 0x02, 0x48, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03 };//定义写块05字节数组（Cmd = H）
        public static byte[] PiccAddValue = { 0x0d, 0x02, 0x4a, 0x07, 0xc1, 0x04, 0x01, 0x01, 0x01, 0x01, 0x04, 0x00, 0x03 };//定义加值字节数组（Cmd = J）
        public static byte[] PiccSubValue = { 0x0d, 0x02, 0x4a, 0x07, 0xc0, 0x04, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x03 };//定义减值字节数组（Cmd = J）
        public static byte[] PiccBlockRead = { 0x0f, 0x02, 0x52, 0x09, 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03 };//定义单指令块读字节数组（Cmd = R）
        public static byte[] PiccBlockWrite = { 0x1f, 0x02, 0x57, 0x19, 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,0x88, 0x99,
            0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff, 0x00, 0x03 };//定义单指令块写字节数组（Cmd = W）

        //定义从机应答数据帧数组
        public static byte[] buffer_GetDvcInfo = new byte[18];
        public static byte[] buffer_PCDConfig = new byte[6];
        public static byte[] buffer_PCDClose = new byte[6];
        public static byte[] buffer_PiccRequest = new byte[8];
        public static byte[] buffer_PiccAnticoll = new byte[10];
        public static byte[] buffer_PiccSelect = new byte[7];
        public static byte[] buffer_PiccHalt = new byte[6];
        public static byte[] buffer_PiccAuthKey = new byte[6];
        public static byte[] buffer_PiccRead_Block04 = new byte[22];
        public static byte[] buffer_PiccRead_Block05 = new byte[22];
        public static byte[] buffer_PiccWrite_Block04 = new byte[6];
        public static byte[] buffer_PiccWrite_Block05 = new byte[6];
        public static byte[] buffer_PiccAddValue = new byte[6];
        public static byte[] buffer_PiccSubValue = new byte[6];
        public static byte[] buffer_PiccBlockRead = new byte[22];
        public static byte[] buffer_PiccBlockWrite = new byte[6];
        public static byte[] cardID = new byte[4];//定义一个接收防碰撞返回的UID的字节数据（4 字节）

        //计算BCC码函数
        public static byte bcc_Calc(byte[] cmd)
        {
            byte BCC = 0;//最后计算得到的BCC码
            byte temp = 0;//存放异或之后的结果
            for (int i = 0; i < (cmd.Length - 2); i++)
            {
                temp ^= cmd[i];//按位异或
            }
            BCC = (byte)~temp;//按位取反
            return BCC;
        }

        public static bool cmdCom(byte[] cmd, byte[] buffer_Cmd)
        {
            sp.Write(cmd, 0, cmd.Length);//主机发送读设备信息指令，其实就是往串口写数据
            Thread.Sleep(300);
            sp.Read(buffer_Cmd, 0, buffer_Cmd.Length);//接收从机应答的数据
            //判断从机应答状态是否为0和帧结束符是否为3
            if (buffer_Cmd[2] == 0x00 && buffer_Cmd[buffer_Cmd.Length - 1] == 0x03)
            {
                if (cmd[1] == 0x02)
                {
                    switch (cmd[2])
                    {
                        case 0x42:
                            {
                                for (int i = 0; i < cardID.Length; i++)
                                {
                                    cardID[i] = buffer_Cmd[i + 4]; //获取从机应答的数据信息中的卡ID
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                return true;
            }
            else
                return false;
        }
        //返回卡ID
        public static byte[] getCardID()
        {
            return cardID;
        }
        public static string showCardID()
        {
            //十进制转为十六进制，如果高位没有值，就用0代替
            return Convert.ToString(cardID[0], 16).PadLeft(2, '0') + " "
                    + Convert.ToString(cardID[1], 16).PadLeft(2, '0') + " "
                    + Convert.ToString(cardID[2], 16).PadLeft(2, '0') + " "
                    + Convert.ToString(cardID[3], 16).PadLeft(2, '0');
        }

        public static string showReadCardInfo()
        {
            string show_ReadCardInfo = "";
            for (int i = 0; i < 16; i++)
            {
                show_ReadCardInfo += (Convert.ToString(PiccWrite_Block04[i + 5], 16).PadLeft(2, '0') + " ").ToString();
            }
            //十进制转为十六进制，如果高位没有值，就用0代替
            return show_ReadCardInfo;
        }
    }
}
