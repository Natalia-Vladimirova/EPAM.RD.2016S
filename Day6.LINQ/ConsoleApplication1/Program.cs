using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    delegate void ExampleDel(int param);
    delegate void ExampleDel2(string param);

    class Program
    {
        static void Main(string[] args)
        {
            KeyValuePair<int, int> ii = new KeyValuePair<int, int>(5, 30);
            KeyValuePair<int, int> jj = new KeyValuePair<int, int>(5, 30);
            Console.WriteLine($"{ii.GetHashCode()} {jj.GetHashCode()}");

            KeyValuePair<int, int> iii = new KeyValuePair<int, int>(5, 30);
            KeyValuePair<int, int> jjj = new KeyValuePair<int, int>(5, 35);
            Console.WriteLine($"{iii.GetHashCode()} {jjj.GetHashCode()}");

            KeyValuePair<int, int> iiii = new KeyValuePair<int, int>(5, 30);
            KeyValuePair<int, int> jjjj = new KeyValuePair<int, int>(6, 30);
            Console.WriteLine($"{iiii.GetHashCode()} {jjjj.GetHashCode()}");

            KeyValuePair<int, string> ss = new KeyValuePair<int, string>(5, "test");
            KeyValuePair<int, string> sss = new KeyValuePair<int, string>(5, "testt");
            Console.WriteLine($"{ss.GetHashCode()} {sss.GetHashCode()}");

            KeyValuePair<string, int> qq = new KeyValuePair<string, int>("test", 5);
            KeyValuePair<string, int> qqq = new KeyValuePair<string, int>("testt", 5);
            Console.WriteLine($"{qq.GetHashCode()} {qqq.GetHashCode()}");

            ExampleDel del1 = (x) => { };
            ExampleDel del2 = (x) => { };
            ExampleDel del3 = (x) => { };
            del3 += (x) => { };
            ExampleDel2 del4 = (x) => { };
            Console.WriteLine($"{del1.GetHashCode()} {del2.GetHashCode()} {del3.GetHashCode()} {del4.GetHashCode()}");

            string str1 = "test";
            string str2 = "test";
            Console.WriteLine($"{str1.Equals(str2)} {str1 == str2}");

            Console.WriteLine($"{str1.IndexOf("e", StringComparison.InvariantCulture)}");

            string str = "";
            DateTime dtStart = DateTime.Now;
            for (int i = 0; i < 100000; i++)
            {
                str += "e";
            }
            DateTime dtEnd = DateTime.Now;
            Console.WriteLine($"str {dtEnd - dtStart}");

            StringBuilder sb = new StringBuilder();
            dtStart = DateTime.Now;
            for (int i = 0; i < 100000; i++)
            {
                sb.Append("e");
            }
            dtEnd = DateTime.Now;
            Console.WriteLine($"sb {dtEnd - dtStart}");

            Console.ReadLine();
        }
    }
}
