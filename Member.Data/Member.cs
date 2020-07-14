using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Member.Data
{
    public class Member
    {
        [Key] public int MemberId { get; set; }

        [Required] public string Surname { get; set; }

        [Required] public string Name { get; set; }

        [Required] public bool IsDeleted { get; set; }

        [ForeignKey("MemberFK")] public Psa Psa { get; set; }
    }
}