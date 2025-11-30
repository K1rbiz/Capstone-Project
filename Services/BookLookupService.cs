using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace Capstone_Project_v0._1.Services
{
    public sealed class BookLookupResult
    {
        public string? Title { get; set; }
        public string? AuthorName { get; set; }
        public string? Isbn { get; set; }
        public int? PageCount { get; set; }
    }

    public sealed class BookLookupService
    {
        private readonly HttpClient _http;

        public BookLookupService(HttpClient http)
        {
            _http = http;
        }

        public async Task<BookLookupResult?> LookupByIsbnAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return null;

            isbn = isbn.Trim();

            // Open Library ISBN endpoint
            var url = $"https://openlibrary.org/isbn/{isbn}.json";

            try
            {
                // Main book data fetch
                using var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                // Parse JSON response
                using var stream = await response.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                var root = doc.RootElement;

                // --- Title ---
                string? title = null;
                if (root.TryGetProperty("title", out var titleProp))
                {
                    title = titleProp.GetString();
                }

                // --- Page count ---
                int? pages = null;
                if (root.TryGetProperty("number_of_pages", out var pagesProp) &&
                    pagesProp.ValueKind == JsonValueKind.Number &&
                    pagesProp.TryGetInt32(out var pageCount))
                {
                    pages = pageCount;
                }


                // --- Author name (requires second API call) ---
                string? authorName = null;

                // "authors": [ { "key": "/authors/OL12345A" }, ... ]
                if (root.TryGetProperty("authors", out var authorsProp) &&
                    authorsProp.ValueKind == JsonValueKind.Array &&
                    authorsProp.GetArrayLength() > 0)
                {
                    var firstAuthor = authorsProp.EnumerateArray().FirstOrDefault();
                    if (firstAuthor.ValueKind == JsonValueKind.Object &&
                        firstAuthor.TryGetProperty("key", out var keyProp))
                    {
                        var authorKey = keyProp.GetString(); // e.g. "/authors/OL12345A"
                        if (!string.IsNullOrWhiteSpace(authorKey))
                        {
                            authorName = await TryGetAuthorNameAsync(authorKey);
                        }
                    }
                }

                return new BookLookupResult
                {
                    Title = title,
                    AuthorName = authorName,
                    Isbn = isbn,
                    PageCount = pages
                };
            }
            catch
            {
                // Network errors or JSON parsing issues → just treat as "not found"
                return null;
            }
        }

        /// <summary>
        /// Given an author key like "/authors/OL12345A",
        /// call https://openlibrary.org/authors/OL12345A.json and return the "name".
        /// </summary>
        private async Task<string?> TryGetAuthorNameAsync(string authorKey)
        {
            if (!authorKey.StartsWith("/"))
            {
                authorKey = "/" + authorKey;
            }

            var url = $"https://openlibrary.org{authorKey}.json";

            try
            {
                using var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                using var stream = await response.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                var root = doc.RootElement;

                if (root.TryGetProperty("name", out var nameProp))
                {
                    return nameProp.GetString();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

    }
}
