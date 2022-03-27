using GraphQLHotChocolate.Data;
using GraphQLHotChocolate.Models;

namespace GraphQLHotChocolate.GraphQL;
public class Query
{
    // Will return all of our todo list items
    // We are injecting the context of our dbConext to access the db
    [UseDbContext(typeof(ApiDbContext))]  
    [UseProjection]  
    public IQueryable<ItemData> GetItems([ScopedService] ApiDbContext context)
    {
        return context.Items;
    }

    [UseDbContext(typeof(ApiDbContext))]
    [UseProjection]
    public IQueryable<ItemList> GetLists([ScopedService] ApiDbContext context)
    {
        return context.Lists;
    }
}