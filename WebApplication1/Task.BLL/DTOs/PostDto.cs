namespace BLL.DTOs;

public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<CommentItemDto> Comments { get; set; } = new();
    public List<LikeItemDto> Likes { get; set; } = new();
}