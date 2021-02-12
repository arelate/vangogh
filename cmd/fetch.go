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

const (
	Store            = "store"
	Account          = "account"
	Wishlist         = "wishlist"
	Details          = "details"
	Products         = "products"
	AccountProducts  = "account-products"
	WishlistProducts = "wishlist-products"
)

const jsonExt = ".json"

type getUrl func(string, gogtypes.Media) *url.URL

var httpClient *http.Client

func paginated(pt string) bool {
	switch pt {
	case Store:
		fallthrough
	case Account:
		fallthrough
	case Wishlist:
		return true
	default:
		return false
	}
}

func singular(pt string) string {
	if paginated(pt) {
		return "page"
	} else {
		return ""
	}
}

func requiresAuthentication(productType string) bool {
	switch productType {
	case Account:
		fallthrough
	case Wishlist:
		fallthrough
	case Details:
		return true
	default:
		return false
	}
}

func sourceUrl(pt string) (getUrl, error) {
	switch pt {
	case Store:
		return gogurls.DefaultProductsPage, nil
	case Account:
		return gogurls.DefaultAccountProductsPage, nil
	case Wishlist:
		return gogurls.DefaultWishlistPage, nil
	case Details:
		return gogurls.Details, nil
	default:
		return nil, fmt.Errorf("cannot provide a source url for a type %s\n", pt)
	}
}

func destinationUrl(pt, media string) (string, error) {
	return fmt.Sprintf("data/%s/%s", pt, media), nil
}

func mainProductType(productType string) string {
	switch productType {
	case Details:
		return AccountProducts
	default:
		return ""
	}
}

func detailProductType(pt string) string {
	switch pt {
	case Store:
		return Products
	case Account:
		return AccountProducts
	case Wishlist:
		return WishlistProducts
	default:
		return ""
	}
}

func paginatedProductType(pt string) string {
	switch pt {
	case Products:
		return Store
	case AccountProducts:
		return Account
	case WishlistProducts:
		return Wishlist
	default:
		return pt
	}
}

func fetchItem(id string, pt string, media gogtypes.Media, sourceUrl getUrl, destUrl string) (io.Reader, error) {

	log.Printf("fetching %s (%s) %s %-s\n", pt, media, singular(pt), id)

	u := sourceUrl(id, media)
	resp, err := httpClient.Get(u.String())
	if err != nil {
		return nil, err
	}

	if resp.StatusCode < 200 || resp.StatusCode > 299 {
		return nil, fmt.Errorf("error fetching read closer at %s: %s", u.String(), resp.Status)
	}

	vs, err := kvas.NewLocal(destUrl, jsonExt, jsonExt)
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

		rdr, err := fetchItem(strconv.Itoa(pp), productType, media, sourceUrl, destUrl)
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

func fetchMissing(
	productType string,
	mt gogtypes.Media,
	sourceUrl getUrl,
	mainDestUrl, detailDestUrl string) error {
	kvMain, err := kvas.NewLocal(mainDestUrl, jsonExt, jsonExt)
	if err != nil {
		return err
	}

	kvDetail, err := kvas.NewLocal(detailDestUrl, jsonExt, jsonExt)
	if err != nil {
		return err
	}
	missingIds := make([]string, 0)
	for _, id := range kvMain.All() {
		if !kvDetail.Contains(id) {
			missingIds = append(missingIds, id)
		}
	}

	if len(missingIds) > 0 {
		if err := fetchItems(missingIds, productType, mt, sourceUrl, detailDestUrl); err != nil {
			return err
		}
	} else {
		log.Printf("no missing %s (%s)\n", productType, mt)
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
		_, err := fetchItem(id, productType, mt, sourceUrl, destUrl)
		if err != nil {
			return err
		}
	}
	return nil
}

func Fetch(ids []string, pt, media string, missing bool) error {

	jar, err := internal.LoadCookieJar()
	if err != nil {
		return err
	}

	httpClient = &http.Client{
		Timeout: time.Minute * 3,
		Jar:     jar,
	}

	if requiresAuthentication(pt) {
		li, err := gogauth.LoggedIn(httpClient)
		if err != nil {
			return err
		}

		if !li {
			log.Fatalf("fetching type %s requires authenticated session", pt)
		}
	}

	// clo.json specifies "normal" type, in order to get it we need to:
	// - fetch individual pages of paginated type = product-type + "-pages"
	// - split fetched pages into individual blocks
	pt = paginatedProductType(pt)

	dstUrl, err := destinationUrl(pt, media)
	if err != nil {
		return err
	}

	srcUrl, err := sourceUrl(pt)
	if err != nil {
		return err
	}

	mt := gogtypes.Parse(media)

	if paginated(pt) {
		if err := fetchPages(pt, mt, srcUrl, dstUrl); err != nil {
			return err
		}

		return split(pt, media)
	} else {
		if missing {
			mainDstUrl, err := destinationUrl(mainProductType(pt), media)
			if err != nil {
				return err
			}
			return fetchMissing(pt, mt, srcUrl, mainDstUrl, dstUrl)
		} else {
			return fetchItems(ids, pt, mt, srcUrl, dstUrl)
		}
	}
}
