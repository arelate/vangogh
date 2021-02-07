package cmd

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/arelate/gogauth"
	"github.com/arelate/gogtypes"
	"github.com/arelate/gogurls"
	"github.com/boggydigital/vangogh/internal"
	"time"

	//"github.com/arelate/gogauth"
	"github.com/boggydigital/kvas"
	"io"
	"log"
	"net/http"
	"net/url"
	"strconv"
)

type getUrl func(string, gogtypes.Media) *url.URL

var httpClient *http.Client

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

func requiresAuthentication(productType string) bool {
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

func sourceUrl(productType string) (getUrl, error) {
	switch productType {
	case "products":
		return gogurls.DefaultProductsPage, nil
	case "account-products":
		return gogurls.DefaultAccountProductsPage, nil
	case "wishlist":
		return gogurls.DefaultWishlistPage, nil
	case "details":
		return gogurls.Details, nil
	default:
		return nil, fmt.Errorf("unknown product type %s\n", productType)
	}
}

func destinationUrl(productType, media string) (string, error) {
	destUrl := "data"

	switch productType {
	case "products":
		destUrl += "/productPages/"
	case "account-products":
		destUrl += "/accountProductPages/"
	case "wishlist":
		destUrl += "/wishlistPages/"
	case "details":
		destUrl += "/details/"
	default:
		return "", fmt.Errorf("unknown product type %s\n", productType)
	}

	destUrl += media
	return destUrl, nil
}

func fetchItem(id string, media gogtypes.Media, sourceUrl getUrl, destUrl string) (io.Reader, error) {
	u := sourceUrl(id, media)
	resp, err := httpClient.Get(u.String())
	if err != nil {
		return nil, err
	}

	if resp.StatusCode < 200 || resp.StatusCode > 299 {
		return nil, fmt.Errorf("error fetching read closer at %s: %s", u.String(), resp.Status)
	}

	vs, err := kvas.NewClient(destUrl, ".json")
	if err != nil {
		return resp.Body, err
	}

	var b bytes.Buffer
	tr := io.TeeReader(resp.Body, &b)

	if err = vs.Set(id, tr); err != nil {
		return &b, err
	}

	if err := resp.Body.Close(); err != nil {
		return &b, err
	}

	return &b, nil
}

func fetchPages(productType string, media gogtypes.Media, sourceUrl getUrl, destUrl string) error {
	totalPages := 1
	for pp := 1; pp <= totalPages; pp++ {

		log.Printf("fetching %s %s page %d/%d\n", productType, media, pp, totalPages)

		rdr, err := fetchItem(strconv.Itoa(pp), media, sourceUrl, destUrl)
		if err != nil {
			return err
		}

		var page gogtypes.Page
		if err = json.NewDecoder(rdr).Decode(&page); err != nil {
			return err
		}

		totalPages = page.TotalPages
	}

	return nil
}

func fetchItems(
	ids []string,
	productType string,
	mt gogtypes.Media,
	sourceUrl getUrl,
	destUrl string) error {

	switch productType {
	case "details":
		break
	default:
		return fmt.Errorf("fetching items of type %s is not supported", productType)
	}

	for _, id := range ids {
		log.Printf("fetching %s %s id %s\n", productType, mt.String(), id)

		_, err := fetchItem(id, mt, sourceUrl, destUrl)
		if err != nil {
			return err
		}
	}
	return nil
}

func Fetch(ids []string, productType, media string) error {

	jar, err := internal.LoadCookieJar()
	if err != nil {
		return err
	}

	httpClient = &http.Client{
		Timeout: time.Minute * 3,
		Jar:     jar,
	}

	if requiresAuthentication(productType) {
		li, err := gogauth.LoggedIn(httpClient)
		if err != nil {
			return err
		}

		if !li {
			log.Fatalf("fetching type %s requires authenticated session. See \"help auth\" for details\n", productType)
		}
	}

	destUrl, err := destinationUrl(productType, media)
	if err != nil {
		return err
	}
	srcUrl, err := sourceUrl(productType)
	if err != nil {
		return err
	}

	mt := gogtypes.Parse(media)

	if paginated(productType) {
		return fetchPages(productType, mt, srcUrl, destUrl)
	} else {
		return fetchItems(ids, productType, mt, srcUrl, destUrl)
	}
}
