namespace DuskProject.Source.Dialog
{
    public class TimedMessage
    {
        private int _endTime;
        private int _timeOut;
        private bool _started = false;

        public TimedMessage(string text, int timeOut)
        {
            Start(text, timeOut);
        }

        public TimedMessage(int timeOut)
        {
            _timeOut = timeOut;
        }

        public string Text { get; private set; } = string.Empty;

        public void Update()
        {
            if (!_started)
            {
                return;
            }

            if (Environment.TickCount > _endTime)
            {
                Text = string.Empty;
                _started = false;
            }
        }

        public void Start(string text, int timeOut)
        {
            _timeOut = timeOut;

            if (!string.IsNullOrEmpty(text))
            {
                Text = text;
                _started = true;
                _endTime = Environment.TickCount + _timeOut;
            }
        }

        public void Start(string text)
        {
            Start(text, _timeOut);
        }

        public void Clear()
        {
            Text = string.Empty;
            _started = false;
        }
    }
}
