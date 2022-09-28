using System.Management;

namespace Once
{
    class Watcher
    {
        public ManagementEventWatcher EventWatcher { get; set; }
        public EventArrivedEventHandler EventHandler { get; set; }

        public Watcher(ManagementEventWatcher eventWatcher, EventArrivedEventHandler eventHandler)
        {
            this.EventWatcher = eventWatcher;
            this.EventHandler = eventHandler;
        }
    }

    public class ProcessWatcher
    {
        private static string PROCESS_STARTED_WQL = "SELECT * FROM Win32_ProcessStartTrace";
        private static string PROCESS_TERMINATED_WQL = "SELECT * FROM Win32_ProcessStopTrace";

        private List<Watcher> Watchers = new();
        
        public delegate void ProcessEventHandler(ProcessEventArgs ev);
        public event ProcessEventHandler? ProcessEvent;

        public ProcessWatcher()
        {
            this.Watchers.Add(new Watcher(
                new ManagementEventWatcher(new WqlEventQuery(PROCESS_STARTED_WQL)),
                this.HandleProcessStarted
            ));
            this.Watchers.Add(new Watcher(
                new ManagementEventWatcher(new WqlEventQuery(PROCESS_TERMINATED_WQL)),
                this.HandleProcessTerminated
            ));
        }

        public void Start()
        {
            foreach (Watcher watcher in this.Watchers)
            {
                watcher.EventWatcher.Start();
                watcher.EventWatcher.EventArrived += new EventArrivedEventHandler(watcher.EventHandler);
            }
        }

        public void Stop()
        {
            foreach (Watcher watcher in this.Watchers)
            {
                watcher.EventWatcher.EventArrived -= new EventArrivedEventHandler(watcher.EventHandler);
                watcher.EventWatcher.Stop();
            }
        }

        private void HandleProcessStarted(object sender, EventArrivedEventArgs eventArgs)
        {
            ProcessEventArgs ev = new ProcessEventArgs(
                ExtractProcessId(eventArgs),
                ExtractProcessName(eventArgs),
                ProcessEventType.STARTED
            );
            
            this.Dispatch(ev);
        }

        private void HandleProcessTerminated(object sender, EventArrivedEventArgs eventArgs)
        {
            ProcessEventArgs ev = new ProcessEventArgs(
                ExtractProcessId(eventArgs),
                ExtractProcessName(eventArgs),
                ProcessEventType.TERMINATED
            );

            this.Dispatch(ev);
        }

        private void Dispatch(ProcessEventArgs ev)
        {
            Console.WriteLine("Event: {0}, {1} (ID: {2})", ev.EventType.ToString(), ev.ProcessName, ev.ProcessId);

            ProcessEventHandler? handler = this.ProcessEvent;
            handler?.Invoke(ev);
        }

        private int ExtractProcessId(EventArrivedEventArgs eventArgs)
        {
            return Convert.ToInt32(eventArgs.NewEvent.Properties["ProcessID"].Value);
        }

        private string ExtractProcessName(EventArrivedEventArgs eventArgs)
        {
            return Convert.ToString(eventArgs.NewEvent.Properties["ProcessName"].Value);
        }
    }
}
