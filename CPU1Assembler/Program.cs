using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GSVM.Components.Processors.CPU_1.Assembler;

namespace CPU1Assembler
{
    class Program
    {
        static Preprocessor preprocessor;
        static Assembler assembler;

        static void Main(string[] args)
        {
            preprocessor = new Preprocessor();

            string output = args[0];
            string input = args[1];

            string[] code = File.ReadAllLines(input);

            try
            {
                preprocessor.AddCode(code);
                code = preprocessor.ProcessCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            File.WriteAllLines("prep_" + output + "_.asm", code);

            try
            {
                assembler = new Assembler(preprocessor.Pragmas);
                assembler.AddSource(code);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            try
            {
                assembler.Assemble();
            }
            catch (AssemblyException ex)
            {
                Console.WriteLine("{0}: {1} - {2}", ex.LineNumber, ex.Message, ex.Code);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            File.WriteAllBytes(output, assembler.Binary);
            Console.WriteLine("{0} bytes written", assembler.Length);

            Console.ReadLine();
        }
    }
}
