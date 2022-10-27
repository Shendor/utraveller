using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Model;

namespace UTraveller.PhotoViewer.ViewModel
{
    public class FacebookPrivacyModel
    {
        public FacebookPrivacyModel(string name, FacebookPrivacyType privacyType)
        {
            Name = name;
            PrivacyType = privacyType;
        }


        public string Name
        {
            get;
            private set;
        }


        public FacebookPrivacyType PrivacyType
        {
            get;
            private set;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is FacebookPrivacyModel))
            {
                return false;
            }

            return ((FacebookPrivacyModel)obj).PrivacyType == this.PrivacyType;
        }


        public override int GetHashCode()
        {
            return PrivacyType.GetHashCode();
        }


        public override string ToString()
        {
            return Name;
        }
    }
}
