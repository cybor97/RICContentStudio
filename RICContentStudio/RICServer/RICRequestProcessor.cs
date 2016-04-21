using System;
using System.Linq;
using System.Collections.Generic;

namespace RICContentStudio
{
    public static class RICRequestProcessor
    {
        public static List<Article> Take(int count)
        {
            return new List<Article>(RICClient.RequestData(new RICRequest
            {
                Type = "GET",
                In = "Article",
                By = "Take",
                Data = count.ToString()
            }).Data.OfType<Article>());
        }

        public static Article GetArticleByID(int ID)
        {
            return (Article)RICClient.RequestData(new RICRequest
            {
                Type = "GET",
                In = "Article",
                By = "ID",
                Data = ID.ToString()
            }).Data.FirstOrDefault(c => c is Article);
        }
        public static List<Article> GetArticlesByTitle(string title)
        {
            return new List<Article>(RICClient.RequestData(new RICRequest
            {
                Type = "GET",
                In = "Article",
                By = "Title",
                Data = title
            }).Data.OfType<Article>());
        }
        public static List<Article> GetArticlesByText(string text)
        {
            return new List<Article>(RICClient.RequestData(new RICRequest
            {
                Type = "GET",
                In = "Article",
                By = "Text",
                Data = text
            }).Data.OfType<Article>());
        }
        public static List<Article> GetArticlesByTags(string tags)
        {
            return new List<Article>(RICClient.RequestData(new RICRequest
            {
                Type = "GET",
                In = "Article",
                By = "AnyTag",
                Data = tags.Replace(" ", "").Replace(",", "")
            }).Data.OfType<Article>());
        }
        public static string Remove(int ID)
        {
            return RICClient.RequestData(new RICRequest
            {
                Type = "REMOVE",
                In = "Article",
                By = "ID",
                Data = ID.ToString()
            }).Data.OfType<string>().FirstOrDefault();
        }
        public static string Set(int at, Article article)
        {
            article.ID = at;
            return RICClient.RequestData(new RICRequest
            {
                Type = "SET",
                In = "Article",
                By = "ID",
                Data = article.ToString()
            }).Data.OfType<string>().FirstOrDefault();
        }
        public static string Add(Article article)
        {
            var similarArticles = GetArticlesByTitle(article.Title);
            if (similarArticles.Any(c => c.Title == article.Title))
                return Set(similarArticles.First(c => c.Title == article.Title).ID, article);
            else
                return RICClient.RequestData(new RICRequest
                {
                    Type = "ADD",
                    In = "Article",
                    By = "ID",
                    Data = article.ToString()
                }).Data.OfType<string>().FirstOrDefault();
        }
    }
}

