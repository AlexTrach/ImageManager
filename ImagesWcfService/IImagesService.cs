using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ImagesWcfService
{  
    [ServiceContract(CallbackContract = typeof(IImagesServiceCallback), SessionMode = SessionMode.Required)]
    public interface IImagesService
    {
        [OperationContract(IsOneWay = true, IsInitiating=true, IsTerminating = false)]
        void Subscribe();

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        Image[] GetNextThumbnails(int numberOfThumbnails, int widthOfThumbnail, bool resetToBeginning);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        Image[] GetNextThumbnailsWithSuchTags(int numberOfThumbnails, int widthOfThumbnail, Tag[] tags, bool resetToBeginning);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        Image GetThumbnail(int widthOfThumbnail, int id);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        Image GetFullSizeImage(int id);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        Tag[] GetAllTags();

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        Tag GetTag(int id);

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        void AddImage(Image image);

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        void UpdateImage(Image image);

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        void DeleteImage(int id);

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        void AddTag(Tag tag);

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        void UpdateTag(Tag tag);

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        void DeleteTag(int id);

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = true)]
        void Unsubscribe();
    }
}
