using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ImagesDal
{
    public class TagRepository : BaseRepository<Tag>
    {
        public TagRepository()
        {
            Table = Context.Tags;
        }

        public override int Delete(int id)
        {
            Context.Entry(new Tag { Id = id }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public override Task<int> DeleteAsync(int id)
        {
            Context.Entry(new Tag { Id = id }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        public override List<Tag> GetAllEagerly()
        {
            return Table.Include(tag => tag.Images).ToList();
        }

        public override Task<List<Tag>> GetAllEagerlyAsync()
        {
            return Table.Include(tag => tag.Images).ToListAsync();
        }

        public override List<Tag> GetSpecifiedRange(int startIndex, int count)
        {
            return Table.OrderBy(tag => tag.Id).Skip(startIndex).Take(count).ToList();
        }

        public override Task<List<Tag>> GetSpecifiedRangeAsync(int startIndex, int count)
        {
            return Table.OrderBy(tag => tag.Id).Skip(startIndex).Take(count).ToListAsync();
        }
    }
}
