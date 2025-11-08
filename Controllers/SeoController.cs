using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Xml.Linq;
using WebBanHang.Data;

namespace WebBanHang.Controllers
{
    public class SeoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("sitemap.xml")]
        public async Task<IActionResult> Sitemap()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("urlset",
                    new XAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9"),
                    new XAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                    new XAttribute("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"),

                    // Trang chủ
                    new XElement("url",
                        new XElement("loc", baseUrl),
                        new XElement("lastmod", DateTime.Now.ToString("yyyy-MM-dd")),
                        new XElement("changefreq", "daily"),
                        new XElement("priority", "1.0")
                    ),

                    // Danh sách sản phẩm
                    new XElement("url",
                        new XElement("loc", $"{baseUrl}/Home/Index"),
                        new XElement("lastmod", DateTime.Now.ToString("yyyy-MM-dd")),
                        new XElement("changefreq", "daily"),
                        new XElement("priority", "0.9")
                    ),

                    // Các danh mục
                    (await _context.Categories.ToListAsync()).Select(category =>
                        new XElement("url",
                            new XElement("loc", $"{baseUrl}/Home/ProductsByCategory?categoryId={category.Id}"),
                            new XElement("lastmod", DateTime.Now.ToString("yyyy-MM-dd")),
                            new XElement("changefreq", "weekly"),
                            new XElement("priority", "0.8")
                        )
                    ),

                    // Các sản phẩm
                    (await _context.Products.ToListAsync()).Select(product =>
                        new XElement("url",
                            new XElement("loc", $"{baseUrl}/Home/Details/{product.Id}"),
                            new XElement("lastmod", DateTime.Now.ToString("yyyy-MM-dd")),
                            new XElement("changefreq", "weekly"),
                            new XElement("priority", "0.7")
                        )
                    )
                )
            );

            return Content(sitemap.ToString(), "application/xml", System.Text.Encoding.UTF8);
        }

        [HttpGet]
        [Route("robots.txt")]
        public IActionResult Robots()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var robotsContent = $@"User-agent: *
Allow: /

# Sitemap location
Sitemap: {baseUrl}/sitemap.xml

# Block admin pages
Disallow: /Admin/
Disallow: /Account/

# Allow all other pages
Allow: /Home/
Allow: /Cart/
Allow: /Order/";

            return Content(robotsContent, "text/plain", System.Text.Encoding.UTF8);
        }
    }
}

