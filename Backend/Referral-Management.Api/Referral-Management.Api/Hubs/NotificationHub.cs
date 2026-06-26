using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Referral_Management.Api.Hubs
{
    [AllowAnonymous] // ✅ IMPORTANT
    public class NotificationHub : Hub
    {
        // ✅ UPDATED METHOD NAME (VERY IMPORTANT FIX)
        public async Task AddToGroup(string groupName)
        {
            try
            {
                Console.WriteLine($"🔥 AddToGroup HIT: {groupName}");

                if (string.IsNullOrWhiteSpace(groupName))
                {
                    Console.WriteLine("❌ Group name is empty");
                    return;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                Console.WriteLine($"✅ Joined group: {groupName}");
            }
            catch (Exception ex)
            {
                // ✅ FULL ERROR LOG (IMPORTANT)
                Console.WriteLine($"❌ AddToGroup ERROR FULL: {ex}");
            }
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"✅ Connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"❌ Disconnected: {exception?.Message}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}