using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(value: "Проверка слайс");
            double[] t = {1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var res = t.Batch(size: 3).ToString();
            Console.WriteLine("result ", res);
            Console.ReadKey();
        }

    }
}
