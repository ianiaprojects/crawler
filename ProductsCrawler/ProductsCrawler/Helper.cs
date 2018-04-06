namespace ProductsCrawler
{
    using HtmlAgilityPack;

    public static class Helper
    {
        public static string TrimWhiteSpace(this string input)
        {
            return input.Trim(new char[] { ' ', '\t', '\n' });
        }

        public static HtmlNodeCollection GetAllNodesWithClass(this HtmlDocument document, string className)
        {
            return document.DocumentNode.SelectNodes("//*[contains(@class,'" + className + "')]");
        }
    }
}
