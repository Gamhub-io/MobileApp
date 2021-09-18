using System;
using System.Collections.Generic;
using System.Text;

namespace AresNews.Models
{
    public class BackupArticles : Article
    {

        public DateTime DateBackup { get; private set; } = DateTime.Now;

    }
}
