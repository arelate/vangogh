package cmd

import (
	"fmt"
	"github.com/arelate/gog_auth"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_pages"
	"github.com/arelate/vangogh_products"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/fetch"
	"github.com/boggydigital/vangogh/cmd/itemize"
	"github.com/boggydigital/vangogh/cmd/split"
	"github.com/boggydigital/vangogh/internal"
	"log"
)

//GetData gets remote data from GOG.com and stores as local products (splitting as paged data if needed)
func GetData(
	idSet gost.StrSet,
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

	//only print "header" for details types, since they go really well with
	//itemization detailed information
	if vangogh_products.IsDetail(pt) {
		fmt.Printf("get %s (%s):\n", pt, mt)
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	if vangogh_products.RequiresAuth(pt) {
		if verbose {
			log.Printf("%s requires authenticated session, checking if user is logged in.", pt)
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
		return split.Pages(pt, mt, since)
	}

	if vangogh_products.IsArray(pt) {
		// using "licences" as id, since that's how we store that data in kvas
		if err := fetch.Items([]string{vangogh_products.Licences.String()}, pt, mt, verbose); err != nil {
			return err
		}
		return split.Pages(pt, mt, since)
	}

	idSet, err = itemize.All(idSet, missing, updated, since, pt, mt)
	if err != nil {
		return err
	}

	approvedIds := idSet.Except(gost.NewStrSetWith(denyIds...))

	return fetch.Items(approvedIds, pt, mt, verbose)
}
