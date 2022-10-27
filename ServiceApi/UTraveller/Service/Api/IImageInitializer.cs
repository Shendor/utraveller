using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface IImageInitializer
    {
        Task<bool> InitializeImage(Photo photoItem);

        void DisposeImage(Photo photoItem);
    }
}
