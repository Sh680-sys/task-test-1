using DAL.Entities;

namespace DAL.Interfaces;

public interface IPostRepository : IRepository<Post>
{
    Task<Post?> GetPostWithDetailsAsync(int postId, CancellationToken ct = default);
    Task<List<Post>> GetFeedAsync(int userId, int take = 50, CancellationToken ct = default);
}