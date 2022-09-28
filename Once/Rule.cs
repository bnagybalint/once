

namespace Once
{
    public class RuleTrigger
    {
        public int? ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public ProcessEventType? ProcessEventType { get; set; }

        public RuleTrigger(int? processId = null, string? processName = null, ProcessEventType? processEventType = null)
        {
            this.ProcessId = processId;
            this.ProcessName = processName;
            this.ProcessEventType = processEventType;
        }

        public bool Matches(ProcessEventArgs ev)
        {
            if(this.ProcessId != null && this.ProcessId != ev.ProcessId)
            {
                return false;
            }

            if(this.ProcessName != null && this.ProcessName != ev.ProcessName)
            {
                return false;
            }

            if(this.ProcessEventType != null && this.ProcessEventType != ev.EventType)
            {
                return false;
            }

            return true;
        }
    }

    public abstract class RuleAction
    {
        public abstract void Execute();
    }

    public class ExecuteCommandRuleAction: RuleAction
    {
        public string Command { get; set; }
        public string? Cwd { get; set; }
        public TimeSpan? Delay { get; set; }

        public ExecuteCommandRuleAction(string command, string? cwd = null, TimeSpan? delay = null)
        {
            this.Command = command;
            this.Cwd = cwd;
            this.Delay = delay;
        }

        public override void Execute()
        {
            Console.WriteLine("Performing ExecuteCommand: {0}.", this.Command);
        }
    }

    public class Rule
    {
        public string Name { get; set; }
        private List<RuleTrigger> Triggers { get; set; }
        private List<RuleAction> Actions { get; set; }

        public Rule(string? name = null, List<RuleTrigger>? triggers = null, List<RuleAction>? actions = null)
        {
            this.Name = (name != null) ? name : "<unnamed rule>";
            this.Triggers = (triggers != null) ? triggers : new();
            this.Actions = (actions != null) ? actions : new();
        }

        /// <summary>
        /// Handles the process event passed. If the rule triggers (i.e. any of the trigger conditions are met),
        /// all the associated actions are executed.
        /// </summary>
        /// <param name="ev">the process event to handle.</param>
        /// <returns>True, if the rule was triggered</returns>
        public bool HandleEvent(ProcessEventArgs ev)
        {
            bool triggered = this.ShouldHandleEvent(ev);

            if(triggered)
            {
                Console.WriteLine("Rule trigger condition fulfilled, performing actions...");
                this.PerformActions();
            }

            return triggered;
        }

        private bool ShouldHandleEvent(ProcessEventArgs ev)
        {
            foreach(RuleTrigger trigger in this.Triggers)
            {
                if(trigger.Matches(ev))
                {
                    return true;
                }
            }

            return false;
        }

        private void PerformActions()
        {
            foreach(RuleAction action in this.Actions)
            {
                // TODO put this in a different thread to handle delayed execution
                action.Execute();
            }
        }
    }
}