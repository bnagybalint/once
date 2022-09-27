using System.Management;

namespace Once
{
    class Watcher
    {
        public ManagementEventWatcher EventWatcher { get; set; }
        public EventArrivedEventHandler EventHandler { get; set; }

        public Watcher(ManagementEventWatcher eventWatcher, EventArrivedEventHandler eventHandler)
        {
            EventWatcher = eventWatcher;
            EventHandler = eventHandler;
        }
    }

    public class ProcessWatcher
    {
        static string PROCESS_STARTED_WQL = "SELECT * FROM Win32_ProcessStartTrace";
        static string PROCESS_TERMINATED_WQL = "SELECT * FROM Win32_ProcessStopTrace";

        private List<Watcher> mWatchers;

        public ProcessWatcher()
        {
            mWatchers = new List<Watcher>();
            mWatchers.Add(new Watcher(
                new ManagementEventWatcher(new WqlEventQuery(PROCESS_STARTED_WQL)),
                this.OnProcessStarted
            ));
            mWatchers.Add(new Watcher(
                new ManagementEventWatcher(new WqlEventQuery(PROCESS_TERMINATED_WQL)),
                this.OnProcessTerminated
            ));
        }

        public void Start()
        {
            foreach (Watcher watcher in mWatchers)
            {
                watcher.EventWatcher.Start();
                watcher.EventWatcher.EventArrived += new EventArrivedEventHandler(watcher.EventHandler);
            }
        }

        public void Stop()
        {
            foreach (Watcher watcher in mWatchers)
            {
                watcher.EventWatcher.EventArrived -= new EventArrivedEventHandler(watcher.EventHandler);
                watcher.EventWatcher.Stop();
            }
        }

        public void OnProcessStarted(object sender, EventArrivedEventArgs eventArgs)
        {
            ProcessEvent ev = new ProcessEvent();
            ev.ProcessId = ExtractProcessId(eventArgs);
            ev.ProcessName = ExtractProcessName(eventArgs);
            ev.EventType = ProcessEventType.STARTED;
            
            Dispatch(ev);
        }

        public void OnProcessTerminated(object sender, EventArrivedEventArgs eventArgs)
        {
            ProcessEvent ev = new ProcessEvent();
            ev.ProcessId = ExtractProcessId(eventArgs);
            ev.ProcessName = ExtractProcessName(eventArgs);
            ev.EventType = ProcessEventType.TERMINATED;

            Dispatch(ev);
        }

        public void Dispatch(ProcessEvent ev)
        {
            Console.WriteLine("{0}: {1} (ID: {2})", ev.EventType.ToString(), ev.ProcessName, ev.ProcessId);

            // TODO dispatch event to trigger rules
        }

        private int ExtractProcessId(EventArrivedEventArgs eventArgs)
        {
            return Convert.ToInt32(eventArgs.NewEvent.Properties["ProcessID"].Value);
        }

        private string? ExtractProcessName(EventArrivedEventArgs eventArgs)
        {
            return Convert.ToString(eventArgs.NewEvent.Properties["ProcessName"].Value);
        }
    }
}
