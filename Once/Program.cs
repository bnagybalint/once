using Once;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Setting up process listener.");
        ProcessWatcher processWatcher = new ();

        Console.WriteLine("Setting up rule engine.");
        RuleEngine ruleEngine = new();
        // TODO setup engine
        processWatcher.ProcessEvent += ruleEngine.HandleProcessEvent;

        Console.WriteLine("Starting process watcher.");
        processWatcher.Start();

        // TODO do proper threading
        do {
            Thread.Sleep(1000);
        } while (true);

        // processWatcher.Stop();
    }

}