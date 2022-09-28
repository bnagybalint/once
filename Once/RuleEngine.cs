namespace Once
{
    public class RuleEngine
    {
        private List<Rule> Rules = new();

        public RuleEngine()
        {
        }

        public void AddRule(Rule rule)
        {
            this.Rules.Add(rule);
        }

        public void HandleProcessEvent(ProcessEventArgs ev)
        {
            foreach (Rule rule in this.Rules)
            {
                try
                {
                    bool triggered = rule.HandleEvent(ev);

                    if(triggered)
                    {
                        Console.WriteLine("Rule {0} was triggered", rule.Name);
                    }
                }
                catch(Exception e)
                {
                    Console.Error.WriteLine("Failed to evaluate rule:");
                    Console.Error.WriteLine(" - Rule: {0}", rule.Name);
                    Console.Error.WriteLine(" - Error: {0}", e.Message);
                }
            }
        }
    }
}