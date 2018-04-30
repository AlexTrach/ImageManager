using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ImagesDal
{
    public class ImageRepository : BaseRepository<Image>
    {
        public ImageRepository()
        {
            Table = Context.Images;
        }

        public override int Delete(int id)
        {
            Context.Entry(new Image { Id = id }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public override Task<int> DeleteAsync(int id)
        {
            Context.Entry(new Image { Id = id }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        public override List<Image> GetAllEagerly()
        {
            return Table.Include(image => image.Tags).ToList();
        }

        public override Task<List<Image>> GetAllEagerlyAsync()
        {
            return Table.Include(image => image.Tags).ToListAsync();
        }

        public override List<Image> GetSpecifiedRange(int startIndex, int count)
        {
            return Table.OrderBy(image => image.Id).Skip(startIndex).Take(count).ToList();
        }

        public override Task<List<Image>> GetSpecifiedRangeAsync(int startIndex, int count)
        {
            return Table.OrderBy(image => image.Id).Skip(startIndex).Take(count).ToListAsync();
        }

        public List<Image> GetSpecifiedRangeFromImagesWithSuchTags(int startIndex, int count, List<Tag> tags)
        {
            IQueryable<Image> queryResult = (IQueryable<Image>) new List<Image>();
            for (int i = 0; i < tags.Count; i++)
            {
                if (i == 0)
                {
                    queryResult = Context.Entry(tags[i]).Collection(tag => tag.Images).Query();
                }
                else
                {
                    queryResult = Context.Entry(tags[i]).Collection(currentTag => currentTag.Images).Query().Intersect(queryResult);
                }
            }

            return queryResult.OrderBy(image => image.Id).Skip(startIndex).Take(count).ToList();
        }

        public Task<List<Image>> GetSpecifiedRangeFromImagesWithSuchTagsAsync(int startIndex, int count, List<Tag> tags)
        {
            IQueryable<Image> queryResult = null;
            for (int i = 0; i < tags.Count; i++)
            {
                if (i == 0)
                {
                    queryResult = Context.Entry(tags[i]).Collection(tag => tag.Images).Query();
                }
                else
                {
                    queryResult = Context.Entry(tags[i]).Collection(currentTag => currentTag.Images).Query().Intersect(queryResult);
                }
            }

            return queryResult.OrderBy(image => image.Id).Skip(startIndex).Take(count).ToListAsync();
        }
    }
}
