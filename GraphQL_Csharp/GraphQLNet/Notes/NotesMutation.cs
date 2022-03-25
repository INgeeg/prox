using GraphQL;
using GraphQL.Types;
using GraphQLNet.EntityFramework;

namespace GraphQLNet.Notes
{
    public class NotesMutation : ObjectGraphType
    {
        public NotesMutation()
        {
            Field<NoteType>(
                "createNote",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "message"}
                ),
                resolve: context =>
                {
                    var message = context.GetArgument<string>("message");
                    var notesContext = context.RequestServices.GetRequiredService<NotesContext>();
                    var note = new Note
                    {
                        Message = message,
                    };
                    notesContext.Notes.Add(note);
                    notesContext.SaveChanges();
                    return note;
                }
            );
        }
    }
}