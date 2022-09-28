using System;
using Newtonsoft.Json.Linq;

namespace Once {
    public class RuleConfig
    {
        public string Version { get; set; }
        public List<Rule> Rules { get; set; }

        public RuleConfig(string version, List<Rule>? rules)
        {
            this.Version = version;
            this.Rules = (rules != null) ? rules : new List<Rule>();
        }

        public static RuleConfig LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Config file not found: {0}", path);
            }
    
            string dataJsonStr = File.ReadAllText(path);

            JObject dataJson = JObject.Parse(dataJsonStr);

            return RuleConfig.Parse(dataJson);
        }

        private static RuleConfig Parse(JObject jobj)
        {
            return new RuleConfig(
                version: jobj["version"]!.ToString(),
                rules: new List<Rule>(from x in jobj["rules"]!.ToList() select ParseRule((JObject)x))
            );
        }

        private static Rule ParseRule(JObject jobj)
        {
            return new Rule(
                name: jobj["name"]?.ToString(),
                triggers: (from x in jobj["triggers"]!.ToList() select ParseTrigger((JObject)x)).ToList<RuleTrigger>(),
                actions: (from x in jobj["actions"]!.ToList() select ParseAction((JObject)x)).ToList<RuleAction>()
            );
        }

        private static RuleTrigger ParseTrigger(JObject jobj)
        {
            string? eventTypeStr = jobj["process_event_type"]?.ToString();

            return new RuleTrigger(
                processName: jobj["process_name"]?.ToString(),
                processEventType: (eventTypeStr != null) ? ParseEventType(eventTypeStr) : null
            );
        }

        private static RuleAction ParseAction(JObject jobj)
        {
            string type = jobj["type"]!.ToString();

            if(type == "execute")
            {
                int? delaySeconds = jobj["delay"]?.ToObject<Int32>();

                return new ExecuteCommandRuleAction(
                    command: jobj["command"]!.ToString(),
                    cwd: jobj["cwd"]?.ToString(),
                    delay: (delaySeconds != null) ? TimeSpan.FromSeconds((double)delaySeconds) : null
                );
            }
            else
            {
                throw new FormatException(String.Format("Invalid value for action type: {0}", type));
            }

        }

        private static ProcessEventType ParseEventType(string s)
        {
            switch(s)
            {
                case "started": return ProcessEventType.STARTED;
                case "terminated": return ProcessEventType.TERMINATED;
                default: throw new FormatException(String.Format("Invalid value for process event type: {0}", s));
            }
        }
    }
}