using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;

namespace XParser
{
    internal class Parser
    {
        private string incoming_file;

        public Parser(string income)
        {
            incoming_file = income;
        }


        public int Parse()
        {

            List<string> codes = new List<string>();
            Int32 count = 1;
            List<string> buffer = new List<string> (); 
            int index = 0;

            for (int i = 1; i < Options.endpoints.Count + 1; i++)
            {
                if (!Directory.Exists(Path.GetFileNameWithoutExtension($"output\\{Path.GetFileNameWithoutExtension(incoming_file)}"))) 
                    { Directory.CreateDirectory($"output\\{Path.GetFileNameWithoutExtension(incoming_file)}"); }

                string dir = Path.GetFileNameWithoutExtension(incoming_file);

                StreamReader reader = new StreamReader(incoming_file);
                count = 0;
                index = 0;

                try
                {
                    using (reader)
                    {
                        string line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            string output = new string(line.Where(c => !char.IsControl(c)).ToArray()).Substring(0, Options.endpoints[i - 1]);

                            if (count > Options.point - 1)
                            {
                                index++;
                                StreamWriter new_writer = new StreamWriter($"output\\{dir}\\{Path.GetFileNameWithoutExtension(incoming_file)}_{Options.endpoints[i - 1]}_part{index}.txt");

                                using (new_writer) { foreach (string data in buffer) { new_writer.WriteLine($"{data}#"); }; buffer.Clear(); }

                                new_writer.Close();
                                new_writer.Dispose();

                                buffer.Add(output);
                                count = 0;

                            }

                            else { buffer.Add(output); }

                            count++;

                        }
                    }
                        

                        if (count <= Options.point)
                        {
                            index++;
                        StreamWriter new_writer = new StreamWriter($"output\\{dir}\\{Path.GetFileNameWithoutExtension(incoming_file)}_{Options.endpoints[i - 1]}_part{index}.txt");
                        using (new_writer) { foreach (string data in buffer) { new_writer.WriteLine($"{data}#"); }; buffer.Clear(); }
                        }

                    


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    reader.Close();
                    reader.Dispose();

                    return -1;
                }
            }

            return 0;
        }
    }
}
