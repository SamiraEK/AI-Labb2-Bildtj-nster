using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Drawing;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace AI_Labb2_Bildtjänster
{
    internal class ImageRecognition
    {
        public static bool ImgMenu;
        private static ImageAnalysis analysis;
        private static ComputerVisionClient cvClient;
        static public async Task PictureConfiqure(string inputImage)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string cogSvcEndpoint = configuration["CognitiveServicesEndpoint"];
                string cogSvcKey = configuration["CognitiveServiceKey"];


                // Authenticate Computer Vision client
                ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(cogSvcKey);
                cvClient = new ComputerVisionClient(credentials)
                {
                    Endpoint = cogSvcEndpoint
                };

                // Analyze image
                await AnalyzeImage(inputImage);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static async Task AnalyzeImage(string imageFile)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Analyzing {imageFile}\n");
            

            // Specify features to be retrieved
            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
             VisualFeatureTypes.Description,
             VisualFeatureTypes.Tags,
             VisualFeatureTypes.Categories,
             VisualFeatureTypes.Brands,
             VisualFeatureTypes.Objects,
             VisualFeatureTypes.Adult
            };



            // Get image analysis
            using (var imageData = File.OpenRead(imageFile))
            {
                //deleting old data
                analysis = null;

                ImgMenu = true;
                // Specify features to be retrieved
                analysis = await cvClient.AnalyzeImageInStreamAsync(imageData, features);
            }
        }
        static public async void ImageMenu(string imageFile)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Enter one of choices to see! " +
           "\nCaption -- You get only the images caption " +
           "\nTags --  You get only the images Tags " +
           "\nRating -- You get the pictures rating like adult or racy " +
           "\nBrand -- If there are any brands in your picture" +
           "\nImage -- A picture with the objects captured" +
           "\n Main -- to go back to the mainmenu");

            string userinput = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            try
            {
                Console.Clear();
                switch (userinput.ToLower())
                {
                    case "caption":
                        // get image captions
                        foreach (var caption in analysis.Description.Captions)
                        {
                            Console.WriteLine($"Description: {caption.Text}.");
                        }
                        break;

                    case "tags":
                        // Get image tags
                        if (analysis.Tags.Count > 0)
                        {
                            Console.WriteLine("Tags:");
                            foreach (var tag in analysis.Tags)
                            {
                                Console.WriteLine($" -{tag.Name} (confidence: {tag.Confidence.ToString("P")})");
                            }
                        }
                        break;
                    case "rating":
                        // Get moderation ratings
                        string ratings = $"Ratings:\n -Adult: {analysis.Adult.IsAdultContent}\n -Racy:{analysis.Adult.IsRacyContent}\n -Gore: {analysis.Adult.IsGoryContent}";
                        Console.WriteLine(ratings);
                        break;
                    case "brand":
                        // Get brands in the image
                        if (analysis.Brands.Count > 0)
                        {
                            Console.WriteLine("Brands:");
                            foreach (var brand in analysis.Brands)
                            {
                                Console.WriteLine($" -{brand.Name} (confidence: {brand.Confidence.ToString("P")})");
                            }
                        }
                        break;
                    case "image":
                        // Get objects in the image
                        if (analysis.Objects.Count > 0)
                        {
                            Console.WriteLine("Objects in image:");

                            // Prepare image for drawing
                            Image image = Image.FromFile(imageFile);
                            Graphics graphics = Graphics.FromImage(image);
                            Pen pen = new Pen(Color.Cyan, 3);
                            Font font = new Font("Arial", 16);
                            SolidBrush brush = new SolidBrush(Color.Black);
                            foreach (var detectedObject in analysis.Objects)
                            {
                                // Print object name
                                Console.WriteLine($" -{detectedObject.ObjectProperty} (confidence:{detectedObject.Confidence.ToString("P")})");
                                // Draw object bounding box
                                var r = detectedObject.Rectangle;
                                Rectangle rect = new Rectangle(r.X, r.Y, r.W, r.H);
                                graphics.DrawRectangle(pen, rect);
                                graphics.DrawString(detectedObject.ObjectProperty, font, brush, r.X, r.Y);
                            }
                            // Save annotated image
                            String output_file = "objects.jpg";
                            image.Save(output_file);
                            Console.WriteLine(" Results saved in " + output_file);
                        }
                        break;
                    case "main":
                        ImgMenu = false;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    default:
                        Console.WriteLine("Please only enter one of the options below");
                        break;
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
