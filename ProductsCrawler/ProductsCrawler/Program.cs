namespace ProductsCrawler
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Linq;

    class Program
    {
        static string domain = "https://www.quickmobile.ro";
        static string state;
        static void Main(string[] args)
        {
            Console.WriteLine("(1) Crawl a web page and extract one of the following:\n" + 
                            "\t-Product - price information(*)\n" +
                            "\t-Phone numbers\n" + 
                            "\t-Email adresses\n" + 
                            "\t-Images\n" + 
                            "(2) Crawl a web page and extract all information of the chosen category.\n" + 
                            "(3) Crawl all the web pages from a root website.\n" + 
                            "(4) Display the top N category based on a criteria to the console.");

            var root = "https://www.quickmobile.ro/laptopuri/laptopuri";
            var rootLinks = ExtractLinks(root);
            /*
            for(var i = 0; i < 10; i++)
            {
                DisplayProducts(rootLinks.ElementAt(i));
            }
            */
            Console.Write("\nCollecting top products in progress ");
            for (var i = 0; i < 10; i++)
            {
                CollectProducts(rootLinks.ElementAt(i));
                Console.Write("---");
            }
            Console.WriteLine(" Finish!\n");

            AllProducts = AllProducts.Distinct().ToList();
            var d = AllProducts.OrderByDescending(p => p.Price).ThenBy(p => p.Name).ToList();
            for (int i = 0; i < 10; i++)
                DisplayProduct(d.ElementAt(i));

            Console.WriteLine("\nSuccess!");
            Console.ReadLine();
        }

        static List<string> ExtractLinks(string root)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(root);

            var SelectedLinks = new List<string>();
            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
            {
                string hrefValue = link.GetAttributeValue("href", string.Empty);
                if (Regex.IsMatch(hrefValue, @"^[/]?([a-z]+[-]?[a-z]+)+[/]?([a-z]+[-]?[a-z]+)+$") &&
                    !hrefValue.Contains("info") &&
                    !hrefValue.Contains("suport") &&
                    !hrefValue.Contains("contact") &&
                    !hrefValue.Contains("comenzi") &&
                    !hrefValue.Contains("wishlist") &&
                    !hrefValue.Contains("servicii") &&
                    !hrefValue.Contains("despre-noi") &&
                    !hrefValue.Contains("magazinele-noastre") &&
                    !hrefValue.Contains("termeni-si-conditii"))
                {
                    SelectedLinks.Add(hrefValue);
                }
            }
            SelectedLinks.Sort();

            // Remove the duplicates from list
            var UniqueLinks = SelectedLinks.Distinct().ToList();
            
            // Extract the most specific URLs
            var IgnoredLinks = new List<String>();
            for (int i = 0; i < UniqueLinks.Count; i++)
            {
                for (int j = i + 1; j < UniqueLinks.Count-1; j++)
                {
                    if(UniqueLinks[j].Contains(UniqueLinks[i]) &&
                        !IgnoredLinks.Contains(UniqueLinks[i]))
                    {
                        IgnoredLinks.Add(UniqueLinks[i]);
                    }
                }
            }

            var list = new List<string>();
            foreach (var i in UniqueLinks)
            {
                if (!IgnoredLinks.Contains(i))
                {
                    string link;
                    if (i[0] == '/')
                    {
                        link = domain + i;
                    }
                    else
                    {
                        link = domain + '/' + i;
                    }
                    list.Add(link);
                }
            }
            return list;
        }
        public static List<Product> AllProducts = new List<Product>();
        public static void CollectProducts(string DocumentLink)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(DocumentLink);

            var products = new List<Product>();
            if (document.GetAllNodesWithClass("card card-product  \n     \n        margin-bottom-card-product") != null)
            {
                foreach (var node in document.GetAllNodesWithClass("card card-product  \n     \n        margin-bottom-card-product"))
                {
                    string brand = string.Empty, description = string.Empty;
                    int price = 0;

                    foreach (var descendant in node.Descendants())
                    {
                        if (descendant.HasClass("card-product-title"))
                        {
                            brand = descendant.InnerText.TrimWhiteSpace();
                        }
                        else if (descendant.HasClass("card-product-description"))
                        {
                            description = descendant.InnerText.TrimWhiteSpace();
                        }
                        else if (descendant.HasClass("card-price"))
                        {
                            string[] numbers = Regex.Split(descendant.InnerText.ToString(), @"\D+");
                            price = Int32.Parse(numbers[0]);
                        }
                    }

                    var product = new Product() { Name = brand + " - " + description, Price = price };
                    AllProducts.Add(product);
                }
            }
        }

        public static void DisplayProduct(Product product)
        {
            Console.Write(product.Name);
            int i = product.Name.Length;
            for (; i < 70; i++)
            {
                Console.Write(".");
            }
            Console.WriteLine(" " + product.Price + " LEI");
        }

        public static void DisplayProducts(string DocumentLink)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(DocumentLink);

            var products = new List<Product>();

            // get the most specific html tag, that contains both, the name of the product and its price
            // for each such tag, extract the name of the product and its price and create a Product object that will be added to the list of products
            if (document.GetAllNodesWithClass("card card-product  \n     \n        margin-bottom-card-product") == null)
            {
                PrintSource(DocumentLink);
                Console.WriteLine("No products were found here!");
            }
            else
            {
                foreach (var node in document.GetAllNodesWithClass("card card-product  \n     \n        margin-bottom-card-product"))
                {
                    // Console.WriteLine(node.InnerHtml);

                    string brand = string.Empty, description = string.Empty;
                    int price = 0;

                    foreach (var descendant in node.Descendants())
                    {
                        if (descendant.HasClass("card-product-title"))
                        {
                            brand = descendant.InnerText.TrimWhiteSpace();
                        }
                        else if (descendant.HasClass("card-product-description"))
                        {
                            description = descendant.InnerText.TrimWhiteSpace();
                        }
                        // TODO
                        else if (descendant.HasClass("card-price"))
                        {
                            string[] numbers = Regex.Split(descendant.InnerText.ToString(), @"\D+");
                            price = Int32.Parse(numbers[0]);
                        }
                    }

                    var product = new Product() { Name = brand + " - " + description, Price = price };
                    products.Add(product);
                }

                PrintSource(DocumentLink);
                PrintProducts(products);
            }

        }
        public static void PrintSource(string DocumentLink)
        {
            Console.WriteLine();
            for (int i = 0; i < DocumentLink.Length + 20; i++)
                Console.Write("-");
            Console.WriteLine(string.Format("\n  PRODUCTS FROM: {0}:  ", DocumentLink));
            for (int i = 0; i < DocumentLink.Length + 20; i++)
                Console.Write("-");
            Console.WriteLine();
        }
        public static void PrintProducts(List<Product> products)
        {
            //products.Sort(delegate (Product p1, Product p2) { return p1.Price.CompareTo(p2.Price); });
            foreach (var product in products)
            {
                    // Pretty printing
                    Console.Write(product.Name);
                    int i = product.Name.Length;
                    for (; i < 70; i++)
                    {
                        Console.Write(".");
                    }
                    Console.WriteLine(" " + product.Price + " LEI");
            }
        }
    }
}
