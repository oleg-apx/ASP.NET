namespace ReactSpaSameHost;

/// <summary>
/// SPA-middleware: <see cref="UseSpaExtensions.UseSpa"/>,
/// в режиме разработки — <see cref="SpaApplicationBuilderExtensions.UseProxyToSpaDevelopmentServer"/>.
/// </summary>
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/dist");
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseStaticFiles();
        var dist = Path.Combine(env.ContentRootPath, "ClientApp", "dist");
        if (Directory.Exists(dist))
        {
            app.UseSpaStaticFiles();
        }

        app.UseEndpoints(endpoints => endpoints.MapControllers());

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "ClientApp";

            if (env.IsDevelopment())
            {
                // Фронтенд собирается отдельно (Vite). Запросы к SPA проксируются на dev-server.
                spa.UseProxyToSpaDevelopmentServer("http://localhost:5173");
            }
        });
    }
}
