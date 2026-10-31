using System.ComponentModel.DataAnnotations;

namespace library.Models;

public class Member
{
    public long MemberId { get; set; }

    [Required]
    public string FullName { get; set; } = "";

    public string? Email { get; set; }

    [Required]
    public string MemberType { get; set; } = "";
}