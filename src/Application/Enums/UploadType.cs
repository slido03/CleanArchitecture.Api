using System.ComponentModel;

namespace CleanArchitecture.Application.Enums
{
    public enum UploadType : byte
    {
        [Description(@"Images\ProfilePictures")]
        ProfilePicture,

        [Description(@"Documents")]
        Document,
    }
}