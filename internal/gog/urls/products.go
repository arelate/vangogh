package urls

import (
	"net/url"
	"strconv"
)

const sortNew = "new"

func ProductsPageURL(mediaType string, page int) *url.URL {
	productsPage := &url.URL{
		Scheme: HttpsScheme,
		Host:   GogHost,
		Path:   ProductPagesPath,
	}
	q := productsPage.Query()
	q.Add("mediaType", mediaType)
	q.Add("sort", sortNew)
	q.Add("page", strconv.Itoa(page))
	productsPage.RawQuery = q.Encode()

	return productsPage
}
