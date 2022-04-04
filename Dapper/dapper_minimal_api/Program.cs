using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Dapper.DbAccess;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"config/appsettings.{builder.Environment.EnvironmentName}.json", 
    optional: false,
    reloadOnChange: true);
var cred = new ClientSecretCredential(builder.Configuration["KeyVaultConfig:TenantId"], builder.Configuration["KeyVaultConfig:ClientId"], builder.Configuration["KeyVaultConfig:ClientSecretId"]);
var client = new SecretClient(new Uri(builder.Configuration["KeyVaultConfig:KVUrl"]), cred);
builder.Configuration.AddAzureKeyVault(client, new AzureKeyVaultConfigurationOptions()
{
    ReloadInterval = TimeSpan.FromMinutes(10)
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddSingleton<IUserData, UserData>();
builder.Services.Configure<ExampleSettings>(builder.Configuration.GetSection("ExampleSetting")); 


var app = builder.Build();

if(app.Environment.IsDevelopment()){
    //app.UseSwagger();
    //app.UseSwaggerUI();
}
app.UseExceptionHandler("/Error");
app.UseSwagger();
app.UseSwaggerUI();
//app.UseHttpsRedirection();
app.ConfigureApi();

app.Run();

