using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Enums;
using DAL.Interfaces;

namespace BLL.Services;

public class PostService(
    IPostRepository postRepo,
    IRepository<User> userRepo,
    IRepository<Comment> commentRepo,
    IRepository<Like> likeRepo,
    IRepository<Notification> notifRepo,
    IUnitOfWork uow,
    IMapper mapper) : IPostService
{
    private readonly IPostRepository _postRepo = postRepo;
    private readonly IRepository<User> _userRepo = userRepo;
    private readonly IRepository<Comment> _commentRepo = commentRepo;
    private readonly IRepository<Like> _likeRepo = likeRepo;
    private readonly IRepository<Notification> _notifRepo = notifRepo;
    private readonly IUnitOfWork _uow = uow;
    private readonly IMapper _mapper = mapper;

    public async Task<PostDto> CreatePostAsync(CreatePostDto dto, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(dto.UserId, ct) ?? throw new ArgumentException("User not found");

        var post = new Post
        {
            UserId = user.Id,
            Content = dto.Content.Trim()
        };

        await _postRepo.AddAsync(post, ct);
        await _uow.SaveChangesAsync(ct);

        var full = await _postRepo.GetPostWithDetailsAsync(post.Id, ct) ?? post;
        return _mapper.Map<PostDto>(full);
    }

    public async Task<PostDto?> AddCommentAsync(int postId, CreateCommentDto dto, CancellationToken ct = default)
    {
        var post = await _postRepo.GetByIdAsync(postId, ct);
        if (post is null) return null;

        var user = await _userRepo.GetByIdAsync(dto.UserId, ct) ?? throw new ArgumentException("User not found");

        var comment = new Comment
        {
            PostId = post.Id,
            UserId = user.Id,
            Content = dto.Content.Trim()
        };

        await _commentRepo.AddAsync(comment, ct);

        // هون منخلق إشعار لصاحب البوست إذا اللي علّق مش هو نفسو
        if (post.UserId != user.Id)
        {
            var notif = new Notification
            {
                UserId = post.UserId,
                Type = NotificationType.Comment,
                Message = $"{user.UserName} commented on your post",
                PostId = post.Id,
                IsRead = false
            };
            await _notifRepo.AddAsync(notif, ct);
        }

        await _uow.SaveChangesAsync(ct);

        var full = await _postRepo.GetPostWithDetailsAsync(post.Id, ct);
        return full is null ? null : _mapper.Map<PostDto>(full);
    }

    public async Task<PostDto?> AddLikeAsync(int postId, CreateLikeDto dto, CancellationToken ct = default)
    {
        var post = await _postRepo.GetByIdAsync(postId, ct);
        if (post is null) return null;

        var user = await _userRepo.GetByIdAsync(dto.UserId, ct) ?? throw new ArgumentException("User not found");

        // هون منمنع الدوبلكيت باستخدام Unique index + منجرب نضيف إذا مو موجود
        var existing = (await _likeRepo.FindAsync(l => l.PostId == post.Id && l.UserId == user.Id, ct)).FirstOrDefault();
        if (existing is null)
        {
            var like = new Like { PostId = post.Id, UserId = user.Id };
            await _likeRepo.AddAsync(like, ct);

            if (post.UserId != user.Id)
            {
                var notif = new Notification
                {
                    UserId = post.UserId,
                    Type = NotificationType.Like,
                    Message = $"{user.UserName} liked your post",
                    PostId = post.Id,
                    IsRead = false
                };
                await _notifRepo.AddAsync(notif, ct);
            }

            await _uow.SaveChangesAsync(ct);
        }

        var full = await _postRepo.GetPostWithDetailsAsync(post.Id, ct);
        return full is null ? null : _mapper.Map<PostDto>(full);
    }

    public async Task<List<PostDto>> GetFeedAsync(int userId, CancellationToken ct = default)
    {
        // هون ما في فولو لحدا، فعم نجيب آخر بوستات
        var posts = await _postRepo.GetFeedAsync(userId, 50, ct);
        return posts.Select(_mapper.Map<PostDto>).ToList();
    }
}