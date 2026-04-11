using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Accounts;

public class DeleteAccountParameters
{
    [Required]
    public Guid AccountId { get; set; }
}
