using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    class ImageModel : ImageServiceModel
    {
        private string m_OutputFolder;
        private int m_TumbnailSize;

        public ImageModel(string m_OutputFolder, int m_TumbnailSize)
        {
            this.m_OutputFolder = m_OutputFolder;
            this.m_TumbnailSize = m_TumbnailSize;
        }

        public string AddFile(string path, out bool result)
        {
            if (m_OutputFolder == null) m_OutputFolder = "Users\IEUser\Desktop\OutputDir";
        }

        public string CreateFolder(string fileName)
        {
            string pathString = System.IO.Path.Combine(m_OutputFolder, fileName);

            return pathString;
        }

        public void MoveFile(string curDir, string dstDir, string fileName)
        {
            string curPathString = System.IO.Path.Combine(curDir, fileName);
            string dstPathString = System.IO.Path.Combine(dstDir, fileName);

            if(!System.IO.Directory.Exists(dstPathString))
            {
                System.IO.Directory.CreateDirectory(dstPathString);
            }

            //System.IO.File.Copy(curPathString, dstPathString, true);

            if (System.IO.Directory.Exists(curDir))
            {
                string[] files = System.IO.Directory.GetFiles(curDir);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = System.IO.Path.GetFileName(s);
                    dstPathString = System.IO.Path.Combine(dstPathString, fileName);
                    System.IO.File.Copy(s, dstPathString, true);
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }

        }

        public void RemoveFile(string pathName)
        {
            if (System.IO.File.Exists(pathName))
            {
                // Use a try block to catch IOExceptions, to
                // handle the case of the file already being
                // opened by another process.
                try
                {
                    System.IO.File.Delete(pathName);
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }
    }
}
