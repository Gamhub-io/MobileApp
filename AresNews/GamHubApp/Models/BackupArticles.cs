using System;
using System.Collections.Generic;
using System.Text;

namespace GamHub.Models
{
    public class BackupArticles : Article
    {

        public DateTime DateBackup { get; private set; } = DateTime.Now;

    }
}
