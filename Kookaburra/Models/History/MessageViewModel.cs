using System;

namespace Kookaburra.Models.History
{
    public class MessageViewModel
    {
        public string Name { get; set; }

        public bool IsVisitor { get; set; }

        public int Message { get; set; }

        public DateTime DateSent { get; set; }
    }
}