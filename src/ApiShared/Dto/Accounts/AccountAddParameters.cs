using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Accounts;

public class AccountAddParameters
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<AccountDto> Accounts { get; set; } = null!;
}
