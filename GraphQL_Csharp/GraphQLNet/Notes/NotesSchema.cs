using GraphQL.Types;

namespace GraphQLNet.Notes;

public class NotesSchema : Schema
{
  public NotesSchema(IServiceProvider serviceProvider) : base(serviceProvider)
  {
    Query = serviceProvider.GetRequiredService<NotesQuery>();
    Mutation = serviceProvider.GetRequiredService<NotesMutation>();
  }
}