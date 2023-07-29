﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AresNews.Models
{
    public class UpdateOrder
    {
        public FeedUpdate Update { get; set; }
        public Feed Feed { get; set; }
        public enum FeedUpdate
        {
            Remove,
            Add,
            Edit
        }
    }
}
