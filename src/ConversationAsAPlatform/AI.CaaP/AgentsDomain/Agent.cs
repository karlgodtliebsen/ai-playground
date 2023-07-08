using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AI.CaaP.Domain;

namespace AI.CaaP.AgentsDomain;

public class Agent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Owner user id
    /// </summary>
    public Guid OwnerId { get; set; }


    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; set; } = default!;

    /// <summary>
    /// Instruction
    /// </summary>
    [StringLength(Int32.MaxValue)]
    public string Instruction { get; set; } = default!;

    /// <summary>
    /// Samples
    /// </summary>
    [StringLength(Int32.MaxValue)]
    public string? Samples { get; set; } = default!;

    /// <summary>
    /// Domain knowledges
    /// </summary>
    [StringLength(Int32.MaxValue)]
    public string? Knowledges { get; set; } = default!;

    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;

    [NotMapped]
    public IList<Conversation> Messages { get; set; } = new List<Conversation>();

}
