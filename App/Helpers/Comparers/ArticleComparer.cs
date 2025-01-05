using GamHubApp.Models;
using System;
using System.Collections.Generic;

namespace GamHubApp.Helpers.Comparers
{
    public class ArticleComparer : IEqualityComparer<Article>
    {
        public bool Equals(Article x, Article y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            // Replace "PropertyName" with the name of the property you want to compare
            return x.Id == y.Id;
        }

        public int GetHashCode(Article article)
        {
            if (Object.ReferenceEquals(article, null)) return 0;

            // Replace "PropertyName" with the name of the property you want to compare
            int hashProductName = article.Id == null ? 0 : article.Id.GetHashCode();

            return hashProductName;
        }
    }
}
