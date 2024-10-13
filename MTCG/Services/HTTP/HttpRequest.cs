using System.Text;

namespace MTCG.Services.HTTP
{
    public class HttpRequest
    {
        public string Method { get; private set; }
        public string Path { get; private set; }
        public string Version { get; private set; }
        public int Content_Length { get; private set; } = 0;
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public StringBuilder Body { get; private set; } = new StringBuilder();
        public HttpRequest(StreamReader reader)
        {
            readHttp(reader);
            readHeader(reader);
            readBody(reader);
        }

        private void readHttp(StreamReader reader)
        {
            string? line;
            line = reader.ReadLine();
            var httpParts = line.Split(' ');
            Method = httpParts[0];
            Path = httpParts[1];
            Version = httpParts[2];
        }

        private void readHeader(StreamReader reader)
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length == 0)
                    break;  // emtpy line indicates the end of the HTTP-headers

                // Parse the header
                var headerParts = line.Split(':');
                var headerName = headerParts[0];
                var headerValue = headerParts[1].Trim();
                Headers[headerName] = headerValue;
                if (headerName == "Content-Length")
                {
                    Content_Length = int.Parse(headerValue);
                }
            }
        }

        private void readBody(StreamReader reader)
        {
            if (Content_Length > 0)
            {
                char[] chars = new char[1024];
                int bytesReadTotal = 0;
                while (bytesReadTotal < Content_Length)
                {
                    var bytesRead = reader.Read(chars, 0, chars.Length);
                    bytesReadTotal += bytesRead;
                    if (bytesRead == 0)
                        break;  // no more data available
                    Body.Append(chars, 0, bytesRead);
                }
            }
        }

    }
}
