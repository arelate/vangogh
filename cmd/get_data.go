package cmd

import (
	"fmt"
	"github.com/arelate/gog_auth"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_pages"
	"github.com/arelate/vangogh_products"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/fetch"
	"github.com/boggydigital/vangogh/cmd/http_client"
	"github.com/boggydigital/vangogh/cmd/itemize"
	"github.com/boggydigital/vangogh/cmd/lines"
	"github.com/boggydigital/vangogh/cmd/split"
	"github.com/boggydigital/vangogh/cmd/url_helpers"
	"log"
	"net/url"
	"time"
)

func GetDataHandler(u *url.URL) error {
	idSet, err := url_helpers.IdSet(u)
	if err != nil {
		return err
	}

	pt := vangogh_products.Parse(url_helpers.Value(u, "product-type"))
	mt := gog_media.Parse(url_helpers.Value(u, "media"))

	denyIdsFile := url_helpers.Value(u, "deny-ids-file")
	denyIds := lines.Read(denyIdsFile)

	updated := url_helpers.Flag(u, "updated")
	since := time.Now().Unix()
	if updated {
		since = time.Now().Add(-time.Hour * 24).Unix()
	}
	missing := url_helpers.Flag(u, "missing")

	return GetData(idSet, denyIds, pt, mt, since, missing, updated)
}

//GetData gets remote data from GOG.com and stores as local products (splitting as paged data if needed)
func GetData(
	idSet gost.StrSet,
	denyIds []string,
	pt vangogh_products.ProductType,
	mt gog_media.Media,
	since int64,
	missing bool,
	updated bool) error {

	if !vangogh_products.Valid(pt) {
		log.Printf("%s is not a valid product type", pt)
		return nil
	}

	if !vangogh_products.SupportsMedia(pt, mt) {
		return nil
	}

	//only print "header" for details types, since they go really well with
	//itemization detailed information
	if vangogh_products.IsDetail(pt) {
		fmt.Printf("get %s (%s):\n", pt, mt)
	}

	httpClient, err := http_client.Default()
	if err != nil {
		return err
	}

	if vangogh_products.RequiresAuth(pt) {
		li, err := gog_auth.LoggedIn(httpClient)
		if err != nil {
			return err
		}

		if !li {
			log.Fatalf("user is not logged in")
		}
	}

	if vangogh_products.IsPaged(pt) {
		if err := vangogh_pages.GetAllPages(httpClient, pt, mt); err != nil {
			return err
		}
		return split.Pages(pt, mt, since)
	}

	if vangogh_products.IsArray(pt) {
		// using "licences" as id, since that's how we store that data in kvas
		if err := fetch.Items([]string{vangogh_products.Licences.String()}, pt, mt); err != nil {
			return err
		}
		return split.Pages(pt, mt, since)
	}

	idSet, err = itemize.All(idSet, missing, updated, since, pt, mt)
	if err != nil {
		return err
	}

	approvedIds := idSet.Except(gost.NewStrSetWith(denyIds...))

	return fetch.Items(approvedIds, pt, mt)
}
