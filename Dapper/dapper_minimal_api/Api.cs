using Microsoft.Extensions.Options;
using Polly;

internal static class Api{
    public static void ConfigureApi(this WebApplication app){
        app.MapGet("/Users", GetUsers);
        app.MapGet("/Settings", GetSettings);
        app.MapGet("/Error", Error);
    }

    private static async Task<IResult> GetUsers(IUserData data){
        try
        {
           return Results.Ok(await data.GetUsers()); 
        }
        catch (Exception ex)
        {
            
            return Results.Problem(ex.Message);
        }


        
    }
    
    private static async Task<IResult> Error(){
            return Results.Problem();
    }

    private static IResult GetSettings(
        IConfiguration config,
        IOptions<ExampleSettings> options,
        IOptionsMonitor<ExampleSettings> optionsMonitor,
        IOptionsSnapshot<ExampleSettings> optionsSnapshot
    ){

        try
        {
            var retryPolicy = Policy.Handle<Exception>()
                .RetryAsync(2, async (ex, count, context) =>
                {
                    //(config as IConfigurationRoot).Reload();
                });

            return Results.Ok(new {
               config = config.GetValue<string>("ExampleSetting:One"),
               optionValueSingleton = options.Value.One,
               optionMonitorTransient = optionsMonitor.CurrentValue.One,
               optionSnapshotScoped = optionsSnapshot.Value.One,               
               optionMonitorTransientTwo = optionsMonitor.CurrentValue.Two,


           }); 
        }
        catch (Exception ex)
        {
            
            return Results.Problem(ex.Message);
        }

    }
}