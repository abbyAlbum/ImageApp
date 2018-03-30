﻿using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageService.Model
{
    public class ImageServiceModel : IImageServiceModel
    {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size


        public ImageServiceModel(string m_OutputFolder, int m_TumbnailSize)
        {
            this.m_OutputFolder = m_OutputFolder;
            this.m_thumbnailSize = m_TumbnailSize;
        }

        public string AddFile(string path, out bool result)
        {
            if (m_OutputFolder == null)
            {
                m_OutputFolder = CreateFolder("OutputDir", ""); //prob wont work.. what directory to use?
                if (!File.Exists(m_OutputFolder))
                {
                    result = false;
                    return "Couldnt create outputdir";
                }
            }
           

            string dst = FindFolder(m_OutputFolder);

            MoveFile(path, path, dst);

            result = true; //when should be false?

            return "noError";

        }

        //we init this once so that if the function is repeatedly called
        //it isn't stressing the garbage man
        private static Regex r = new Regex(":");

        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }

        public string FindFolder(DateTime date, out bool year, out bool month)
        {




            /*string month = GetDate(path, out string year);
            string pathYear = path + Path.DirectorySeparatorChar + year;
            string pathMonth = pathYear + Path.DirectorySeparatorChar + month;

            if (Directory.Exists(pathMonth)) return pathMonth;
            if (Directory.Exists(pathYear)) return pathYear;
            return pathMonth;*/
        }

        public string GetDate(string path, out string year)
        { 

            string[] words = path.Split(Path.DirectorySeparatorChar);
            year = words[words.Length - 1];
            return words[words.Length];
        }

        public string CreateFolder(string fileName, string path)
        {
            string pathString = Path.Combine(path, fileName);

            return pathString;
        }

        public void MoveFile(string curDir, string dstDir, string fileName)
        {
            string curPathString = Path.Combine(curDir, fileName);
            string dstPathString = Path.Combine(dstDir, fileName);

            if (!Directory.Exists(dstPathString))
            {
                Directory.CreateDirectory(dstPathString);
            }

            File.Copy(curPathString, dstPathString, true);


        }

        public void RemoveFile(string pathName)
        {
            if (File.Exists(pathName))
            {
                // Use a try block to catch IOExceptions, to
                // handle the case of the file already being
                // opened by another process.
                try
                {
                    File.Delete(pathName);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }
    }

    #endregion

}

/*if (System.IO.Directory.Exists(curDir))
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
            }*/
