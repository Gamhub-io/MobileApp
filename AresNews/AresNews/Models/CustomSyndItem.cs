using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Text;

namespace AresNews.Models
{
    public class CustomSyndItem : SyndicationItem
    {
        public Source SourceItem { get; set; }

    }
}
