using System.ComponentModel.DataAnnotations;
namespace GraphQLNet.Notes;

public class Note
{
  public Guid Id { get; set; }
  [Required]
  public string Message { get; set; }
} 