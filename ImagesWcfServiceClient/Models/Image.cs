using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImagesWcfServiceClient.Models
{
    public class Image
    {
        public int Id { get; internal set; }
        public string ImageName { get; set; }
        public BitmapImage ImageContent { get; set; }
        public List<Tag> Tags { get; internal set; } = new List<Tag>();
    }
}
