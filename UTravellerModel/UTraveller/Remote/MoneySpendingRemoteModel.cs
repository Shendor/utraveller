using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Model.Remote
{
    public class MoneySpendingRemoteModel : BaseRemoteModel
    {       
        public string MoneySpendingCategory { get; set; }
        
        public float Amount { get; set; }
        
        public string Description { get; set; }

        public string Currency { get; set; }
        
        public DateTime Date { get; set; }
    }
}
