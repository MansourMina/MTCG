using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    public class HttpHandler
    {
        public HttpHandler(HttpRequest request, HttpResponse response) {
            Handle(request, response);
        }
        private void Handle(HttpRequest request, HttpResponse response) {
            if (request.Path == "/" && request.Method == "GET")
            {
                response.Status = "200 OK";
                response.Body = "<html><body>Here are the users...</body></html>";
            }
            else
            {
                response.Status = "404 Not Found";
                response.Body = "<html><body>Page not found</body></html>";
            }
            response.Send();
        }
    }
}
