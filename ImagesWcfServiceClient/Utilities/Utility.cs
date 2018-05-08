using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImagesWcfServiceClient.Models;

namespace ImagesWcfServiceClient.Utilities
{
    static class Utility
    {
        public static bool CheckWhetherTagCollectionsAreEqual(List<Tag> firstTagCollection, List<Tag> secondTagCollection)
        {
            if (firstTagCollection.Count != secondTagCollection.Count)
            {
                return false;
            }

            for (int i = 0; i < firstTagCollection.Count; i++)
            {
                bool isTagFromFirstArrayPresentInSecondArray = false;

                for (int j = 0; j < secondTagCollection.Count; j++)
                {
                    if (firstTagCollection[i].Id == secondTagCollection[j].Id)
                    {
                        isTagFromFirstArrayPresentInSecondArray = true;
                        break;
                    }
                }

                if (!isTagFromFirstArrayPresentInSecondArray)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
