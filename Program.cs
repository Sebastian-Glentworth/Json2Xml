using Newtonsoft.Json;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Json2Xml
{
    internal static class Program
    {
        /// <summary>
        /// A little console app that transforms Json files to xml
        /// </summary>
        /// <param name="file">The file to transform</param>
        /// <param name="outputToFile">Whether to write to file. If false, will print to console</param>
        /// <param name="outputFile">The location to write to. If null, will write to a file in the format {originalFile}.xml</param>
        /// <returns></returns>
        private static async Task Main(FileInfo file, bool outputToFile = true, FileInfo outputFile = null)
        {
            if (!file.Exists) throw new InvalidOperationException($"File {file.FullName} does not exist");

            string json = default;
            XNode xml = default;

            using (var fs = file.Open(FileMode.Open))
            using (var sr = new StreamReader(fs))
            {
                Console.WriteLine($"Reading {file.FullName}");
                json = await sr.ReadToEndAsync();
                Console.WriteLine($"Successfully read {file.FullName}");
            }

            Console.WriteLine("Converting to xml...");
            try
            {
                xml = JsonConvert.DeserializeXNode(json);
            }
            catch (Exception e)
            {
                Console.WriteLine("Whoops! There was an error when converting to XML!");
                Console.WriteLine(e.Message);
            }

            if (outputToFile)
            {
                outputFile ??= new FileInfo($"{Path.GetFileNameWithoutExtension(file.FullName)}.xml");
                using (var fs = outputFile.Open(FileMode.OpenOrCreate))
                using (var sw = new StreamWriter(fs))
                {
                    Console.WriteLine($"writing to {outputFile.FullName}");
                    await sw.WriteAsync(xml.ToString());
                }
            }
            else
            {
                Console.WriteLine(xml);
            }
        }
    }
}