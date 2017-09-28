namespace ProgressButtonDemo
{
    public class ProgressStateChangingEventArgs
    {
        public ProgressStateChangingEventArgs(ProgressState oldValue, ProgressState newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public bool Cancel { get; set; }

        public ProgressState OldValue { get; }

        public ProgressState NewValue { get; }
    }
}