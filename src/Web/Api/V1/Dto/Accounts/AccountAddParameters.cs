using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public class AccountAddParameters
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<AccountDto> Accounts { get; set; }
}
