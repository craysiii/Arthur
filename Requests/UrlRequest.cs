namespace Arthur.Requests;

public class UrlRequest : PdfDocumentRequest
{
    [Required]
    public required string Url { get; set; }
}