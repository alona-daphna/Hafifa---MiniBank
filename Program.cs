using Microsoft.Extensions.Configuration;
using MiniBank.Controllers;

internal class Program
{
    public static void Main()
    {
        var users = new UserController().GetAll();

        users.ForEach(x => Console.WriteLine(x.Name));
    }
}