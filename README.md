# Arthur

Make PDF files from URLs and base64-encoded HTML.

## Endpoints

There are two separate endpoints in order to generate a PDF depending on whether you want to pass a URL or a base64-encoded HTML string.

Each endpoint accepts a POST request with a JSON body.

`/from-url` requires a `url` field in the JSON body. It must include the scheme (i.e. protocol) in order to be considered valid (e.g. `https://google.com` instead of `google.com`)

`/from-base64` requires a `encodedTemplate` field in the JSON body. It must be a base64-encoded string.

## Request Options

The following options are common among both endpoints and can be defined as fields in the JSON body. You can read more about the underlying options [here](https://playwright.dev/dotnet/docs/api/class-page#page-pdf)

#### Width / Height / Margin Measurements

All measurement fields must be an int or float optionally followed by a unit of measurement.

Valid units are `px|in|cm|mm`. If no unit is provided, then defaults to `px`. Examples include `1024`(pixels), `12.5in`, `10cm`, and `400mm`.

#### Header / Footer Options

* `displayHeaderFooter`: Expects `boolean`. Display header and footer. Defaults to `false`.
* `headerTemplate`: Expects `string`. HTML template for the print header. Must be base64-encoded. Check underlying [Playwright documentation](https://playwright.dev/dotnet/docs/api/class-page#page-pdf-option-header-template) for more.
* `footerTemplate`: Expects `string`. HTML template for the print footer. Must be base64-encoded. Check underlying [Playwright documentation](https://playwright.dev/dotnet/docs/api/class-page#page-pdf-option-footer-template) for more.

#### Margin Options
All margin options default to `0px`.

* `margin`: Expects `string`. Margin to use for all sides of each page. This field has lower precedence than other margin fields.
* `marginTop`: Expects `string`. Margin to use for the top of each page.
* `marginLeft`: Expects `string`. Margin to use for the left side of each page.
* `marginRight`: Expects `string`. Margin to use for the right side of each page.
* `marginBottom`: Expects `string`. Margin to use for the bottom of each page.

#### Page Size / Layout Options

* `landscape`: Expects `boolean`. Paper orientation. Defaults to `false`.
* `format`: Expects `string`. Takes priority over width or height. Options are `Letter`, `Legal`, `Tabloid`, `Ledger`, and `A0` through `A6`.  Defaults to `Letter`.
* `width`: Expects `string`. Width of the printed page.
* `height`: Expects `string`. Height of the printed page.
* `singlePage`: Expects `boolean`. Prints to a single page. Takes precedence over `height` field.
* `scale`: Expects `float`. Scale of the page rendering. Must be between `0.1` and `2.0`.
* `preferCssPageSize`: Expects `boolean`. Give any CSS @page size declared in the page priority over what is declared in `width` and `height` or `format` options. Defaults to `false`.

#### Miscellaneous Options

* `outline`: Expects `boolean`. Embed the document outline into the PDF. Defaults to `false`.
* `pageRanges`: Expects `string`. Paper ranges to print, e.g., `1-5, 8, 11-13`. Defaults to all pages.
* `printBackground`: Expects `boolean`. Print background graphics. Defaults to `false`.
* `tagged`: Expects `boolean`. Generate tagged (accessible) PDF. Defaults to `false`.
* `pageRenderDelay`: Expects `int`. Wait for a period of time in milliseconds before saving the PDF. Defaults to `0`.

#### Response Options

* `responseFormat`: Expects `string`. Determines which format to return the PDF in. Valid options are `PDF` and `BASE64`. Defaults to `PDF`.
* `fileName`: Expects `string`. When using the `PDF` `responseFormat` type, determines the name of the returned file.

## Deploying

I suggest that you use docker compose to deploy this.

```dockerfile
services:
  arthur:
    image: 'craysiii/arthur:latest'
    restart: unless-stopped
    ports:
      - 9650:8080
```

## Security Considerations

Currently the docker container is configured to run as `root`. *This is bad.* I need to figure out how to bypass it, but I would not expose this to the public internet.

Our instance sits behind a Cloudflare Tunnel with Access tied to a Service Auth Token.

## License

MIT.