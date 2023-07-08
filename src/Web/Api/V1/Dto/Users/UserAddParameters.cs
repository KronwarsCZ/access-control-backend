using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public class UserAddParameters
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<UserDto> Users { get; set; }
}
