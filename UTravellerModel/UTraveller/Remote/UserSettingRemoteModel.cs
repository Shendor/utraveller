using System;

namespace UTravellerModel.UTraveller.Model.Remote
{
    public class UserSettingRemoteModel
    {
        public long Id
        {
            get;
            set;
        }


        public string MainColor
        {
            get;
            set;
        }


        public string BackgroundColor
        {
            get;
            set;
        }


        public string TextColor
        {
            get;
            set;
        }


        public string LimitCode
        {
            get;
            set;
        }


        public float CoverOpacity
        {
            get;
            set;
        }


        public bool IsLandscapeCover
        {
            get;
            set;
        }
    }
}
