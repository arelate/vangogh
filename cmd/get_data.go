package cmd

import (
	"github.com/arelate/gog_auth"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_pages"
	"github.com/arelate/vangogh_products"
	"github.com/boggydigital/vangogh/internal"
	"log"
)

//GetData gets remote data from GOG.com and stores as local products (splitting as paged data if needed)
func GetData(
	ids []string,
	denyIds []string,
	pt vangogh_products.ProductType,
	mt gog_media.Media,
	since int64,
	missing bool,
	updated bool,
	verbose bool) error {

	if !vangogh_products.Valid(pt) {
		log.Printf("%s is not a valid product type", pt)
		return nil
	}

	if !vangogh_products.SupportsMedia(pt, mt) {
		if verbose {
			log.Printf("%s doesn't support %s media", pt, mt)
		}
		return nil
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	if vangogh_products.RequiresAuth(pt) {
		if verbose {
			log.Printf("%s requires authenticated session, checking if user is logged in...", pt)
		}

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
		return split(pt, mt, since)
	}

	if vangogh_products.IsArray(pt) {
		if err := getItems([]string{vangogh_products.Licences.String()}, pt, mt, verbose); err != nil {
			return err
		}
		return split(pt, mt, since)
	}

	ids, err = itemizeAll(ids, missing, updated, since, pt, mt)
	if err != nil {
		return err
	}

	approvedIds := make([]string, 0, len(ids))
	for _, id := range ids {
		if !stringsContain(denyIds, id) {
			approvedIds = append(approvedIds, id)
		}
	}

	return getItems(approvedIds, pt, mt, verbose)
}
