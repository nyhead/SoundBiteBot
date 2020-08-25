using Microsoft.EntityFrameworkCore;

namespace SoundBiteBot
{
    public class SoundContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=soundbite.db");        
    }

    public class  Post
    {
        public int Id { get; set; }
        public string PostId { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string FileId { get; set; }
    }
}
