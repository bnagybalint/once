using CommandLine;
using CommandLine.Text;
using Once;

public class Program
{
    public class CmdOptions
    {
        [Value(0, MetaName = "config_file_path", Required = true, HelpText = "Config file path")]
        public string ConfigPath { get; set; } = string.Empty;
    }

    static void Main(string[] _args)
    {
        CmdOptions args = new CmdOptions();

        var parserResult = Parser.Default.ParseArguments<CmdOptions>(_args);
        parserResult
            .WithParsed<CmdOptions>(o => 
            {
                args = o;
            })
            .WithNotParsed(x =>
            {
                var helpText = HelpText.AutoBuild(parserResult, h =>
                {
                    h.AutoHelp = false;     // hides --help
                    h.AutoVersion = false;  // hides --version
                    return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                }, e => e);
                Console.WriteLine(helpText);
                Environment.Exit(1);
            });

        Console.WriteLine("Loading rule config.");
        RuleConfig ruleConfig = RuleConfig.LoadFromFile(args.ConfigPath.ToString());

        Console.WriteLine("Setting up rule engine.");
        RuleEngine ruleEngine = new();
        foreach(Rule rule in ruleConfig.Rules)
        {
            Console.WriteLine("Adding rule '{0}'", rule.Name);
            ruleEngine.AddRule(rule);
        }

        Console.WriteLine("Setting up process listener.");
        ProcessWatcher processWatcher = new ();
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