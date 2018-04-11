namespace ImagesDAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ImagesContext : DbContext
    {
        public ImagesContext()
            : base("name=ImagesContextConnection")
        {
        }

        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Image>()
                .HasMany(e => e.Tags)
                .WithMany(e => e.Images)
                .Map(m => m.ToTable("tblTagImage").MapLeftKey("intImageId").MapRightKey("intTagId"));
        }
    }
}
