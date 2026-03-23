
using VRChat.API.Client;
using VRChat.API.Realtime;

namespace VRChatAspRealtimeExample
{
    public class VRChatRealtimeService : BackgroundService
    {
        private readonly IVRChat _vrchat;
        private readonly ILogger<VRChatRealtimeService> _logger;

        public VRChatRealtimeService(IVRChat vrchat, ILogger<VRChatRealtimeService> logger)
        {
            _vrchat = vrchat;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var user = await _vrchat.LoginAsync(stoppingToken);

            _logger.LogInformation("Currently logged in as {user}", user.DisplayName);

            var authToken = _vrchat.GetCookies().FirstOrDefault(c => c.Name == "auth")?.Value;

            var realtime = new VRChatRealtimeClientBuilder()
                .WithApplication(name: "VRChatAspRealtimeExample", version: "1.0.0", contact: "hello@example.com")
                .WithAutoReconnect(AutoReconnectMode.Every10Minutes)
                .WithAuthToken(authToken)
                .Build();

            realtime.Log += Realtime_Log;
            realtime.OnFriendLocation += Realtime_OnFriendLocation;
            realtime.OnDisconnected += Realtime_OnDisconnected;

            await realtime.ConnectAsync(stoppingToken);

            _logger.LogInformation("Connected to VRChat Pipeline");
            await Task.Delay(-1, stoppingToken);
        }

        private void Realtime_OnDisconnected(object? sender, EventArgs e)
        {
            _logger.LogWarning("Disconnected from VRChat Pipeline");
        }

        private void Realtime_OnFriendLocation(object? sender, VRChat.API.Realtime.Models.VRChatEventArgs<VRChat.API.Realtime.Messages.FriendLocationContent> e)
        {
            _logger.LogInformation("Friend {user} location updated to: {location}", e.Message.UserId, e.Message.WorldId);
        }

        private void Realtime_Log(object? sender, VRChat.API.Realtime.Models.LogEventArgs e)
        {
            _logger.LogInformation(e.Message);

            // You can build this out way more if you need to.
        }
    }
}