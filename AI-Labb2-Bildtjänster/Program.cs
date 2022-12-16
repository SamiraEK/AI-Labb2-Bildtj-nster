using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using System.Diagnostics;
using System.Reflection;
using AI_Labb2_Bildtjänster;

class Program
{
    private static bool exit = false;
    static string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\images"; 
    static void Main(string[] args)
    {
        do
        {
            Menu();
        } while (exit == false);
    }

    static void Menu()
    {
        Console.WriteLine("**** Welcome to my application for analyzing image! ****\n\n");
        Console.WriteLine("  Please select one of these following choices:\n\n - Enter your image name like: ImageName.jpg \n -tags Type \"open folder\" -- to open the image folder and add some picture");
        string input = Console.ReadLine();
        if (input.ToLower() == "open folder")
        {
            Process.Start("explorer.exe", path);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
        }
        else
        {
            string image = "images/" + input;
            ImageRecognition.PictureConfiqure(image);
            do
            {
                ImageRecognition.ImageMenu(image);

            }
            while (ImageRecognition.ImgMenu == true);

        }

    }


}
