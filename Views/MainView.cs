namespace MiniBank.Views
{
    internal class MainView
    {
        internal void DisplayErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }        
        internal void DisplaySuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal void DisplayMenu()
        {

        }

        internal string SelectAction()
        {
            return Console.ReadLine();
        }
    }
}