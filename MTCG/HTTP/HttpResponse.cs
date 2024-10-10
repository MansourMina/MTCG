using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    public class HttpResponse
    {
        public string Status { get; set; } = "200 OK";
        public string ContentType { get; set; } = "text/html";
        public string Body { get; set; } = "<html><body>Hello World! This is MIna.</body></html>";

        public StreamWriter Writer { get; set; }

        public HttpResponse(StreamWriter writer) {
            Writer= writer;
        }

        public void Send()
        {
            Writer.WriteLine($"HTTP/1.0 {Status}");
            Writer.WriteLine($"Content-Type: {ContentType}");
            Writer.WriteLine($"Content-Length: {Body.Length}");
            Writer.WriteLine(); // empty line indicates end of headers
            Writer.WriteLine(Body);
        }
    }
}
