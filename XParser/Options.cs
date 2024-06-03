using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace XParser
{
    static class Options
    {
        public static List<int> endpoints = new List<int>() { 24, 30 };
        public static int point = 10000;
        public static int ingoing_file_length = 30;
    }
}
