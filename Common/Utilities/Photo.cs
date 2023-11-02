using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AirlineOrevine.Files;
using LinerDb = DbContext.Models.Liner;

namespace Common.Utilities
{
    public class Photo
    {
        private static BitmapImage? BitmapImage { get; set; }

        public static void LoadPhoto(string base64, Image photoImage, LinerDb liner)
        {
            liner.Photo = base64;
            byte[] binaryData = Convert.FromBase64String(base64);
            BitmapImage = new BitmapImage();
            BitmapImage.BeginInit();
            BitmapImage.StreamSource = new MemoryStream(binaryData);
            BitmapImage.EndInit();
            photoImage.Source = BitmapImage;
        }
        public static void AddPhoto(LinerDb liner, Image photoImage)
        {

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filePath = dlg.FileName;
                byte[] imageArray = System.IO.File.ReadAllBytes(filePath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                liner.Photo = base64ImageRepresentation;
                LoadPhoto(liner.Photo ?? LinerPhoto.img, photoImage, liner);
            }
        }
    }
}
