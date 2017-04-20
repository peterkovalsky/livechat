namespace Kookaburra.Domain
{
    public struct Duration
    {
        public Duration(int minutes, int seconds)
        {
            Minutes = minutes;
            Seconds = seconds;
        }

        public int Minutes { get; }

        public int Seconds { get; }
    }
}