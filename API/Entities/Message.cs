using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }

        public AppUser Sender { get; set; }

        public int RecicipientId { get; set; }
        public string RecipientUsername { get; set; }
        public AppUser Recipient { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;
        public bool SenderDeleted { get; set; } // Each user should only control the visibility of the messages from their side
        public bool RecipientDeleted { get; set; } // Each user should only control the visibility of the messages from their side
        // Physically removed message when both the sender and receiver have deleted the message
    }
}