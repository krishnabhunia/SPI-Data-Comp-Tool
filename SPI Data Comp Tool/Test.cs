using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    //public class A
    //{
    //    public void Disp()
    //    {

    //    }

    //    public void TestA()
    //    {

    //    }
    //}

    //public class B:A
    //{
    //    public void Disp()
    //    {

    //    }

    //    public void TestB()
    //    {

    //    }
    //}

    //public class C
    //{
    //    public void Ru()
    //    {
    //        A a = new A();
    //        B b = new B();

    //        A b1 = new B();
    //        b1.TestA();

    //    }

    //}

    public partial class Test : Form
    {

        private Common_Data common_Data;
        public Test()
        {
            InitializeComponent();

            common_Data = new Common_Data();
        }

        private void btn_Test_Click(object sender, EventArgs e)
        {
            //Common_Data.DisplayMessage(rtb_Log, "Start");
            //Enumerable.Range(-10, 20).Where(x => isPrime(x)).ToList().ForEach(x => Common_Data.DisplayMessage(rtb_Log, x.ToString()));
            //Common_Data.DisplayMessage(rtb_Log, "End");

            List<string> foods = new List<string> { "apple", "banana", "cherry", "honey" };

            List<int> foodint = new List<int> { 2, 3, 1, 4 };

            (from xyz in foodint.Zip(foods, (first, second) => first + " " + second)
                      orderby xyz descending
                      select xyz).ToList().ForEach(x => Common_Data.DisplayMessage(rtb_Log, x.ToString()));

        }

        bool isPrime(int num)
        {
            int c = 2;

            if (num < 2)
                return false;

            while(num % c != 0)
            {
                ++c;
            }

            if (num == c)
                return true;
            else
                return false;

        }

        private void btn_Clear_Reset_Click(object sender, EventArgs e)
        {
            Common_Data.ClearDisplay(rtb_Log);
        }

        private void btn_Intersect_Click(object sender, EventArgs e)
        {
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            DBConnectionStatus dBConnectionStatus = new DBConnectionStatus();

            dBConnectionStatus = userControlDataBase1.GetConnectionDetailsForSource();
            dBConnectionStatus.strCMDText = "SELECT * FROM A1";
            dt1 = common_Data.FillDataTable(dt1, userControlDataBase1.GetConnectionDetailsForSource());
            dt1.TableName = dBConnectionStatus.strCMDText;

            dBConnectionStatus = userControlDataBase1.GetConnectionDetailsForTarget();
            dBConnectionStatus.strCMDText = "SELECT * FROM A2";
            common_Data.FillDataTable(dt2, userControlDataBase1.GetConnectionDetailsForTarget());
            dt2.TableName = dBConnectionStatus.strCMDText;

            IEnumerable<DataRow> result1 = dt1.AsEnumerable().ToList().Intersect(dt2.AsEnumerable().ToList());

            foreach(DataRow dataRow in result1)
            {
                Common_Data.DisplayMessage(rtb_Log, dataRow.ToString());
            }

            IList<string> strList1 = new List<string>() { "One", "Two", "Three", "Four", "Five" };
            IList<string> strList2 = new List<string>() { "Four", "Five", "Six", "Seven", "Eight" };

            var result = strList1.Intersect(strList2);

            foreach (string str in result)
                Common_Data.DisplayMessage(rtb_Log, str);

        }
    }
}
