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

	idSet, err := vangogh_local_data.IdSetFromUrl(u)
	if err != nil {
		return err
	}

	return GetPurchases(
		since,
		idSet,
		vangogh_local_data.OperatingSystemsFromUrl(u),
		vangogh_local_data.DownloadTypesFromUrl(u),
		vangogh_local_data.ValuesFromUrl(u, "language-code"),
		vangogh_local_data.FlagFromUrl(u, "exclude-patches"),
		vangogh_local_data.ValueFromUrl(u, "gaugin-url"),
		vangogh_local_data.FlagFromUrl(u, "force"))
}

func GetPurchases(
	since int64,
	idSet map[string]bool,
	operatingSystems []vangogh_local_data.OperatingSystem,
	downloadTypes []vangogh_local_data.DownloadType,
	langCodes []string,
	excludePatches bool,
	gauginUrl string,
	force bool) error {

	productTypes := []vangogh_local_data.ProductType{
		vangogh_local_data.OrderPage, // required for Search > Owned
		vangogh_local_data.Licences,  // required for ownership check
		vangogh_local_data.AccountPage,
		vangogh_local_data.Details,
	}

	for _, pt := range productTypes {
		if err := GetData(idSet, nil, pt, since, false, false); err != nil {
			return err
		}
	}

	if err := reductions.Orders(since); err != nil {
		return err
	}

	if err := reductions.Owned(); err != nil {
		return err
	}

	if err := GetDownloads(idSet, operatingSystems, downloadTypes, langCodes, excludePatches, false, force); err != nil {
		return err
	}

	if err := Validate(idSet, operatingSystems, downloadTypes, langCodes, excludePatches, false, false); err != nil {
		return err
	}

	if err := Summarize(since, gauginUrl); err != nil {
		return err
	}

	return nil
}
