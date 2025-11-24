using BLL.DTOs;

namespace BLL.Interfaces;

public interface IPostService
{
    Task<PostDto> CreatePostAsync(CreatePostDto dto, CancellationToken ct = default);

    Task<PostDto?> AddCommentAsync(int postId, CreateCommentDto dto, CancellationToken ct = default);

    Task<PostDto?> AddLikeAsync(int postId, CreateLikeDto dto, CancellationToken ct = default);

    Task<List<PostDto>> GetFeedAsync(int userId, CancellationToken ct = default);
}