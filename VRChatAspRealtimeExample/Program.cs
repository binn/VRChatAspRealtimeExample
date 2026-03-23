using VRChat.API.Extensions.Hosting;

namespace VRChatAspRealtimeExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddVRChat(vr => vr
                .WithUsername(builder.Configuration["VRChatUsername"])
                .WithPassword(builder.Configuration["VRChatPassword"])
                .WithTwoFactorSecret(builder.Configuration["VRChatTwoFactorSecret"])
                .WithApplication(name: "VRChatAspRealtimeExample", version: "1.0.0", contact: "hello@example.com")
            );

            builder.Services.AddHostedService<VRChatRealtimeService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
