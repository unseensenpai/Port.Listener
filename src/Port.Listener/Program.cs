using Hangfire;
using Hangfire.MemoryStorage;
using Newtonsoft.Json;
using PortListener.Concrete;
using PortListener.Contract;
using PortListener.DAL;
using PortListener.Jobs;
using PortListener.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
ConfigureHostConfigs(builder);
ConfigureServices(builder);
var app = builder.Build();
ConfigureApp(app);
app.Run();

static void ConfigureHostConfigs(WebApplicationBuilder builder)
{
    builder.Configuration.AddJsonFile("appsettings.json", true, true);
}

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    RegisterHangfire(builder.Services);
    RegisterDependencies(builder.Services);
    RegisterLogger(builder.Services);
    RegisterMapper(builder.Services);
    RegisterXPO(builder);
}



static void ConfigureApp(WebApplication app)
{

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI((opts) =>
        {
            opts.DocumentTitle = "Post Listener API";
            opts.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        });
    }
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthorization();
    app.UseHangfireDashboard("/hangfire", new DashboardOptions { DashboardTitle = "Port Listener Hangfire" });
    app.MapGet("/", context => Task.Run(() => context.Response.Redirect("/swagger/index.html")));
    app.MapControllers();

    BackgroundRecurringJobManager.EnqueuePingJob();
}

static void RegisterDependencies(IServiceCollection services)
{
    services.AddTransient<IPortCheckService, PortCheckService>();
}

static void RegisterXPO(WebApplicationBuilder builder)
{
    builder.Services.AddXpoDefaultSession();
    builder.Services.AddXpoDefaultUnitOfWork(true, (options) =>
        options
        //.UseConnectionString()
        .UseConnectionString(builder.Configuration.GetConnectionString("PostgreSql"))
        // Pass all of your persistent object types to this method.
        .UseEntityTypes(new Type[] { typeof(PortsDAL) })
        .UseThreadSafeDataLayer(true)
        );
}

static void RegisterLogger(IServiceCollection services)
{
    services.AddLogging((loggingBuilder) =>
    {
        loggingBuilder.AddSerilog(dispose: true);
        loggingBuilder.AddConsole();
    });
}

static void RegisterMapper(IServiceCollection services)
{
    services.AddAutoMapper(x => x.CreateMap<PortsModel, PortsDAL>().ReverseMap().ForMember(fm => fm.Address, opt => opt.MapFrom(src => src.IpAddress)));
}
static void RegisterHangfire(IServiceCollection services)
{
    services.AddHangfire(config =>
    {
        config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseMemoryStorage()
            .UseSerializerSettings(new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
    });
    services.AddHangfireServer();
}