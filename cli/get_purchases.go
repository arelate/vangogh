package cli

import (
	"github.com/arelate/vangogh_local_data"
	"net/url"
)

func GetPurchasesHandler(u *url.URL) error {
	idSet, err := vangogh_local_data.IdSetFromUrl(u)
	if err != nil {
		return err
	}

	return GetPurchases(
		idSet,
		vangogh_local_data.OperatingSystemsFromUrl(u),
		vangogh_local_data.DownloadTypesFromUrl(u),
		vangogh_local_data.ValuesFromUrl(u, "language-code"),
		vangogh_local_data.FlagFromUrl(u, "exclude-patches"),
		vangogh_local_data.FlagFromUrl(u, "force"))
}

func GetPurchases(
	idSet map[string]bool,
	operatingSystems []vangogh_local_data.OperatingSystem,
	downloadTypes []vangogh_local_data.DownloadType,
	langCodes []string,
	excludePatches bool,
	force bool) error {

	if err := GetData(nil, nil, vangogh_local_data.AccountPage, 0, false, false); err != nil {
		return err
	}

	if err := GetData(idSet, nil, vangogh_local_data.Details, 0, false, false); err != nil {
		return err
	}

	if err := Reduce(0, []string{vangogh_local_data.OwnedProperty}, true); err != nil {
		return err
	}

	if err := GetDownloads(idSet, operatingSystems, downloadTypes, langCodes, excludePatches, false, force); err != nil {
		return err
	}

	if err := Validate(idSet, operatingSystems, downloadTypes, langCodes, excludePatches, false, false); err != nil {
		return err
	}

	return nil
}
