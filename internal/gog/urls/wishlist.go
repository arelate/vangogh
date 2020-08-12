package urls

import (
	"net/url"
	"strconv"
)

const dateAdded = "date_added"

func WishlistPageURL(mediaType MediaType, hidden bool) *url.URL {
	wishlistPage := &url.URL{
		Scheme: HttpsScheme,
		Host:   GogHost,
		Path:   WishlistPath,
	}
	hiddenFlag := "0"
	if hidden {
		hiddenFlag = "1"
	}
	q := wishlistPage.Query()
	q.Add("mediaType", strconv.Itoa(int(mediaType)))
	q.Add("sortBy", dateAdded)
	q.Add("hiddenFlag", hiddenFlag)
	wishlistPage.RawQuery = q.Encode()

	return wishlistPage

}
