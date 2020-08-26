using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(AspNetCoreCustomIdentyJwtDemo.Areas.Identity.IdentityHostingStartup))]

namespace AspNetCoreCustomIdentyJwtDemo.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}