package cmd

import (
	"bytes"
	"fmt"
	"github.com/arelate/gog_auth"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_pages"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/vangogh/internal"
	"io"
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

func itemizeAll(
	ids []string,
	missing, updated bool,
	modifiedAfter int64,
	pt vangogh_products.ProductType,
	mt gog_media.Media) ([]string, error) {

	for _, mainPt := range vangogh_products.MainTypes(pt) {
		if missing {
			missingIds, err := itemizeMissing(pt, mainPt, mt, modifiedAfter)
			if err != nil {
				return ids, err
			}
			if len(missingIds) == 0 {
				fmt.Printf("no missing %s data for %s (%s)\n", pt, mainPt, mt)
			}
			ids = append(ids, missingIds...)
		}
		if updated {
			updatedIds, err := itemizeUpdated(modifiedAfter, mainPt, mt)
			if err != nil {
				return ids, err
			}
			if len(updatedIds) == 0 {
				fmt.Printf("no updated %s data for %s (%s)\n", pt, mainPt, mt)
			}
			ids = append(ids, updatedIds...)
		}
	}

	return ids, nil
}

func itemizeMissing(
	detailPt, mainPt vangogh_products.ProductType,
	mt gog_media.Media,
	modifiedAfter int64) ([]string, error) {

	//api-products-v2 provides includes-games for "PACK"
	if mainPt == vangogh_products.ApiProductsV2 &&
		detailPt == vangogh_products.ApiProductsV2 {
		return itemizeMissingIncludesGames(modifiedAfter)
	}

	//licences give a signal when DLC has been purchased, this would add
	//required (base) game details to the updates
	if mainPt == vangogh_products.LicenceProducts &&
		detailPt == vangogh_products.Details {
		return itemizeRequiredGames(modifiedAfter, mt)
	}

	missingIds := make([]string, 0)

	mainDestUrl, err := vangogh_urls.LocalProductsDir(mainPt, mt)
	if err != nil {
		return missingIds, err
	}

	detailDestUrl, err := vangogh_urls.LocalProductsDir(detailPt, mt)
	if err != nil {
		return missingIds, err
	}

	kvMain, err := kvas.NewJsonLocal(mainDestUrl)
	if err != nil {
		return missingIds, err
	}

	kvDetail, err := kvas.NewJsonLocal(detailDestUrl)
	if err != nil {
		return missingIds, err
	}
	for _, id := range kvMain.All() {
		if !kvDetail.Contains(id) {
			missingIds = append(missingIds, id)
		}
	}

	return missingIds, nil
}

func itemizeUpdated(
	since int64,
	pt vangogh_products.ProductType,
	mt gog_media.Media) ([]string, error) {

	updatedIds := make([]string, 0)

	mainDestUrl, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return updatedIds, err
	}

	kvMain, err := kvas.NewJsonLocal(mainDestUrl)
	if err != nil {
		return updatedIds, err
	}

	updatedIds = kvMain.ModifiedAfter(since)

	return updatedIds, nil
}

func itemizeMissingIncludesGames(modifiedAfter int64) ([]string, error) {

	missingIncludesGames := make([]string, 0)

	//currently api-products-v2 support only gog_media.Game, and since this method is exclusively
	//using api-products-v2 we're fine specifying media directly
	vrApv2, err := vangogh_values.NewReader(vangogh_products.ApiProductsV2, gog_media.Game)

	if err != nil {
		return missingIncludesGames, err
	}

	for _, id := range vrApv2.ModifiedAfter(modifiedAfter) {

		// have to use product reader and not extracts here, since extracts wouldn't be ready
		// while we're still getting data. Attempting to minimize the impact by only querying
		// new or updated api-product-v2 items since start to the sync
		apv2, err := vrApv2.ApiProductV2(id)

		if err != nil {
			return missingIncludesGames, err
		}

		for _, igId := range apv2.GetIncludesGames() {
			if !vrApv2.Contains(igId) {
				missingIncludesGames = append(missingIncludesGames, igId)
			}
		}
	}

	return missingIncludesGames, nil
}

//itemizeRequiredGames enumerates all base products for a newly acquired DLCs
func itemizeRequiredGames(createdAfter int64, mt gog_media.Media) ([]string, error) {
	requiredGamesForNewLicences := make([]string, 0)

	vrLicences, err := vangogh_values.NewReader(vangogh_products.LicenceProducts, mt)
	if err != nil {
		return requiredGamesForNewLicences, err
	}

	vrApv2, err := vangogh_values.NewReader(vangogh_products.ApiProductsV2, gog_media.Game)
	if err != nil {
		return requiredGamesForNewLicences, err
	}

	for _, id := range vrLicences.CreatedAfter(createdAfter) {
		//like in itemizeMissingIncludesGames, we can't use extracts here,
		//because we're in process of getting data and would rather query api-products-v2 directly.
		//the performance impact is expected to be minimal since we're only loading newly acquired licences
		apv2, err := vrApv2.ApiProductV2(id)
		if err != nil {
			return requiredGamesForNewLicences, err
		}

		for _, rg := range apv2.GetRequiredGames() {
			requiredGamesForNewLicences = append(requiredGamesForNewLicences, rg)
		}
	}

	return requiredGamesForNewLicences, nil
}

func getItems(
	ids []string,
	pt vangogh_products.ProductType,
	mt gog_media.Media,
	verbose bool) error {

	if !vangogh_products.SupportsGetItems(pt) {
		return fmt.Errorf("getting %s data is not supported", pt)
	}

	if verbose {
		log.Printf("get data for ids: %v", ids)
	}

	destUrl, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return err
	}

	sourceUrl, err := vangogh_urls.RemoteProductsUrl(pt)
	if err != nil {
		return err
	}

	for _, id := range ids {
		_, err := getItem(id, pt, mt, sourceUrl, destUrl, verbose)
		if err != nil {
			log.Printf("couldn't get data for %s (%s) %s: %v", pt, mt, id, err)
		}
	}
	return nil
}

func getItem(
	id string,
	pt vangogh_products.ProductType,
	mt gog_media.Media,
	sourceUrl vangogh_urls.ProductTypeUrl,
	destUrl string,
	verbose bool) (io.Reader, error) {

	fmt.Printf("get %s (%s) data %s\n", pt, mt, id)

	httpClient, err := internal.HttpClient()
	if err != nil {
		return nil, err
	}

	u := sourceUrl(id, mt)
	if verbose {
		log.Printf("source url %s", u)
	}

	resp, err := httpClient.Get(u.String())
	if err != nil {
		return nil, err
	}

	if resp.StatusCode < 200 || resp.StatusCode > 299 {
		return nil, fmt.Errorf("unexpected status: %s", resp.Status)
	}

	vs, err := kvas.NewJsonLocal(destUrl)
	if err != nil {
		return resp.Body, err
	}

	var b bytes.Buffer
	tr := io.TeeReader(resp.Body, &b)

	if verbose {
		log.Printf("set value for item %s at destination %s", id, destUrl)
	}
	if err = vs.Set(id, tr); err != nil {
		return &b, err
	}

	if err := resp.Body.Close(); err != nil {
		return &b, err
	}

	return &b, nil
}
