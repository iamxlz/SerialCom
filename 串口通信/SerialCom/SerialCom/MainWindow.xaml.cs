using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;//引入串口操作的命名空间
using System.Threading;//引入线程的命名空间
//using SerialCom;//引入数据库文件所在的命名空间

namespace SerialCom
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        HWCMS hwcms_Window = new HWCMS();

        public MainWindow()
        {
            //使窗体弹出时在屏幕居中位置
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            InitAccessControlClose();
            InitComponent();//调用初始化串口函数
            InintEnabledFalse();//调用初始化按钮灰化处理
        }

        public void InintSyntax()
        {
            /*
            “增”语法：INSERT INTO 表名(字段名1, 字段名2, …) VALUES（值1, 值2, …);
            “删”语法：DELETE FROM 表名 WHERE [条件表达式]
            “改”语法：UPDATE 表名 SET 字段名1 = 值1[, 字段名2 = 值2, …] [WHERE 条件表达式]
            */
        }

        public void InitAccessControlClose()
        {
            BitmapImage image;
            //该路径对应项目的bin/debug文件所在的路径
            string path = System.Environment.CurrentDirectory;
            path = path + "/images/门.jpg";
            //实例化位图对象，把获取的图片与实例化的位图对象相关联
            image = new BitmapImage(new Uri(path, UriKind.Absolute));
            img_AccessControl.Source = image;//显示图片到img_AccessControl中
        }

        public void InitAccessControlOpen()
        {
            BitmapImage image;
            //该路径对应项目的bin/debug文件所在的路径
            string path = System.Environment.CurrentDirectory;
            path = path + "/images/开门.jpg";
            //实例化位图对象，把获取的图片与实例化的位图对象相关联
            image = new BitmapImage(new Uri(path, UriKind.Absolute));
            img_AccessControl.Source = image;//显示图片
        }

        //自定义初始化按钮灰化处理为True
        public void InintEnabledTrue()
        {
            btn_CloseSerial.IsEnabled = true;
            btn_SendCmd01.IsEnabled = true;
            btn_Request.IsEnabled = true;
            btn_CascAnticoll.IsEnabled = true;
            btn_CascSelect.IsEnabled = true;
            btn_Halt.IsEnabled = true;
            btn_AuthKey.IsEnabled = true;
            btn_Read.IsEnabled = true;
            btn_Write.IsEnabled = true;
            btn_Add.IsEnabled = true;
            btn_Sub.IsEnabled = true;
            btn_BlockRead.IsEnabled = true;
            btn_BlockWrite.IsEnabled = true;
        }

        //自定义初始化按钮灰化处理为False
        public void InintEnabledFalse()
        {
            btn_CloseSerial.IsEnabled = false;
            btn_SendCmd01.IsEnabled = false;
            btn_Request.IsEnabled = false;
            btn_CascAnticoll.IsEnabled = false;
            btn_CascSelect.IsEnabled = false;
            btn_Halt.IsEnabled = false;
            btn_AuthKey.IsEnabled = false;
            btn_Read.IsEnabled = false;
            btn_Write.IsEnabled = false;
            btn_Add.IsEnabled = false;
            btn_Sub.IsEnabled = false;
            btn_BlockRead.IsEnabled = false;
            btn_BlockWrite.IsEnabled = false;
        }

        //自定义初始化串口函数
        private void InitComponent()
        {
            cbBox_Serial.Items.Clear();//清空下拉框初始化的值
            for (int i = 0; i < RFIDHelper.comNumber.Length; i++)
            {
                cbBox_Serial.Items.Add(RFIDHelper.comNumber[i]);//添加可用串口到下拉框中
            }
            cbBox_Serial.SelectedIndex = 0;//让串口操作部分下拉框默认选择第一项
            cbBox_Cmd01.SelectedIndex = 0;//让控制类命令部分下拉框默认选择第一项
        }

        //从tb_SecretKey中获取密钥数据
        private void gettb_SecretKey(byte[] Cmd)
        {
            string[] temp = tb_SecretKey.Text.Split(' ');
            byte[] secret_Key = new byte[temp.Length];
            for (int i = 0; i < temp.Length; i++)
                //字符串数组转换为字节数组，十六进制转为十进制
                secret_Key[i] = byte.Parse(temp[i], System.Globalization.NumberStyles.HexNumber);
            //密钥
            for (int i = 9, j = 0; j < secret_Key.Length; i++, j++)
                Cmd[i] = secret_Key[j];
        }

        private void gettb_Write(byte[] Cmd)
        {
            //从TextBox中获取16个字节数据
            string[] temp = tb_Write.Text.Split(' ');
            byte[] values = new byte[temp.Length];
            for (int i = 0; i < temp.Length; i++)
                //字符串数组转换为字节数组，十六进制转为十进制
                values[i] = byte.Parse(temp[i], System.Globalization.NumberStyles.HexNumber);
            //值
            for (int i = 5, j = 0; j < values.Length; i++, j++)
                Cmd[i] = values[j];
        }

        private void btn_openChuanKou_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /* 串口属性的配置*/
                RFIDHelper.sp.PortName = cbBox_Serial.Text;//设置串口名称为下拉框所显示的文本
                RFIDHelper.sp.Parity = 0;//无奇偶校验位
                RFIDHelper.sp.DataBits = 8;//8个数据位
                RFIDHelper.sp.StopBits = System.IO.Ports.StopBits.One;//1个停止位
                RFIDHelper.sp.WriteTimeout = 2000;//设置系统超时时间
                RFIDHelper.sp.WriteBufferSize = 1024;//设置缓冲区的大小
                RFIDHelper.sp.ReadBufferSize = 1024;

                RFIDHelper.sp.Open();//打开串口
                tb_Hint.Text = "串口打开成功！";
                InintEnabledTrue();//打开按钮灰化处理
                btn_OpenSerial.IsEnabled = false;//关闭串口按钮灰化处理
            }
            catch (Exception)
            {
                string str = "Set the serial port failed!";
                MessageBox.Show(str, "串口通信");
            }
        }

        private void btn_closeChuanKou_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RFIDHelper.sp.Close();//关闭串口
                InintEnabledFalse();//关闭按钮灰化处理
                btn_OpenSerial.IsEnabled = true;//使能打开串口按钮
                tb_Hint.Text = "关闭串口成功！";
                InitAccessControlClose();
            }
            catch (Exception ex)
            {
                tb_Hint.Text = ex.Message;
            }
        }

        private void btn_SendCmd01_Click(object sender, RoutedEventArgs e)
        {
            bool status = false;
            switch (cbBox_Cmd01.SelectedIndex)//判断下拉框选择的是哪个命令
            {
                case 0:
                    {
                        RFIDHelper.GetDvcInfo[RFIDHelper.GetDvcInfo.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.GetDvcInfo);
                        status = RFIDHelper.cmdCom(RFIDHelper.GetDvcInfo, RFIDHelper.buffer_GetDvcInfo);
                        if (status)
                        {
                            tb_Hint.Text = "读设备信息成功！";
                        }
                        else
                        {
                            tb_Hint.Text = "读设备信息失败！";
                        }
                    }
                    break;
                case 1:
                    {
                        RFIDHelper.PCDConfig[RFIDHelper.PCDConfig.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.PCDConfig);
                        status = RFIDHelper.cmdCom(RFIDHelper.PCDConfig, RFIDHelper.buffer_PCDConfig);
                        if (status)
                        {
                            tb_Hint.Text = "配置读卡芯片成功！";
                        }
                        else
                        {
                            tb_Hint.Text = "配置读卡芯片失败！";
                        }

                    }
                    break;
                case 2:
                    {
                        RFIDHelper.PCDClose[RFIDHelper.PCDClose.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.PCDClose);
                        status = RFIDHelper.cmdCom(RFIDHelper.PCDClose, RFIDHelper.buffer_PCDClose);
                        if (status)
                        {
                            tb_Hint.Text = "关闭读卡芯片成功！";
                        }
                        else
                        {
                            tb_Hint.Text = "关闭读卡芯片失败！";
                        }
                    }
                    break;
            }
        }

        private void btn_Request_Click(object sender, RoutedEventArgs e)
        {
            bool status = false;
            RFIDHelper.PiccRequest[RFIDHelper.PiccRequest.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.PiccRequest);
            status = RFIDHelper.cmdCom(RFIDHelper.PiccRequest, RFIDHelper.buffer_PiccRequest);
            if (status)
            {
                tb_Hint.Text = "请求成功！";
            }
            else
            {
                tb_Hint.Text = "请求失败！";
            }
        }

        private void btn_CascAnticoll_Click(object sender, RoutedEventArgs e)
        {
            bool status = false;
            RFIDHelper.PiccAnticoll[RFIDHelper.PiccAnticoll.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.PiccAnticoll);
            status = RFIDHelper.cmdCom(RFIDHelper.PiccAnticoll, RFIDHelper.buffer_PiccAnticoll);
            if (status)
            {
                tb_Hint.Text = " 防碰撞成功！\n" + "卡ID：" + RFIDHelper.showCardID();
            }
            else
            {
                tb_Hint.Text = "设备没应答！";
            }
        }

        private void btn_CascSelect_Click(object sender, RoutedEventArgs e)
        {
            bool status = false;
            //把前一个防碰撞命令返回的UID（包含级联志）赋值给选择主机命令的UID（4 字节）
            for (int i = 0; i < RFIDHelper.cardID.Length; i++)
                RFIDHelper.PiccSelect[5 + i] = RFIDHelper.cardID[i];
            //把计算好的BCC码赋值给选择字节数组
            RFIDHelper.PiccSelect[RFIDHelper.PiccSelect.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.PiccSelect);
            status = RFIDHelper.cmdCom(RFIDHelper.PiccSelect, RFIDHelper.buffer_PiccSelect);
            //判断从机应答状态是否为0和帧结束符是否为3
            if (status)
            {
                //string temp = "SELECT * FROM authorization";
                //StringBuilder temp2 = DataBaseHelper.QueryData(temp);
                //string[] temp3 = temp2.ToString().Split(' ');
                //for (int i = 0; i < temp3.Length; i++)
                //{
                //    //字符串数组转换为字节数组，十六进制转为十进制
                //    MacthingCardID[i] = byte.Parse(temp3[i]);
                //}
                //Enumerable.SequenceEqual()
                //把卡ID显示出来（4 字节，低字节在先）,十进制转为十六进制,如果高位没有值，就用0代替
                tb_Hint.Text = "选择成功！\n" + "卡ID：" + RFIDHelper.showCardID();

            }
            else
            {
                tb_Hint.Text = "选择失败！";
            }
        }

        private void btn_Halt_Click(object sender, RoutedEventArgs e)
        {
            bool status = false;
            RFIDHelper.PiccHalt[RFIDHelper.PiccHalt.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.PiccHalt);
            status = RFIDHelper.cmdCom(RFIDHelper.PiccHalt, RFIDHelper.buffer_PiccHalt);
            if (status)
            {
                tb_Hint.Text = "暂停成功，操作需重新激活！";
            }
            else
            {
                tb_Hint.Text = "暂停失败！";
            }
        }

        private void btn_AuthKey_Click(object sender, RoutedEventArgs e)
        {
            bool status = false;
            byte block_Num;
            string str = tb_BlockAddress.Text.ToString().Trim();//从tb_BlockAddress读数据
            if (str.Contains('.') || str == "")
            {
                MessageBox.Show("不能输入小数或输入为空");
                return;
            }
            else
            {
                block_Num = Convert.ToByte(str);
                if (block_Num >= 0 && block_Num <= 63)
                {
                    RFIDHelper.PiccAuthKey[RFIDHelper.PiccAuthKey.Length - 3] = block_Num;
                }
                else
                {
                    MessageBox.Show("请输入0-63之间的正整数");
                    return;
                }
            }
            if (rb_SecretKeyA.IsChecked == true)
            {
                RFIDHelper.PiccAuthKey[4] = 0x60;
            }
            else
            {
                RFIDHelper.PiccAuthKey[4] = 0x61;
            }

            //卡序列号
            for (int i = 0; i < RFIDHelper.cardID.Length; i++)
                RFIDHelper.PiccAuthKey[5 + i] = RFIDHelper.cardID[i];

            //从tb_SecretKey中获取密钥
            gettb_SecretKey(RFIDHelper.PiccAuthKey);

            RFIDHelper.PiccAuthKey[RFIDHelper.PiccAuthKey.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.PiccAuthKey);
            status = RFIDHelper.cmdCom(RFIDHelper.PiccAuthKey, RFIDHelper.buffer_PiccAuthKey);
            if (status)
            {

                hwcms_Window.Show();//开启热水卡系统
                this.Hide();//隐藏门禁系统
                if (rb_SecretKeyA.IsChecked == true)
                {
                    tb_Hint.Text = "密钥A验证成功！";
                }
                else
                {
                    tb_Hint.Text = "密钥B验证成功！";
                }

            }
            else
            {
                tb_Hint.Text = "验证失败！";
            }
        }

        private void btn_Read_Click(object sender, RoutedEventArgs e)
        {
            //bool status = false;
            //RFIDHelper.PiccRead[RFIDHelper.PiccRead.Length - 3] = RFIDHelper.PiccAuthKey[RFIDHelper.PiccAuthKey.Length - 3];
            //RFIDHelper.PiccRead[RFIDHelper.PiccRead.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.PiccRead);
            //status = RFIDHelper.cmdCom(RFIDHelper.PiccRead, RFIDHelper.buffer_PiccRead);
            //if (status)
            //{
            //    tb_Hint.Text = "读卡成功：" + RFIDHelper.showReadCardInfo();
            //}
            //else
            //{
            //    tb_Hint.Text = "读卡失败！";
            //}
        }

        private void btn_Write_Click(object sender, RoutedEventArgs e)
        {
            //判断数据是否等于16 字节
            //if (tb_Write.Text.Split(' ').Length == 16)
            //{
            //    bool status = false;
            //    RFIDHelper.PiccWrite_Block04[4] = RFIDHelper.PiccAuthKey[RFIDHelper.PiccAuthKey.Length - 3];
            //    gettb_Write(RFIDHelper.PiccWrite_Block04);
            //    RFIDHelper.PiccWrite_Block04[RFIDHelper.PiccWrite_Block04.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.PiccWrite_Block04);
            //    status = RFIDHelper.cmdCom(RFIDHelper.PiccWrite_Block04, RFIDHelper.buffer_PiccWriteBlock04);
            //    if (status)
            //    {
            //        tb_Hint.Text = "写卡成功！";
            //    }
            //    else
            //    {
            //        tb_Hint.Text = "写卡失败！";
            //    }
            //}
            //else
            //{
            //    tb_Hint.Text = "写数据要等于16 字节！";
            //}
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            //sp.Write(PiccAddValue, 0, PiccAddValue.Length);//主机发送读设备信息指令，其实就是往串口写数据
            //Thread.Sleep(300);
            //sp.Read(buffer_PiccAddValue, 0, buffer_PiccAddValue.Length);//接收从机应答的数据

            ////判断从机应答状态是否为0和帧结束符是否为3
            //if (buffer_PiccAddValue[2] == 0x00 && buffer_PiccAddValue[5] == 0x03)
            //{
            //    tb_Hint.Text = "加值成功！";
            //}
            //else
            //{
            //    tb_Hint.Text = "加值失败！";
            //}
        }

        private void btn_Sub_Click(object sender, RoutedEventArgs e)
        {
            //sp.Write(PiccSubValue, 0, PiccSubValue.Length);//主机发送读设备信息指令，其实就是往串口写数据
            //Thread.Sleep(300);
            //sp.Read(buffer_PiccSubValue, 0, buffer_PiccSubValue.Length);//接收从机应答的数据

            ////判断从机应答状态是否为0和帧结束符是否为3
            //if (buffer_PiccSubValue[2] == 0x00 && buffer_PiccSubValue[5] == 0x03)
            //{
            //    tb_Hint.Text = "减值成功";
            //}
            //else
            //{
            //    tb_Hint.Text = "减值失败！";
            //}
        }
        private void btn_BlockRead_Click(object sender, RoutedEventArgs e)
        {
            //if (rb_SecretKeyA.IsChecked == true)
            //{
            //    PiccBlockRead[7] = 0x60;
            //}
            //else
            //{
            //    PiccBlockRead[7] = 0x61;
            //}

            //gettb_SecretKey(PiccBlockRead);
            //sp.Write(PiccBlockRead, 0, PiccBlockRead.Length);//主机发送读设备信息指令，其实就是往串口写数据
            //Thread.Sleep(300);
            //sp.Read(buffer_PiccBlockRead, 0, buffer_PiccBlockRead.Length);//接收从机应答的数据

            ////判断从机应答状态是否为0和帧结束符是否为3
            //if (buffer_PiccBlockRead[2] == 0x00 && buffer_PiccBlockRead[21] == 0x03)
            //{
            //    tb_Hint.Text = "单指令读成功：" + Convert.ToString(PiccBlockWrite[13], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[14], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[15], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[16], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[17], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[18], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[19], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[20], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[21], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[22], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[23], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[24], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[25], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[26], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[27], 16).PadLeft(2, '0') + " "
            //        + Convert.ToString(PiccBlockWrite[28], 16).PadLeft(2, '0');
            //}
            //else
            //{
            //    tb_Hint.Text = "单指令读失败";
            //}
        }

        private void btn_BlockWrite_Click(object sender, RoutedEventArgs e)
        {
            //if (tb_Write.Text.Split(' ').Length == 16)
            //{
            //    if (rb_SecretKeyA.IsChecked == true)
            //    {
            //        PiccBlockWrite[6] = 0x60;
            //    }
            //    else
            //    {
            //        PiccBlockWrite[6] = 0x61;
            //    }
            //    gettb_SecretKey(PiccBlockWrite);
            //    gettb_Write(PiccBlockWrite);
            //    PiccBlockWrite[29] = bcc_Calc(PiccBlockWrite);
            //    sp.Write(PiccBlockWrite, 0, PiccBlockWrite.Length);//主机发送读设备信息指令，其实就是往串口写数据
            //    Thread.Sleep(300);
            //    sp.Read(buffer_PiccBlockWrite, 0, buffer_PiccBlockWrite.Length);//接收从机应答的数据

            //    //判断从机应答状态是否为0和帧结束符是否为3
            //    if (buffer_PiccBlockWrite[2] == 0x00 && buffer_PiccBlockWrite[5] == 0x03)
            //    {
            //        tb_Hint.Text = "单指令写成功！";
            //    }
            //    else
            //    {
            //        tb_Hint.Text = "单指令写失败！";
            //    }
            //}
            //else
            //{
            //    tb_Hint.Text = "单指令写数据要等于16 字节！";
            //}
        }

        private void lb_Authorization_Click(object sender, RoutedEventArgs e)
        {
            //    string sqlStr = "INSERT INTO authorization(card_id) VALUES(" + CardID[0] + CardID[1] + CardID[2] + CardID[3] + ")";
            //    DataBaseHelper.GetYingXiangHang(sqlStr);
            //}
        }
    }
}
