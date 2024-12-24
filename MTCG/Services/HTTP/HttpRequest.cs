using System.Text;

namespace MTCG.Services.HTTP
{
    public class HttpRequest
    {
        public string? Method { get; private set; }
        public string? Path { get; private set; }
        public string? Version { get; private set; }
        public int Content_Length { get; private set; } = 0;
        public Dictionary<string, string> Headers { get; set; } = [];
        public StringBuilder Body { get; private set; } = new StringBuilder();
        public string? Authorization { get; private set; }
        public Dictionary<string, string>? PathVariables { get; set; } = new Dictionary<string, string>();
        public HttpRequest(StreamReader reader)
        {
            try
            {
                ReadHttp(reader);
                ReadHeader(reader);
                ReadBody(reader);
            }
            catch (InvalidDataException)
            {
                throw;
            }

        }

        private void ReadHttp(StreamReader reader)
        {
            string? line = reader.ReadLine() ?? throw new InvalidDataException("The HTTP request is incomplete or invalid");
            var httpParts = line.Split(' ');
            if (httpParts.Length < 3)
                throw new InvalidDataException("The HTTP request line is incomplete");

            Method = httpParts[0];
            Path = httpParts[1];
            Version = httpParts[2];
        }

        private void ReadHeader(StreamReader reader)
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
                    Content_Length = int.Parse(headerValue);
                if (headerName == "Authorization")
                    Authorization = headerValue;
            }
        }

        private void ReadBody(StreamReader reader)
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
