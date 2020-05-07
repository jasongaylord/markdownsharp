using System;
using System.Diagnostics;
using System.IO;

namespace MarkdownSharpTests
{
    public class Program
    {
        /// <summary>
        /// mini test harness for one-liner Markdown bug repros 
        /// for anything larger, I recommend using the folder based approach and Test()
        /// </summary>
        private static void AdHocTest()
        {
            var m = new MarkdownSharp.Markdown();
            //var m = new MarkdownSharp.MarkdownOld();

            //string input = "<div class=\"inlinepage\">\n<div class=\"toggleableend\">\nfoo\n</div>\n</div>";
            //string input = "Same thing but with paragraphs:\n\n1. First\n\n2. Second:\n\t* Fee\n\t* Fie\n\t* Foe\n\n3. Third\n\n";
            //string input = "*\tthis\n\n\t*\tsub\n\n\tthat";
            //string input = "1. one\n\n        code<t>\n\n2. two\n\n        code<t>\n            indented-12-spaces<t>\n\n";
            string input = "\n\n    code<t>\n";

            string output = m.Transform(input);

            Console.WriteLine("input:");
            Console.WriteLine(input);
            Console.WriteLine("output:");
            Console.WriteLine(output);
        }


        /// <summary>
        /// iterates through all the test files in a given folder and generates file-based output 
        /// this is essentially the same as running the unit tests, but with diff-able results
        /// </summary>
        /// <remarks>
        /// two files should be present for each test:
        /// 
        /// test_name.text         -- input (raw markdown)
        /// test_name.html         -- output (expected cooked html output from reference markdown engine)
        /// 
        /// this file will be generated if, and ONLY IF, the expected output does not match the actual output:
        /// 
        /// test_name.xxxx.actual.html  -- actual output (actual cooked html output from our markdown c# engine)
        ///                             -- xxxx is the 16-bit CRC checksum of the file contents; this is included
        ///                                so you can tell if the contents of a failing test have changed
        /// </remarks>
        static void Test(string testfolder)
        {
            var m = new MarkdownSharp.Markdown();

            Console.WriteLine();
            Console.WriteLine(@"MarkdownSharp v" + MarkdownSharp.Markdown.Version + @" test run on " + Path.DirectorySeparatorChar + testfolder);
            Console.WriteLine();

            string path = Path.Combine(Utilities.ExecutingAssemblyPath, Path.Combine("testfiles", testfolder));
            string output;
            string expected;
            string actualpath;

            int ok = 0;
            int okalt = 0;
            int err = 0;
            int errnew = 0;
            int total = 0;

            foreach (var file in Directory.GetFiles(path, "*.text"))
            {

                expected = Utilities.FileContents(Path.ChangeExtension(file, "html"));
                output = m.Transform(Utilities.FileContents(file));

                actualpath = Path.ChangeExtension(file, Utilities.GetCrc16(output) + ".actual.html");

                total++;

                Console.Write(String.Format("{0:000} {1,-55}", total, Path.GetFileNameWithoutExtension(file)));

                if (output == expected)
                {
                    ok++;
                    Console.WriteLine("OK");
                }
                else if (Utilities.RemoveWhitespace(output) == Utilities.RemoveWhitespace(expected))
                {
                    ok++;
                    okalt++;
                    Console.WriteLine("OK^");
                    if (!File.Exists(actualpath))
                        File.WriteAllText(actualpath, output);
                }
                else
                {
                    err++;
                    if (File.Exists(actualpath))
                        Console.WriteLine("Mismatch");
                    else
                    {
                        errnew++;
                        Console.WriteLine("Mismatch *NEW*");
                        File.WriteAllText(actualpath, output);
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Tests        : " + total);
            if (okalt > 0)
                Console.WriteLine("OK           : " + ok + " (^ " + okalt + " whitespace differences)");
            else
                Console.WriteLine("OK           : " + ok);
            Console.Write("Mismatch     : " + err);
            if (errnew > 0)
                Console.WriteLine(" (" + errnew + " *NEW*)");
            else
                Console.WriteLine();

            if (errnew > 0)
            {
                Console.WriteLine();
                Console.WriteLine("for each mismatch, an *.actual.html file was generated in");
                Console.WriteLine(path);
                Console.WriteLine("to troubleshoot mismatches, use a diff tool on *.html and *.actual.html");
            }

        }


        /// <summary>
        /// executes a standard benchmark on short, medium, and long markdown samples  
        /// use this to verify any impacts of code changes on performance  
        /// please DO NOT MODIFY the input samples or the benchmark itself as this will invalidate previous 
        /// benchmark runs!
        /// </summary>
        static void Benchmark()
        {

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(@"MarkdownSharp v" + MarkdownSharp.Markdown.Version + " benchmark, takes 10 ~ 30 seconds...");
            Console.WriteLine();

            Benchmark(Utilities.FileContents(Path.Combine("benchmark", "markdown-example-short-1.text")), 4000);
            Benchmark(Utilities.FileContents(Path.Combine("benchmark", "markdown-example-medium-1.text")), 1000);
            Benchmark(Utilities.FileContents(Path.Combine("benchmark", "markdown-example-long-2.text")), 100);
            Benchmark(Utilities.FileContents(Path.Combine("benchmark", "markdown-readme.text")), 1);
            Benchmark(Utilities.FileContents(Path.Combine("benchmark", "markdown-readme.8.text")), 1);
            Benchmark(Utilities.FileContents(Path.Combine("benchmark", "markdown-readme.32.text")), 1);
        }

        /// <summary>
        /// performs a rough benchmark of the Markdown engine using small, medium, and large input samples 
        /// please DO NOT MODIFY the input samples or the benchmark itself as this will invalidate previous 
        /// benchmark runs!
        /// </summary>
        static void Benchmark(string text, int iterations)
        {
            var m = new MarkdownSharp.Markdown();

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < iterations; i++)
                m.Transform(text);
            sw.Stop();

            Console.WriteLine("input string length: " + text.Length);
            Console.Write(iterations + " iteration" + (iterations == 1 ? "" : "s") + " in " + sw.ElapsedMilliseconds + " ms");
            if (iterations == 1)
                Console.WriteLine();
            else
                Console.WriteLine(" (" + Convert.ToDouble(sw.ElapsedMilliseconds) / Convert.ToDouble(iterations) + " ms per iteration)");
        }


        /// <summary>
        /// executes nunit-console.exe to run all the tests in this assembly
        /// </summary>
        static void UnitTests()
        {
            /// TODO Implement Log4net
            //log4net.Config.XmlConfigurator.Configure();

            string testAssemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;

            Console.WriteLine("Running tests in {0}\n", testAssemblyLocation);

            var p = new Process();

            string path = Path.Combine(Path.GetDirectoryName(testAssemblyLocation), @"nunit-console\nunit-console.exe");
            path = path.Replace(@"\bin\Debug", "");
            path = path.Replace(@"\bin\Release", "");
            p.StartInfo.FileName = path;
            p.StartInfo.Arguments = "\"" + testAssemblyLocation + "\" /labels /nologo";

            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;

            p.StartInfo.RedirectStandardOutput = true;
            p.OutputDataReceived += new DataReceivedEventHandler(p_DataReceived);

            p.StartInfo.RedirectStandardError = true;
            p.ErrorDataReceived += new DataReceivedEventHandler(p_DataReceived);

            p.Start();

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            while (!p.HasExited)
                System.Threading.Thread.Sleep(500);

            Console.WriteLine();
        }

        private static void p_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data)) return;
            Console.WriteLine(e.Data);
        }
    }
}
