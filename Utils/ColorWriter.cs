using System.Drawing;
using Console = Colorful.Console;

namespace MiniBank.Utils
{
    internal class ColorWriter
    {
        internal Color PrimaryColor { get; set; }

        public ColorWriter()
        {
            PrimaryColor = Color.DodgerBlue;
        }

        internal void DisplayErrorMessage(string message) => Console.WriteLine(message, Color.Salmon);
        internal void DisplaySuccessMessage(string message) => Console.WriteLine(message, Color.LawnGreen);

        internal void DisplayPrimary(string message) => Console.WriteLine(message, PrimaryColor);
    }
}
