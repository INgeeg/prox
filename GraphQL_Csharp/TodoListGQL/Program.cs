
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Server.Ui.Voyager;
using HotChocolate.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoListQL.Data;




var builder = WebApplication.CreateBuilder(args);
ConfigurationManager Configuration = builder.Configuration;
IWebHostEnvironment env = builder.Environment;

builder.Services.AddPooledDbContextFactory<ApiDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")
                ));
// builder.Services.AddGraphQLServer()
//                         .AddQueryType<Query>()
//                         .AddType<ListType>()
//                         .AddProjections()
//                         .AddMutationType<Mutation>()
//                         .AddFiltering()
//                         .AddSorting();

var app = builder.Build();

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapGraphQL();
// });

// app.UseGraphQLVoyager(new VoyagerOptions()
// {
//     GraphQLEndPoint = "/graphql"
// }, "/graphql-ui");

app.Run();
