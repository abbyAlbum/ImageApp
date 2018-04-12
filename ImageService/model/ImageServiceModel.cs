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

        /// <summary>
        ///Creates the ImageServiceModel.
        ///Param: string - the output folder, int - the thumbnailsize
        /// </summary>
        public ImageServiceModel(string m_OutputFolder, int m_TumbnailSize)
        {
            this.m_OutputFolder = m_OutputFolder;
            this.m_thumbnailSize = m_TumbnailSize;
        }
        /// <summary>
        /// Creates a hidden directory to add the pictures to.
        /// Creates the years folder (sub directory) and thumbnail folder.
        /// Organises by year/month and creates a thumbnail. 
        /// Param: string - path, bool - the result
        /// </summary>
        public string AddFile(string path, out bool result)
        {
            DirectoryInfo output = new DirectoryInfo(m_OutputFolder);
            DirectoryInfo desktop = Directory.CreateDirectory(m_OutputFolder);
            DirectoryInfo di = desktop.CreateSubdirectory("OutputDir");
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            DirectoryInfo sub_years = di.CreateSubdirectory("Years");
            DirectoryInfo sub_Thumb = di.CreateSubdirectory("Thumbnails");

            string[] paths = Directory.GetDirectories(di.FullName);
            DateTime date = GetDateTakenFromImage(path, out string prob);
            if (date == DateTime.MinValue)
            {
                result = false;
                return prob;
            }
            string dst = FindFolder(date, paths[0]);
            string dst2 = FindFolder(date, paths[1]);

            dst2 = MoveFile(path, dst2, out result);
            if (result == false) return dst2 + prob;
            return MakeTumb(dst2, dst, out result) + prob;

        }
        /// <summary>
		/// Creates a thumbnail for each image.
        /// Param: string - path to the picture, string - dst.
        /// Return: string - that file was added.
		/// </summary>
        public string MakeTumb(string path_to_pic, string dst, out bool result)
        {
            Image image = Image.FromFile(path_to_pic);
            try
            {
                string name = Path.GetFileName(path_to_pic);
                string dst2 = dst + Path.DirectorySeparatorChar + name;
                Image thumb = image.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero);
                thumb.Save(Path.ChangeExtension(path_to_pic, "thumb"));
                image.Dispose();
                thumb.Dispose();
                path_to_pic = Path.ChangeExtension(path_to_pic, "thumb");
                dst2 = Path.ChangeExtension(dst2, "thumb");

                if (File.Exists(dst2)) dst2 = GetUniquePath(dst, dst2);
              
                File.Move(path_to_pic, dst2);
            }
            catch (Exception e)
            {
                if(image != null) image.Dispose();
                result = false;
                return e.ToString();
            }
            result = true;
            return "File Added";
        }
        /// <summary>
		/// Gets the path of the file - if not the same.
        /// Param: string path
        /// Return: the unique path
		/// </summary>
        public string GetUniquePath(string dst, string dst2)
        {
            int num = 1;
            string name1 = Path.GetFileNameWithoutExtension(dst2);
            string ext = Path.GetExtension(dst2);
            dst2 = dst + Path.DirectorySeparatorChar + name1 + num.ToString() + ext;
            while (File.Exists(dst2))
            {
                num++;
                dst2 = dst + Path.DirectorySeparatorChar + name1 + num.ToString() + ext;
            }
            return dst2;
        }

    
        //we init this once so that if the function is repeatedly called
        //it isn't stressing the garbage man
        private static Regex r = new Regex(":");

        //retrieves the datetime WITHOUT loading the whole image
        /// <summary>
		/// retrieves the datetime without loading the whole image.
        /// Param: string - path.
		/// </summary>
        public static DateTime GetDateTakenFromImage(string path, out string prob)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    prob = "";
                    return DateTime.Parse(dateTaken);
                }
            } catch(Exception)
            {
                return GetCreation(path, out prob);
            }
        }
        /// <summary>
        /// When there is no date get the creation
        /// Param: string path, out string prob
        /// </summary>
        public static DateTime GetCreation(string path, out string prob)
        {
            try
            {
                DateTime date = File.GetCreationTime(path);
                prob = " no date taken propperty, took creation time";
                return date;
            }
            catch (Exception d)
            {
                prob = "no date taken property" + d.ToString();
                return new DateTime();
            }
        }
        /// <summary>
        /// Finds the correct folder to add the image according to the date.
        /// Then creates the directory to place the image if not already created. 
        /// Param: DatimeTime - the date of the image, string - image path. 
        /// Return: The path with the date.
        /// </summary>
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
        /// <summary>
        /// Adds the image to that specific directory. 
        /// Param: string - the current directory, string - the destination path. 
        /// Return: destination string.
        /// </summary>
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
        /// <summary>
        /// Remove file if already exists.
        /// Param: String - image path. 
        /// Param: string - path.
        /// </summary>
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

//retrieves the datetime WITHOUT loading the whole image
/* public static DateTime GetDateTakenFromImage(string path, out string prob)
 {
     Image myImage = Image.FromFile(path);
     try
     {
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
         prob = "none";
         return dtaken;
     }
     catch (Exception e)
     {
         myImage.Dispose();
         prob = e.ToString();
         return new DateTime();
     }

 }*/
