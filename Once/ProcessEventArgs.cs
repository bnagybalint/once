namespace Once {
    public enum ProcessEventType {
        STARTED,
        TERMINATED,
    }

    public struct ProcessEventArgs
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public ProcessEventType EventType { get; set; }

        public ProcessEventArgs(int processId, string processName, ProcessEventType eventType)
        {
            this.ProcessId = processId;
            this.ProcessName = processName;
            this.EventType = eventType;
        }
    }
}
