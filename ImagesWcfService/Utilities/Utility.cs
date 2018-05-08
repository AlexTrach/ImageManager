using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesWcfService.Utilities
{
    static class Utility
    {
        public static bool TagArraysAreEqual(Tag[] firstTagArray, Tag[] secondTagArray)
        {
            if (firstTagArray.Length != secondTagArray.Length)
            {
                return false;
            }

            for (int i = 0; i < firstTagArray.Length; i++)
            {
                bool isTagFromFirstArrayPresentInSecondArray = false;

                for (int j = 0; j < secondTagArray.Length; j++)
                {
                    if (firstTagArray[i].Id == secondTagArray[j].Id)
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
