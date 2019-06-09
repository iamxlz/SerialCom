using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;//添加数据库操作的相关应用
using MySql.Data.MySqlClient;
using System.Configuration;

namespace SerialCom
{
    class DataBaseHelper
    {
        //对数据库连接的方法
        public static MySqlConnection getMySqlConnection()
        {
            //数据库连接
            MySqlConnection conn = new MySqlConnection();
            try
            {
                conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["connString"];
                conn.Open();
                conn.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return conn;
        }

        //数据库的删除、增加、修改、更新操作的函数
        public static int GetYingXiangHang(string sqlStr)
        {
            int num = 0;//执行增删查改后受影响的行数
            MySqlConnection conn = getMySqlConnection();//创建并配置MySqlConnection对象
            try
            {
                conn.Open();//打开数据库
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);//创建MySqlCommand对象
                num = cmd.ExecuteNonQuery();//执行对应的操作
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();//关闭数据库
                conn.Dispose();//释放数据库所占用资源
            }
            return num;
        }

        //对数据库进行查询操作的函数
        public static StringBuilder QueryData(string sqlStr)
        {
            StringBuilder stb = null;
            MySqlConnection conn = getMySqlConnection();//创建配置connection对象
            try
            {
                conn.Open();//打开数据库
                MySqlDataAdapter da = new MySqlDataAdapter(sqlStr, conn);//创建DataAdapter
                DataSet ds = new DataSet();//创建DataSet
                da.Fill(ds);//把数据从数据库填充到内存数据库dataset中
                stb = new StringBuilder();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    for (int i = 0; i < dr.ItemArray.Length; i++)
                    {
                        stb.Append(dr[i].ToString() + "");
                    }
                }
            }
            catch (Exception)
            {
                //throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();//关闭数据库
                conn.Dispose();//释放数据库所占用资源
            }
            return stb;
        }
    }
}
