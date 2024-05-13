namespace Greeter;

public static partial class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to WebAssembly Program.Main(string[] args)!!!");

        if (args.Length > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Here are the arguments passed to Program.Main:");

            foreach(string arg in args) 
            { 
                Console.WriteLine($"\t{arg}");
            }
        }
    }
}