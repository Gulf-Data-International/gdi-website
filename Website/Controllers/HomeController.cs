using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Canvas.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;

namespace Canvas.Controllers
{
    //[Route("[controller]")]
    public class HomeController : Controller
    {
        //[Route("/")]
        //public IActionResult Index()
        //{
        //    return View();
        //}
        private IOptions<RequestLocalizationOptions> _LocOptions;
        public HomeController(IOptions<RequestLocalizationOptions> LocOptions)
        {
            _LocOptions = LocOptions;
        }
        public IActionResult Index(string culture = "")
        {
            return View();
        }
        //[Route("Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        //[Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[Route("SetLanguage")]
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            _currentLanguage = culture.Substring(0, 2).ToLower();
            
            return LocalRedirect(GetNewPath(returnUrl, culture));
            //return LocalRedirect("~/" + new Uri(returnUrl).PathAndQuery.TrimStart('/'));
        }

        private string _currentLanguage;
        private string CurrentLanguage
        {
            get
            {
                if (!string.IsNullOrEmpty(_currentLanguage))
                    return _currentLanguage;

                if (string.IsNullOrEmpty(_currentLanguage))
                {
                    var feature = HttpContext.Features.Get<IRequestCultureFeature>();
                    _currentLanguage = feature.RequestCulture.Culture.TwoLetterISOLanguageName.ToLower();
                }

                return _currentLanguage;
            }
        }

        public ActionResult RedirectToDefaultLanguage()
        {
            var culture = CurrentLanguage;
            //if (culture != "en") culture = "en";
            //if (String.IsNullOrEmpty(culture))  culture = "en";
            //return RedirectToAction("Index", new { culture });
            return LocalRedirect(GetNewPath(GetAbsoluteUri().AbsoluteUri, culture));
        }


        private string GetNewPath(HttpContext context, string newCulture)
        {
            var routeData = context.GetRouteData();
            var router = routeData.Routers[0];
            var virtualPathContext = new VirtualPathContext(
                context,
                routeData.Values,
                new RouteValueDictionary { { "culture", newCulture } });

            return router.GetVirtualPath(virtualPathContext).VirtualPath;
        }

        private string GetNewPath(string uri, string newCulture)
        {
            string newUrl = uri;
            
            Uri path = new Uri(uri.TrimStart('~'));
            List<string> segments = new List<string>();
            foreach(var item in path.Segments){ if (item != "/") { segments.Add(item.TrimEnd('/')); }else{ segments.Add(item); } }
            // path.PathAndQuery
            int changeSegment = 0;
            if (segments[0] == "/") changeSegment = 1;
            if ( (segments.Count > 1) && segments[changeSegment].Length == 2 && _LocOptions.Value.SupportedCultures.Where(c => c.TwoLetterISOLanguageName == segments[changeSegment]).Count() > 0)
            {

                segments[changeSegment] = newCulture.Substring(0, 2);
                if (changeSegment == 1)
                {
                    newUrl = String.Join("/", segments).TrimStart('/');
                }
                else
                {
                    newUrl = String.Join("/", segments);
                }
            }

            if (segments.Count == 1 & segments[0] == "/") newUrl = CurrentLanguage;
            return "~/" + newUrl + "/";
        }

        private Uri GetAbsoluteUri()
        {
            var request = HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            uriBuilder.Path = request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();
            return uriBuilder.Uri;
        }
    }
}
