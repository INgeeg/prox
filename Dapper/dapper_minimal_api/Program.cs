using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Dapper.DbAccess;

using GraphQL.Data;
using GraphQL.GraphQL;
using GraphQL.GraphQL.DataItem;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Mongo.Models;
using Mongo.Services;
using L = GraphQL.GraphQL.Lists;


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

builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddType<ItemType>()
    .AddType<L.ListType>()
    .AddMutationType<Mutation>()
    .AddProjections()
    .AddSorting()
    .AddFiltering();
builder.Services.AddPooledDbContextFactory<ApiDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("Default")
    ));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddSingleton<IUserData, UserData>();
builder.Services.Configure<ExampleSettings>(builder.Configuration.GetSection("ExampleSetting")); 
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(nameof(DatabaseSettings)));
//builder.Services.AddSingleton<IDatabaseSettings>(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);


builder.Services.AddSingleton<MongoService>();

builder.Services.AddScoped<IProductService, ProductService>();


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
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
    endpoints.MapControllers();
});

app.Run();

