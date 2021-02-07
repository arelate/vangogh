package cmd

import (
	"encoding/json"
	"github.com/arelate/gogtypes"
	"github.com/arelate/gogurls"
	"github.com/boggydigital/kvas"
	"io/ioutil"
	"log"
	"net/http"
	"net/url"
	"strconv"
)

func paginated(productType string) bool {
	switch productType {
	case "products":
		fallthrough
	case "account-products":
		fallthrough
	case "wishlist":
		return true
	default:
		return false
	}
}

func requiresAuth(productType string) bool {
	switch productType {
	case "account-products":
		fallthrough
	case "wishlist":
		fallthrough
	case "details":
		return true
	default:
		return false
	}
}

func fetchPages(productType string, media string, sourceUrl func(int, gogtypes.Media) *url.URL, destUrl string) error {
	totalPages := 1
	for pp := 1; pp <= totalPages; pp++ {

		log.Printf("fetching %s %s page %d/%d\n", productType, media, pp, totalPages)

		resp, err := http.Get(sourceUrl(pp, gogtypes.Parse(media)).String())
		if err != nil {
			return err
		}

		bytes, err := ioutil.ReadAll(resp.Body)
		if err != nil {
			return err
		}

		if err = resp.Body.Close(); err != nil {
			return err
		}

		vs, err := kvas.NewClient(destUrl, ".json", false)
		if err != nil {
			return err
		}

		if err = vs.Set(strconv.Itoa(pp), bytes); err != nil {
			return err
		}

		var page gogtypes.Page
		if err = json.Unmarshal(bytes, &page); err != nil {
			return err
		}

		totalPages = page.TotalPages
	}

	return nil
}

func Fetch(productType string, media string) error {

	destUrl := "data"
	var sourceUrl func(int, gogtypes.Media) *url.URL

	switch productType {
	case "products":
		destUrl += "/productPages/"
		sourceUrl = gogurls.DefaultProductsPage
	case "account-products":
		destUrl += "/accountProductPages/"
		sourceUrl = gogurls.DefaultAccountProductsPage
	case "wishlist":
		destUrl += "/wishlistPages/"
		sourceUrl = gogurls.DefaultWishlistPage
	case "details":
		destUrl += "/details/"
		sourceUrl = gogurls.Details
	default:
		log.Fatalf("unknown product type %s\n", productType)
	}

	destUrl += media

	if paginated(productType) {
		return fetchPages(productType, media, sourceUrl, destUrl)
	} else {
		log.Fatalf("fetching %s is not supported yet\n", productType)
	}

	return nil
}
