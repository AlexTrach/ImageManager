using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ImagesDal
{
    public class ImagesRepository
    {
        public ImagesContext Context { get; } = new ImagesContext();

        public int AddImage(Image image)
        {
            Context.Images.Add(image);
            return SaveChanges();
        }

        public Task<int> AddImageAsync(Image image)
        {
            Context.Images.Add(image);
            return SaveChangesAsync();
        }

        public int UpdateImage(Image image)
        {
            Context.Entry(image).State = EntityState.Modified;
            return SaveChanges();
        }

        public Task<int> UpdateImageAsync(Image image)
        {
            Context.Entry(image).State = EntityState.Modified;
            return SaveChangesAsync();
        }

        public int DeleteImage(int id)
        {
            Context.Images.Remove(Context.Images.Find(id));
            return SaveChanges();
        }

        public Task<int> DeleteImageAsync(int id)
        {
            Context.Images.Remove(Context.Images.Find(id));
            return SaveChangesAsync();
        }

        public Image GetOneImage(int id)
        {
            return Context.Images.Find(id);
        }

        public Task<Image> GetOneImageAsync(int id)
        {
            return Context.Images.FindAsync(id);
        }

        public List<Image> GetAllImages()
        {
            return Context.Images.ToList();
        }

        public Task<List<Image>> GetAllImagesAsync()
        {
            return Context.Images.ToListAsync();
        }

        public List<Image> GetSpecifiedRangeOfImages(int startIndex, int count)
        {
            return Context.Images.OrderBy(image => image.Id).Skip(startIndex).Take(count).ToList();
        }

        public Task<List<Image>> GetSpecifiedRangeOfImagesAsync(int startIndex, int count)
        {
            return Context.Images.OrderBy(image => image.Id).Skip(startIndex).Take(count).ToListAsync();
        }

        public List<Image> GetSpecifiedRangeFromImagesWithSuchTags(int startIndex, int count, List<Tag> tags)
        {
            int[] ids = new int[tags.Count];
            for (int i = 0; i < tags.Count; i++)
            {
                ids[i] = tags[i].Id;
            }

            IQueryable<Tag> tagsToSearchBy = Context.Tags.Where(tagToSearchBy => ids.Contains(tagToSearchBy.Id));

            IQueryable<Image> queryResult = Context.Images.Where(image => image.Tags.Intersect(tagsToSearchBy).Count() == tagsToSearchBy.Count());

            return queryResult.OrderBy(image => image.Id).Skip(startIndex).Take(count).ToList();
        }

        public Task<List<Image>> GetSpecifiedRangeFromImagesWithSuchTagsAsync(int startIndex, int count, List<Tag> tags)
        {
            int[] ids = new int[tags.Count];
            for (int i = 0; i < tags.Count; i++)
            {
                ids[i] = tags[i].Id;
            }

            IQueryable<Tag> tagsToSearchBy = Context.Tags.Where(tagToSearchBy => ids.Contains(tagToSearchBy.Id));

            IQueryable<Image> queryResult = Context.Images.Where(image => image.Tags.Intersect(tagsToSearchBy).Count() == tagsToSearchBy.Count());

            return queryResult.OrderBy(image => image.Id).Skip(startIndex).Take(count).ToListAsync();
        }

        public int AddTag(Tag tag)
        {
            Context.Tags.Add(tag);
            return SaveChanges();
        }

        public Task<int> AddTagAsync(Tag tag)
        {
            Context.Tags.Add(tag);
            return SaveChangesAsync();
        }

        public int UpdateTag(Tag tag)
        {
            Context.Entry(tag).State = EntityState.Modified;
            return SaveChanges();
        }

        public Task<int> UpdateTagAsync(Tag tag)
        {
            Context.Entry(tag).State = EntityState.Modified;
            return SaveChangesAsync();
        }

        public int DeleteTag(int id)
        {
            Context.Tags.Remove(Context.Tags.Find(id));
            return SaveChanges();
        }

        public Task<int> DeleteTagAsync(int id)
        {
            Context.Tags.Remove(Context.Tags.Find(id));
            return SaveChangesAsync();
        }

        public Tag GetOneTag(int id)
        {
            return Context.Tags.Find(id);
        }

        public Task<Tag> GetOneTagAsync(int id)
        {
            return Context.Tags.FindAsync(id);
        }

        public List<Tag> GetAllTags()
        {
            return Context.Tags.ToList();
        }

        public Task<List<Tag>> GetAllTagsAsync()
        {
            return Context.Tags.ToListAsync();
        }

        public List<Tag> GetSpecifiedRangeOfTags(int startIndex, int count)
        {
            return Context.Tags.OrderBy(tag => tag.Id).Skip(startIndex).Take(count).ToList();
        }

        public Task<List<Tag>> GetSpecifiedRangeOfTagsAsync(int startIndex, int count)
        {
            return Context.Tags.OrderBy(tag => tag.Id).Skip(startIndex).Take(count).ToListAsync();
        }

        protected int SaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected async Task<int> SaveChangesAsync()
        {
            try
            {
                return await Context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool _disposed = false;

        public void Dispose()
        {
            CleanUp(true);
            GC.SuppressFinalize(this);
        }

        private void CleanUp(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            _disposed = true;
        }

        ~ImagesRepository()
        {
            CleanUp(false);
        }
    }
}
