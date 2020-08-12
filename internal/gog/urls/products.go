package urls

import (
	"net/url"
)

const newestFirst = "release_desc"

func ProductsPageURL(mediaType MediaType) *url.URL {
	productsPage := &url.URL{
		Scheme: HttpsScheme,
		Host:   GogHost,
		Path:   ProductPagesPath,
	}
	q := productsPage.Query()
	q.Add("mediaType", mediaType.String())
	q.Add("sort", newestFirst)
	//q.Add("page", strconv.Itoa(page))
	productsPage.RawQuery = q.Encode()

	return productsPage
}
