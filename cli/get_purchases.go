package cli

import (
	"github.com/arelate/vangogh/cli/reductions"
	"github.com/arelate/vangogh_local_data"
	"net/url"
)

func GetPurchasesHandler(u *url.URL) error {
	since, err := vangogh_local_data.SinceFromUrl(u)
	if err != nil {
		return err
	}

	ids, err := vangogh_local_data.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return GetPurchases(
		since,
		ids,
		vangogh_local_data.OperatingSystemsFromUrl(u),
		vangogh_local_data.DownloadTypesFromUrl(u),
		vangogh_local_data.ValuesFromUrl(u, vangogh_local_data.LanguageCodeProperty),
		vangogh_local_data.FlagFromUrl(u, "no-patches"),
		vangogh_local_data.FlagFromUrl(u, "force"))
}

func GetPurchases(
	since int64,
	ids []string,
	operatingSystems []vangogh_local_data.OperatingSystem,
	downloadTypes []vangogh_local_data.DownloadType,
	langCodes []string,
	excludePatches bool,
	force bool) error {

	productTypes := []vangogh_local_data.ProductType{
		vangogh_local_data.OrderPage, // required for Search > Owned
		vangogh_local_data.Licences,  // required for ownership check
		vangogh_local_data.AccountPage,
		vangogh_local_data.Details,
	}

	for _, pt := range productTypes {
		if err := GetData(ids, nil, pt, since, false, false); err != nil {
			return err
		}
	}

	if err := reductions.Orders(since); err != nil {
		return err
	}

	if err := reductions.Owned(); err != nil {
		return err
	}

	if err := GetDownloads(ids, operatingSystems, downloadTypes, langCodes, excludePatches, false, force); err != nil {
		return err
	}

	if err := Validate(ids, operatingSystems, downloadTypes, langCodes, excludePatches, false, false); err != nil {
		return err
	}

	if err := Summarize(since); err != nil {
		return err
	}

	return nil
}
