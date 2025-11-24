using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class PostRepository(AppDbContext context) : GenericRepository<Post>(context), IPostRepository
{
    public async Task<Post?> GetPostWithDetailsAsync(int postId, CancellationToken ct = default)
    {
        // هون جبنا البوست مع الكومنتات واللايكات تبعو عشان نرجع الداتا كاملة
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .Include(p => p.Likes).ThenInclude(l => l.User)
            .FirstOrDefaultAsync(p => p.Id == postId, ct);
    }

    public async Task<List<Post>> GetFeedAsync(int userId, int take = 50, CancellationToken ct = default)
    {
        // مش عاملين فولو/فولونج، فهلا بنرجع آخر بوستات عالسريع
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .ToListAsync(ct);
    }
}