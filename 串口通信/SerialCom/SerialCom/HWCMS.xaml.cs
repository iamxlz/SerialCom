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
using System.Windows.Shapes;

namespace SerialCom
{
    /// <summary>
    /// HWCMS.xaml 的交互逻辑
    /// </summary>
    public partial class HWCMS : Window
    {
        public HWCMS()
        {
            InitializeComponent();
        }
        private void btn_ActivateCard_Click(object sender, RoutedEventArgs e)
        {            
            bool piccWrite_block04_Status = false;
            bool piccWrite_block05_Status = false;

            RFIDHelper.PiccWrite_Block04[4] = 0x20;//写卡块号04数据

            int money = Convert.ToInt32(tb_Money.Text.Trim());
            RFIDHelper.PiccWrite_Block04[5] = (byte)money;
            RFIDHelper.PiccWrite_Block04[6] = (byte)(money >> 8);

            string student_NumberStr = tb_StudentNumber.Text.Trim();
            byte[] student_Number = new byte[student_NumberStr.Length];
            for (int i = 0; i < student_NumberStr.Length; i++)
            {
                //通过字符串拆分的方法，把字符串的值一个一个赋值到数组中，也可以把字符串中的一个或者多个值,拆分为一个值，赋值到数组中
                student_Number[i] = Convert.ToByte(student_NumberStr.Substring(i, 1));
                RFIDHelper.PiccWrite_Block04[i + 7] = student_Number[i]; //10字节存放学号
            }
            

            RFIDHelper.PiccWrite_Block04[RFIDHelper.PiccWrite_Block04.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.PiccWrite_Block04);
            piccWrite_block04_Status = RFIDHelper.cmdCom(RFIDHelper.PiccWrite_Block04, RFIDHelper.buffer_PiccWrite_Block04);

            RFIDHelper.PiccWrite_Block05[4] = 0x21;//写卡块号05数据

            byte[] sex = Encoding.UTF8.GetBytes(tb_Six.Text.Trim());
            for (int i = 0; i < sex.Length; i++)
            {
                RFIDHelper.PiccWrite_Block05[i + 5] = sex[i];
            }

            byte[] name = Encoding.UTF8.GetBytes(tb_Name.Text.Trim());
            for (int i = 0; i < name.Length; i++)
            {
                RFIDHelper.PiccWrite_Block05[i + 8] = name[i];
            }

            RFIDHelper.PiccWrite_Block05[RFIDHelper.PiccWrite_Block05.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.PiccWrite_Block05);
            piccWrite_block05_Status = RFIDHelper.cmdCom(RFIDHelper.PiccWrite_Block05, RFIDHelper.buffer_PiccWrite_Block05);

            if (piccWrite_block04_Status && piccWrite_block05_Status)
            {
                MessageBox.Show("开卡成功！", "提示");
            }
            else
            {
                MessageBox.Show("开卡失败！", "提示");
            }
        }

        private void btn_Query_Click(object sender, RoutedEventArgs e)
        {
            bool piccRead_Block04_Status = false;
            bool piccRead_Block05_Status = false;

            RFIDHelper.PiccRead_Block04[RFIDHelper.PiccRead_Block04.Length - 3] = 0x016;
            RFIDHelper.PiccRead_Block04[RFIDHelper.PiccRead_Block04.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.PiccRead_Block04);
            piccRead_Block04_Status = RFIDHelper.cmdCom(RFIDHelper.PiccRead_Block04, RFIDHelper.buffer_PiccRead_Block04);

            RFIDHelper.PiccRead_Block05[RFIDHelper.PiccRead_Block05.Length - 3] = 0x017;
            RFIDHelper.PiccRead_Block05[RFIDHelper.PiccRead_Block05.Length - 2] = RFIDHelper.bcc_Calc(RFIDHelper.PiccRead_Block05);
            piccRead_Block05_Status = RFIDHelper.cmdCom(RFIDHelper.PiccRead_Block05, RFIDHelper.buffer_PiccRead_Block05);

            if (piccRead_Block04_Status && piccRead_Block05_Status)
            {
                string show_StudentNumber = "";
                for (int i = 0; i < 10; i++)
                {
                    show_StudentNumber += RFIDHelper.buffer_PiccRead_Block04[i + 6].ToString();
                }
                lb_ShowStudentNumber.Content = "学号：" + show_StudentNumber;
                lb_ShowMoney.Content = "金额：" + Convert.ToString(RFIDHelper.buffer_PiccRead_Block04[4] +
                    RFIDHelper.buffer_PiccRead_Block04[5] * 256);
                try
                {
                    byte[] show_Name = new byte[12];
                    for (int i = 0; i < show_Name.Length; i++)
                    {
                        show_Name[i] = RFIDHelper.buffer_PiccRead_Block04[i + 7];
                    }
                    lb_ShowName.Content = "姓名：" + Encoding.UTF8.GetString(show_Name);
                    byte[] show_Sex = new byte[3];
                    for (int i = 0; i < show_Sex.Length; i++)
                    {
                        show_Sex[i] = RFIDHelper.buffer_PiccRead_Block04[i + 4];
                    }
                    lb_ShowSex.Content = "性别：" + Encoding.UTF8.GetString(show_Sex);
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("数组名不能为空！", "错误");
                }
            }
            else
            {
                MessageBox.Show("查询失败！", "提示");
            }
        }
    }
}
