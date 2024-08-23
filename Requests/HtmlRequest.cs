namespace Arthur.Requests;

public class HtmlRequest : PdfDocumentRequest
{
    [Required]
    public required string EncodedTemplate { get; set; }
}