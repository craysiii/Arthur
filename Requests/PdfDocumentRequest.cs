namespace Arthur.Requests;

public class PdfDocumentRequest
{
    public bool? DisplayHeaderFooter { get; set; }
    public string? FooterTemplate { get; set; }
    public string? HeaderTemplate { get; set; }
    public bool? SinglePage { get; set; }
    public bool? Landscape { get; set; }
    [RegularExpression(@"^(Letter|Legal|Tabloid|Ledger|A[0-6]{1})$",
        ErrorMessage = "Format must be one of the following values: Letter, Legal, Tabloid, Ledger, A<0-6>")]
    public string? Format { get; set; }
    [RegularExpression(@"^[+-]?(\d*[.])?\d+(px|in|cm|mm)?$",
        ErrorMessage = "Height must be in the format <int or float><px|in|cm|mm> e.g. '8.5in' or '250cm'")]
    public string? Height { get; set; }
    [RegularExpression(@"^[+-]?(\d*[.])?\d+(px|in|cm|mm)?$",
        ErrorMessage = "Width must be in the format <int or float><px|in|cm|mm> e.g. '8.5in' or '250cm'")]
    public string? Width { get; set; }
    [RegularExpression(@"^[+-]?(\d*[.])?\d+(px|in|cm|mm)?$",
        ErrorMessage = "Margin must be in the format <int or float><px|in|cm|mm> e.g. '8.5in' or '250cm'")]
    public string? Margin { get; set; }
    [RegularExpression(@"^[+-]?(\d*[.])?\d+(px|in|cm|mm)?$",
        ErrorMessage = "MarginTop must be in the format <int or float><px|in|cm|mm> e.g. '8.5in' or '250cm'")]
    public string? MarginTop { get; set; }
    [RegularExpression(@"^[+-]?(\d*[.])?\d+(px|in|cm|mm)?$",
        ErrorMessage = "MarginBottom must be in the format <int or float><px|in|cm|mm> e.g. '8.5in' or '250cm'")]
    public string? MarginBottom { get; set; }
    [RegularExpression(@"^[+-]?(\d*[.])?\d+(px|in|cm|mm)?$",
        ErrorMessage = "MarginLeft must be in the format <int or float><px|in|cm|mm> e.g. '8.5in' or '250cm'")]
    public string? MarginLeft { get; set; }
    [RegularExpression(@"^[+-]?(\d*[.])?\d+(px|in|cm|mm)?$",
        ErrorMessage = "MarginRight must be in the format <int or float><px|in|cm|mm> e.g. '8.5in' or '250cm'")]
    public string? MarginRight { get; set; }
    public bool? Outline { get; set; }
    [RegularExpression(@"^((\d+(\-\d+)?)(\,( )?)?)+$",
        ErrorMessage = "PageRanges must be in the format like so: '1-5, 8, 11-13'")]
    public string? PageRanges { get; set; }
    public bool? PreferCssPageSize { get; set; }
    public bool? PrintBackground { get; set; }
    [Range(0.1, 2, ErrorMessage = "Scale must be between 0.1 and 2.0")]
    public float? Scale { get; set; }
    public bool? Tagged { get; set; }
    [RegularExpression(@"^(PDF|BASE64)$", ErrorMessage = "ReturnFormat must be either 'PDF' or 'BASE64'")]
    public ResponseFormat? ResponseFormat { get; set; } = Enums.ResponseFormat.PDF;
    [RegularExpression(@"^[\w\-. ]+\.pdf")]
    public string? FileName { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "PageRenderDelay must be greater than 0")]
    public int? PageRenderDelay { get; set; }
}