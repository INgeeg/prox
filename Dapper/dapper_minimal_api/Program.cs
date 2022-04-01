using Dapper.DbAccess;
using Microsoft.AspNetCore.Mvc.ApplicationParts;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"config/appsettings.{builder.Environment.EnvironmentName}.json", 
    optional: false,
    reloadOnChange: true); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddSingleton<IUserData, UserData>();
builder.Services.Configure<ExampleSettings>(builder.Configuration.GetSection("ExampleSetting")); 


var app = builder.Build();

if(app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseHttpsRedirection();
app.ConfigureApi();

app.Run();
