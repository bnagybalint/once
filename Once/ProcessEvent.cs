namespace Once {
    public enum ProcessEventType {
        STARTED,
        TERMINATED,
    }

    public class ProcessEvent
    {
        public int ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public ProcessEventType? EventType { get; set; }
    }
}
