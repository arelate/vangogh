package urls

import (
	"net/url"
	"strconv"
)

const sortDatePurchased = "date_purchased"

func AccountProductsPageURL(mediaType MediaType, updated bool, hidden bool) *url.URL {

	accountProductsPage := &url.URL{
		Scheme: HttpsScheme,
		Host:   GogHost,
		Path:   AccountProductPagesPath,
	}
	hiddenFlag, updatedFlag := "0", "0"
	if hidden {
		hiddenFlag = "1"
	}
	if updated {
		updatedFlag = "1"
	}
	q := accountProductsPage.Query()
	q.Add("mediaType", strconv.Itoa(int(mediaType)))
	q.Add("sortBy", sortDatePurchased)
	//q.Add("page", strconv.Itoa(page))
	q.Add("hiddenFlag", hiddenFlag)
	q.Add("isUpdated", updatedFlag)
	accountProductsPage.RawQuery = q.Encode()

	return accountProductsPage
}
