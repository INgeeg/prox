using Microsoft.Extensions.Options;
internal static class Api{
    public static void ConfigureApi(this WebApplication app){
        app.MapGet("/Users", GetUsers);
        app.MapGet("/Settings", GetSettings);
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

    private static IResult GetSettings(
        IConfiguration config,
        IOptions<ExampleSettings> options,
        IOptionsMonitor<ExampleSettings> optionsMonitor,
        IOptionsSnapshot<ExampleSettings> optionsSnapshot
    ){

        try
        {
           return Results.Ok(new {
               config = config.GetValue<string>("ExampleSetting:One"),
               optionValue = options.Value.One,
               optionMonitor = optionsMonitor.CurrentValue.One,
               optionSnapshot = optionsSnapshot.Value.One,

           }); 
        }
        catch (Exception ex)
        {
            
            return Results.Problem(ex.Message);
        }

    }
}