using System.ComponentModel.DataAnnotations;
namespace GraphQLNet.Notes;

public class Todo
{
  public Guid Id { get; set; }
  [Required]
  public string Note { get; set; }
} 