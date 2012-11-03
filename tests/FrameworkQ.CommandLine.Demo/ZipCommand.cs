using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace FrameworkQ.CommandLine.Demo
{
    [Command ("zip", "Compresses the file")]
    class ZipCommand : Command
    {
        private string _in = string.Empty;
        private string _outpath = string.Empty;

        public ZipCommand()
            : base("zip")
        {
        }

        [CommandParameter("in", false, CommandOption.CommandOptionType.FilePath,false,
            "The file to be zipped")]
        public string In
        {
            get { return _in; }
            set { _in = value; }
        }

        [CommandParameter("out", false, CommandOption.CommandOptionType.PossibleFilePath, false,
            "Output zip file name")]
        public string Outpath
        {
            get { return _outpath; }
            set { _outpath = value; }
        }

        private int _bufferSize = 512;
        [CommandParameter("buffer", true, CommandOption.CommandOptionType.Integer,
            true, "Number of bytes to zip on each iteration")]
        public int BufferSize
        {
            get { return _bufferSize; }
            set { _bufferSize = value; }
        }

        [System.Security.Permissions.FileIOPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Execute()
        {
            if (File.Exists(this.In))
            {
                byte[] buffer = new byte[this.BufferSize];
                using (FileStream fs = new FileStream (this.In,FileMode.Open, FileAccess.Read))
                {
                    using (FileStream outfile = new FileStream(this.Outpath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (GZipStream zs = new GZipStream(outfile, CompressionMode.Compress))
                        {
                            System.IO.Compression.DeflateStream s;
                            int totalRead = 0;
                            int bytesRead = 0;
                            while (totalRead < fs.Length)
                            {
                                bytesRead = fs.Read(buffer, 0, this.BufferSize);
                                zs.Write(buffer, 0, bytesRead);
                                totalRead = totalRead + bytesRead;
                            }
                        }
                    }
                }
            }
        }
    }
}
