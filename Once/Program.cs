using Once;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Setting up process listener...");
        ProcessWatcher processWatcher = new ProcessWatcher();
        processWatcher.Start();

        Console.WriteLine("Listening started!");

        // TODO do proper threading
        do {
            Thread.Sleep(1000);
        } while (true);
    }

}