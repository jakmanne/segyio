using System;
using System.Text;
using dotnet;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace testprosjekt
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Segyio pointer = Segyio.Open("C:/Users/Jakob/segyio/test-data/small.sgy", "r");  
            
            float[] traces = pointer.Readtrace(0);
            Dictionary<string, int> dict = pointer.Gettraceheader(1);

            for (int i = 0; i< traces.Length; i++)
            {
                Console.WriteLine(traces[i]);
            }

               
             foreach (KeyValuePair<string, int> kvp in dict)
             {
                Console.WriteLine(kvp.Key + "  " + kvp.Value);
             }
        }
    }
}
