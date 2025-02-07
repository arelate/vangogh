package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reductions"
	"net/url"
)

func GetPurchasesHandler(u *url.URL) error {
	since, err := vangogh_integration.SinceFromUrl(u)
	if err != nil {
		return err
	}

	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return GetPurchases(
		since,
		ids,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.ValuesFromUrl(u, vangogh_integration.LanguageCodeProperty),
		vangogh_integration.DownloadTypesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "no-patches"),
		vangogh_integration.FlagFromUrl(u, "force"))
}

func GetPurchases(
	since int64,
	ids []string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	force bool) error {

	productTypes := []vangogh_integration.ProductType{
		vangogh_integration.OrderPage, // required for Search > Owned
		vangogh_integration.Licences,  // required for ownership check
		vangogh_integration.AccountPage,
		vangogh_integration.Details,
	}

	for _, pt := range productTypes {
		if err := GetDataLegacy(ids, nil, pt, since, false, false); err != nil {
			return err
		}
	}

	if err := reductions.Orders(since); err != nil {
		return err
	}

	if err := reductions.Owned(); err != nil {
		return err
	}

	if err := GetDownloads(ids, operatingSystems, langCodes, downloadTypes, noPatches, false, force); err != nil {
		return err
	}

	if err := Validate(ids, operatingSystems, langCodes, downloadTypes, noPatches, false); err != nil {
		return err
	}

	if err := Summarize(since); err != nil {
		return err
	}

	return nil
}
