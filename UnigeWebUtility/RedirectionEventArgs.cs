namespace UnigeWebUtility
{
    using System;

    public class RedirectionEventArgs : EventArgs
    {
        public Uri RequestedUri { get; set; }

        public Uri RedirectionUri { get; set; }
    }
}