using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FrameworkQ.CommandLine
{
    public class Path : ParameterType
    {
        public Path()
            : base()
        {
        }

        public enum PathType
        {
            FolderOrFile,
            FolderOnly, 


            FileOnly, 
            PossibleFilePath
        }

        private string _pathString = string.Empty;
        public string PathString
        {
            get { return _pathString; }
            set { _pathString = value; }
        }

        private PathType _pathType = PathType.FolderOrFile;
        public PathType FolderOrFile
        {
            get { return _pathType; }
            set { _pathType = value; }
        }

        private bool _isRelativePathAccepted = true;
        public bool IsRelativePathAccepted
        {
            get { return _isRelativePathAccepted; }
            set { _isRelativePathAccepted = value; }
        }

        private bool _canPathBeCreated = false;
        public bool CanPathBeCreated
        {
            get { return _canPathBeCreated; }
            set { _canPathBeCreated = value; }
        }

        public string GetFullPath()
        {
            try
            {
                return System.IO.Path.GetFullPath(this.PathString);
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool IsValidPath()
        {
            string pathToTest = this.GetFullPath();
            if (this.FolderOrFile == PathType.FileOnly || this.FolderOrFile == PathType.FolderOrFile)
            {
                return File.Exists(pathToTest);
            }

            if (this.FolderOrFile == PathType.FolderOnly || this.FolderOrFile == PathType.FolderOrFile)
            {
                return Directory.Exists(pathToTest);
            }

            if (this.FolderOrFile == PathType.PossibleFilePath)
            {
                int pos = pathToTest.LastIndexOf("\\");
                if (pos > 1)
                {
                    return Directory.Exists(pathToTest.Substring(0, pos));
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}
