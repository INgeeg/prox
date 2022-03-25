using System.Linq;
using HotChocolate;
using TodoListQL.Data;
using TodoListQL.Models;

namespace TodoListQL.GraphQL
{
    public class Query
    {
        // Will return all of our todo list items
        // We are injecting the context of our dbConext to access the db
        // public IQueryable<ItemData> GetItem([Service] ApiDbContext context)
        // {
        //     return context;
        // }

        public IQueryable<ItemList> GetList([Service] ApiDbContext context){
            return context.Lists;
        }
    }


}