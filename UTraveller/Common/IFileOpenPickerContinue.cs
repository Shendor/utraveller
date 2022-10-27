using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace UTraveller.Common
{
    public interface IFileOpenPickerContinue
    {
        void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args);
    }

    public interface IFilePickerContinuationViewModelPage
    {
        IFileOpenPickerContinue ViewModel { get; }
    }
}
