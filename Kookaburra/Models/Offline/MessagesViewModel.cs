using System;

namespace Kookaburra.Models.Offline
{
    public class MessagesViewModel
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public DateTime Time { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public bool IsRead { get; set; }
    }
}