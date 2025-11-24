using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs;

public class CreatePostDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}

public class CreateCommentDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
}

public class CreateLikeDto
{
    [Required]
    public int UserId { get; set; }
}