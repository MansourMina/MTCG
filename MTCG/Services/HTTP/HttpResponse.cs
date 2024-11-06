namespace MTCG.Services.HTTP
{
    public class HttpResponse(StreamWriter writer)
    {
        public string Status { get; set; } = "200 OK";
        public string ContentType { get; set; } = "text/html";
        public string Body { get; set; } = "<html><body>Hello World! This is MIna.</body></html>";

        public StreamWriter Writer { get; set; } = writer;

        public void Send()
        {
            // Header
            Writer.WriteLine($"HTTP/1.0 {Status}");
            Writer.WriteLine($"Content-Type: {ContentType}");
            Writer.WriteLine($"Content-Length: {Body.Length}");
            Writer.WriteLine(); // empty line indicates end of headers

            // Body
            Writer.WriteLine(Body);
        }
    }
}
