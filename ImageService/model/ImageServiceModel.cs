using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

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
            DirectoryInfo output = new DirectoryInfo(m_OutputFolder);
            DirectoryInfo desktop = Directory.CreateDirectory(m_OutputFolder);
            DirectoryInfo di = desktop.CreateSubdirectory("OutputDir");
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            DirectoryInfo sub_years = di.CreateSubdirectory("Years");
            DirectoryInfo sub_Thumb = di.CreateSubdirectory("Thumbnails");

            /*if (!Directory.Exists(path))
            {
                result = false;
                return "path not valid!";
            }*/
            string[] paths = Directory.GetDirectories(di.FullName);
            DateTime date = GetDateTakenFromImage(path);
            if (date == DateTime.MinValue)
            {
                result = false;
                return "prob with path";
            }
            string dst = FindFolder(date, paths[0]);
            string dst2 = FindFolder(date, paths[1]);

            dst2 = MoveFile(path, dst2, out result);
            if (result == false) return dst2;
            MakeTumb(dst2, dst);


            result = true;

            return "noError";

        }

        public void MakeTumb(string path_to_pic, string dst)
        {
            string name = Path.GetFileName(path_to_pic);
            dst = dst + Path.DirectorySeparatorChar + name;
            Image image = Image.FromFile(path_to_pic);
            Image thumb = image.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero);
            thumb.Save(Path.ChangeExtension(path_to_pic, "thumb"));
            image.Dispose();
            thumb.Dispose();
            path_to_pic = Path.ChangeExtension(path_to_pic, "thumb");
            dst = Path.ChangeExtension(dst, "thumb");

            if (!File.Exists(dst))
            {
                File.Move(path_to_pic, dst);
            }


        }

        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetDateTakenFromImage(string path)
        {
            try
            {
                Image myImage = Image.FromFile(path);
                PropertyItem propItem = myImage.GetPropertyItem(306);
                DateTime dtaken;

                //Convert date taken metadata to a DateTime object
                string sdate = Encoding.UTF8.GetString(propItem.Value).Trim();
                string secondhalf = sdate.Substring(sdate.IndexOf(" "), (sdate.Length - sdate.IndexOf(" ")));
                string firsthalf = sdate.Substring(0, 10);
                firsthalf = firsthalf.Replace(":", "-");
                sdate = firsthalf + secondhalf;
                dtaken = DateTime.Parse(sdate);
                myImage.Dispose();
                return dtaken;
            }
            catch (Exception e)
            {
                return new DateTime();
            }

        }

        public string FindFolder(DateTime date, string path)
        {

            int pic_year = date.Year;
            int pic_month = date.Month;
            char sep_char = Path.DirectorySeparatorChar;

            string year_path = path + sep_char + pic_year.ToString();
            string month_path = year_path + sep_char + pic_month.ToString();


            Directory.CreateDirectory(month_path);
            return month_path;
        }

        public string MoveFile(string curDir, string dst2, out bool result)
        {

            string name = Path.GetFileName(curDir);
            string dst4 = dst2 + Path.DirectorySeparatorChar + name;
            if (File.Exists(dst4))
            {
                int num = 1;
                string name1 = Path.GetFileNameWithoutExtension(curDir);
                string ext = Path.GetExtension(curDir);
                dst4 = dst2 + Path.DirectorySeparatorChar + name1 + num.ToString() + ext;
                while (File.Exists(dst4))
                {
                    num++;
                    dst4 = dst2 + Path.DirectorySeparatorChar + name1 + num.ToString() + ext;
                }
            }
                
                try
                {
                    File.Move(curDir, dst4);
                }
                catch (Exception e)
                {
                    result = false;
                    return e.ToString();
                }
            
           
            result = true;
            return dst4;

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
            }
              public string CreateFolder(string fileName, string path)
        {
            string pathString = Path.Combine(path, fileName);

            return pathString;
        }

         public string GetDate(string path, out string year)
        {

            string[] words = path.Split(Path.DirectorySeparatorChar);
            year = words[words.Length - 1];
            return words[words.Length];
        }
*/
