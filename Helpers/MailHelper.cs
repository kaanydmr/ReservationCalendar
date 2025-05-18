using Project.Data;
using Project.Models;
using System;
using System.Threading.Tasks;

namespace Project.Helpers
{
    public static class MailHelper
    {
        public static async Task SendMessageAsync(
            ReservationDbContext context,
            int senderUserId,
            string subject,
            string message)
        {
            var contactMessage = new ContactMessage
            {
                UserId = senderUserId,      // Recipient of the message
                Subject = subject,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.Now
                // Optionally, add a SenderId property to ContactMessage if you want to track sender
            };

            context.ContactMessages.Add(contactMessage);
            await context.SaveChangesAsync();
        }
    }
}