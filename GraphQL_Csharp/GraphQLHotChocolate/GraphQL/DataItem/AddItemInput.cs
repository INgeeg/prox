namespace GraphQLHotChocolate.GraphQL.DataItem;

public record AddItemInput(string title, string description, bool done, int listId);
