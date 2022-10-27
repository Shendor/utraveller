using System;
using System.Windows.Media.Imaging;
using UTraveller.Common.Converter;

namespace UTravellerModel.UTraveller.Model
{
    public class User : BaseModel
    {
        public User()
        {
            ChangeDate = DateTime.Now;
            Name = "Unknown";
        }

        public string Name
        {
            get;
            set;
        }


        public string Email
        {
            get;
            set;
        }


        public string Description
        {
            get;
            set;
        }


        public byte[] AvatarContent
        {
            get;
            set;
        }


        public byte[] CoverContent
        {
            get;
            set;
        }


        public string RESTAccessToken
        {
            get;
            set;
        }
    }
}
