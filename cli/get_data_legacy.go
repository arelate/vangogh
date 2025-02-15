package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"net/url"
)

func GetDataLegacyHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	productType := vangogh_integration.ProductTypeFromUrl(u)

	skipIds := vangogh_integration.ValuesFromUrl(u, "skip-id")

	updated := vangogh_integration.FlagFromUrl(u, "updated")
	since, err := vangogh_integration.SinceFromUrl(u)
	if err != nil {
		return err
	}

	missing := vangogh_integration.FlagFromUrl(u, "missing")

	return GetDataLegacy(
		ids,
		skipIds,
		productType,
		since,
		missing,
		updated)
}

// GetDataLegacy gets remote data from GOG.com and stores as local products (splitting as paged data if needed)
func GetDataLegacy(
	ids []string,
	skipIds []string,
	pt vangogh_integration.ProductType,
	since int64,
	missing bool,
	updated bool) error {

	//gda := nod.NewProgress("getting %s data...", pt)
	//defer gda.Done()
	//
	//if !vangogh_integration.IsValidProductType(pt) {
	//	gda.EndWithResult("%s is not a valid product type", pt)
	//	return nil
	//}
	//
	//acp, err := vangogh_integration.AbsCookiePath()
	//if err != nil {
	//	return err
	//}
	//
	//hc, err := coost.NewHttpClientFromFile(acp)
	//if err != nil {
	//	return err
	//}
	//
	//if vangogh_integration.IsProductRequiresAuth(pt) {
	//	err := gog_integration.IsLoggedIn(hc)
	//	if err != nil {
	//		return err
	//	}
	//}
	//
	//if vangogh_integration.IsGOGPagedProduct(pt) {
	//	if err := fetch.Pages(pt, since, hc, gda); err != nil {
	//		return err
	//	}
	//	return split(pt, since)
	//}
	//
	//if vangogh_integration.IsArrayProduct(pt) {
	//	ids := []string{pt.String()}
	//	if err := fetch.Items(ids, pt, hc); err != nil {
	//		return err
	//	}
	//	return split(pt, since)
	//}
	//
	//ids, err = itemizations.All(ids, missing, updated, since, pt)
	//if err != nil {
	//	return err
	//}
	//
	//skipIdSet := make(map[string]bool, len(skipIds))
	//for _, id := range skipIds {
	//	skipIdSet[id] = true
	//}
	//
	//approvedIds := make([]string, 0, len(ids))
	//
	//for _, id := range ids {
	//	if !skipIdSet[id] {
	//		approvedIds = append(approvedIds, id)
	//	}
	//}
	//
	//return fetch.Items(approvedIds, pt, hc)
	return nil
}
