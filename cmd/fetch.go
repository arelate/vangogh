package cmd

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_auth"
	"github.com/arelate/gog_types"
	"github.com/arelate/gog_urls"
	"github.com/boggydigital/vangogh/internal"
	//"github.com/arelate/gog_auth"
	"github.com/boggydigital/kvas"
	"io"
	"log"
	"net/url"
	"strconv"
)

const (
	Store            = "store"
	Account          = "account"
	Wishlist         = "wishlist"
	Details          = "details"
	StoreProducts    = "store-products"
	AccountProducts  = "account-products"
	WishlistProducts = "wishlist-products"
)

type getUrl func(string, gog_types.Media) *url.URL

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
		return gog_urls.DefaultProductsPage, nil
	case Account:
		return gog_urls.DefaultAccountProductsPage, nil
	case Wishlist:
		return gog_urls.DefaultWishlistPage, nil
	case Details:
		return gog_urls.Details, nil
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
		return StoreProducts
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
	case StoreProducts:
		return Store
	case AccountProducts:
		return Account
	case WishlistProducts:
		return Wishlist
	default:
		return pt
	}
}

func fetchItem(id string, pt string, media gog_types.Media, sourceUrl getUrl, destUrl string) (io.Reader, error) {

	log.Printf("fetching %s (%s) %s %-s\n", pt, media, singular(pt), id)

	httpClient, err := internal.HttpClient()
	if err != nil {
		return nil, err
	}

	u := sourceUrl(id, media)
	resp, err := httpClient.Get(u.String())
	if err != nil {
		return nil, err
	}

	if resp.StatusCode < 200 || resp.StatusCode > 299 {
		return nil, fmt.Errorf("error fetching read closer at %s: %s", u.String(), resp.Status)
	}

	vs, err := kvas.NewJsonLocal(destUrl)
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

func fetchPages(productType string, media gog_types.Media, sourceUrl getUrl, destUrl string) error {
	totalPages := 1
	for pp := 1; pp <= totalPages; pp++ {

		rdr, err := fetchItem(strconv.Itoa(pp), productType, media, sourceUrl, destUrl)
		if err != nil {
			return err
		}

		var page gog_types.Page
		if err = json.NewDecoder(rdr).Decode(&page); err != nil {
			return err
		}

		totalPages = page.TotalPages
	}

	return nil
}

func fetchMissing(
	productType string,
	mt gog_types.Media,
	sourceUrl getUrl,
	mainDestUrl, detailDestUrl string) error {
	kvMain, err := kvas.NewJsonLocal(mainDestUrl)
	if err != nil {
		return err
	}

	kvDetail, err := kvas.NewJsonLocal(detailDestUrl)
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
	mt gog_types.Media,
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

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	if requiresAuthentication(pt) {
		li, err := gog_auth.LoggedIn(httpClient)
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

	mt := gog_types.Parse(media)

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
