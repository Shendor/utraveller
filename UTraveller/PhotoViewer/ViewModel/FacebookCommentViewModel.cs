using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;
using UTraveller.Service.Model;

namespace UTraveller.PhotoViewer.ViewModel
{
    public class FacebookCommentViewModel : BaseViewModel
    {
        private FacebookComment comment;

        public FacebookCommentViewModel(FacebookComment comment)
        {
            this.comment = comment;
        }


        public string Text
        {
            get { return comment.Message; }
        }


        public string From
        {
            get { return comment.From.Name; }
        }


        public string AvatarUrl
        {
            get { return comment.From.ImageUrl; }
        }
    }
}
