using GraphQLNet.Notes;
using Microsoft.EntityFrameworkCore;

namespace GraphQLNet.EntityFramework;

public class NotesContext : DbContext
{
    public DbSet<Note> Notes { get; set; }
    public DbSet<Todo> Todos { get; set; }

    public NotesContext(DbContextOptions options) : base(options)
    {

    }
}
