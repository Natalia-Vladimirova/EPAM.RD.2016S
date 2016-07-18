using System;
using System.IO;
using System.Net;
using System.Text;

namespace FileStreams
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Arguments: <source> <destination>");
                Console.ReadLine();
                return;
            }

            string source = args[0];
            string destin = args[1];

            ByteCopy(source, destin);
            BlockCopy(source, destin);
            LineCopy(source, destin);
            MemoryBufferCopy(source, destin);
            WebClient();
            Console.ReadLine();
        }

        public static void ByteCopy(string source, string destin)
        {
            int bytesCounter = 0;

            // Implement byte-copy here.
            
            using (var sourceStream = new FileStream(source,FileMode.Open, FileAccess.Read))
            using (var destinStream = new FileStream(destin, FileMode.OpenOrCreate, FileAccess.Write))
            {
                int b;
                while ((b = sourceStream.ReadByte()) != -1)
                {
                    bytesCounter++;
                    destinStream.WriteByte((byte)b);
                }
            }
            
            Console.WriteLine("ByteCopy() done. Total bytes: {0}", bytesCounter);
        }

        public static void BlockCopy(string source, string destin)
        {
            // Implement block copy via buffer.
            int bufSize = 1024;
            using (var sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read))
            using (var destinStream = new FileStream(destin, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] buffer = new byte[bufSize];
                int bytesRead;

                do
                {
                    bytesRead = sourceStream.Read(buffer, 0, bufSize); 
                    
                    Console.WriteLine("BlockCopy(): writing {0} bytes.", bytesRead);
                    destinStream.Write(buffer, 0 , bytesRead); 
                }
                while (bytesRead == buffer.Length);
            }
        }

        public static void LineCopy(string source, string destin)
        {
            int linesCount = 0;

            // implement copying lines using StreamReader/StreamWriter.
            
            using (var sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read))
            using (var destinStream = new FileStream(destin, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var streamReader = new StreamReader(sourceStream))
                using (var streamWriter = new StreamWriter(destinStream))
                {
                    while (true)
                    {
                        linesCount++;
                        string line;
                        if ((line = streamReader.ReadLine()) == null)
                        {
                            break;
                        }
                        streamWriter.WriteLine(line); 
                    }
                }
            }
            
            Console.WriteLine("LineCopy(): {0} lines.", linesCount);
        }

        public static void MemoryBufferCopy(string source, string destin)
        {
            var stringBuilder = new StringBuilder();

            string content;

            using (var textReader = (TextReader)new StreamReader(source)) 
            {
                content = textReader.ReadToEnd(); // read entire file
            }

            using (var sourceReader = new StringReader(content))
            using (var sourceWriter = new StringWriter(stringBuilder))
            {
                string line;

                do
                {
                    line = sourceReader.ReadLine(); 
                    if (line != null)
                    {
                        sourceWriter.WriteLine(line); 
                    }

                } while (line != null);
            }

            Console.WriteLine("MemoryBufferCopy(): chars in StringBuilder {0}", stringBuilder.Length);

            const int blockSize = 1024;

            using (var stringReader = new StringReader(stringBuilder.ToString())) // Use StringReader to read from stringBuilder.
            using (var memoryStream = new MemoryStream(blockSize))
            using (var streamWriter = new StreamWriter(memoryStream)) // Compose StreamWriter with memory stream.
            using (var destinStream = new FileStream(destin, FileMode.OpenOrCreate, FileAccess.Write)) // Use file stream.
            {
                char[] buffer = new char[blockSize];
                int bytesRead;

                do
                {
                    bytesRead = stringReader.ReadBlock(buffer, 0, blockSize); // Read block from stringReader to buffer.
                    streamWriter.Write(buffer, 0, bytesRead); // Write buffer to streamWriter.
                }
                while (bytesRead == blockSize);
                destinStream.Write(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length); // write memoryStream.GetBuffer() content to destination stream.
            }
        }

        public static void WebClient()
        {
            WebClient webClient = new WebClient();
            using (var stream = webClient.OpenRead("http://google.com"))
            {
                if (stream == null)
                {
                    return;
                }

                Console.WriteLine("WebClient(): CanRead = {0}", stream.CanRead); // print if it is possible to read from the stream
                Console.WriteLine("WebClient(): CanWrite = {0}", stream.CanWrite); // print if it is possible to write to the stream
                Console.WriteLine("WebClient(): CanSeek = {0}", stream.CanSeek); // print if it is possible to seek through the stream

                // Save steam content to "google_request.txt" file.
                using (var sr = new StreamReader(stream, Encoding.Unicode))
                using (var fs = new FileStream("google_request.txt", FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sw.WriteLine(line);
                    }
                }
            }
        }
    }
}