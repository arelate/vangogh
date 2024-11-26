package cli

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/url"
	"slices"
)

func CascadeValidationHandler(u *url.URL) error {

	return CascadeValidation(
		vangogh_local_data.OperatingSystemsFromUrl(u),
		vangogh_local_data.ValuesFromUrl(u, vangogh_local_data.LanguageCodeProperty),
		vangogh_local_data.FlagFromUrl(u, "no-patches"))
}

func CascadeValidation(
	operatingSystems []vangogh_local_data.OperatingSystem,
	langCodes []string,
	noPatches bool) error {

	cva := nod.NewProgress("cascading product installers validation...")
	defer cva.End()

	rdx, err := vangogh_local_data.NewReduxWriter(
		vangogh_local_data.OwnedProperty,
		vangogh_local_data.NativeLanguageNameProperty,
		vangogh_local_data.IsRequiredByGamesProperty,
		vangogh_local_data.IsIncludedByGamesProperty,
		vangogh_local_data.ProductTypeProperty,
		vangogh_local_data.ManualUrlStatusProperty,
		vangogh_local_data.ManualUrlValidationResultProperty,
		vangogh_local_data.ProductValidationResultProperty)
	if err != nil {
		return err
	}

	productValidationResults := make(map[string][]string)

	vrDetails, err := vangogh_local_data.NewProductReader(vangogh_local_data.Details)
	if err != nil {
		return cva.EndWithError(err)
	}

	keys, err := vrDetails.Keys()
	if err != nil {
		return cva.EndWithError(err)
	}

	cva.TotalInt(len(keys))

	for _, id := range keys {

		if id == "1453375253" {
			fmt.Println("1453375253")
		}

		det, err := vrDetails.Details(id)
		if err != nil {
			return cva.EndWithError(err)
		}

		dls, err := vangogh_local_data.FromDetails(det, rdx)
		if err != nil {
			return cva.EndWithError(err)
		}

		// will only propagate installers validation results
		dls = dls.Only(operatingSystems, langCodes, []vangogh_local_data.DownloadType{vangogh_local_data.Installer}, noPatches)

		productVrs := make([]vangogh_local_data.ValidationResult, 0, len(dls))

		for _, dl := range dls {
			if vrs, ok := rdx.GetLastVal(vangogh_local_data.ManualUrlValidationResultProperty, dl.ManualUrl); ok {
				vr := vangogh_local_data.ParseValidationResult(vrs)
				productVrs = append(productVrs, vr)
			} else {
				productVrs = append(productVrs, vangogh_local_data.ValidationResultUnknown)
			}
		}

		slices.Sort(productVrs)

		if len(productVrs) > 0 {
			productValidationResults[id] = []string{productVrs[len(productVrs)-1].String()}
		} else {
			productValidationResults[id] = []string{vangogh_local_data.ValidationResultUnknown.String()}
		}

		cva.Increment()
	}

	if err := rdx.BatchReplaceValues(vangogh_local_data.ProductValidationResultProperty, productValidationResults); err != nil {
		return cva.EndWithError(err)
	}

	return nil
}

func filterOwnedRelated(rdx kevlar.ReadableRedux, p, id string) []string {
	ownedRelated := make([]string, 0)
	if related, ok := rdx.GetAllValues(p, id); ok {
		for _, rid := range related {
			if own, ok := rdx.GetLastVal(vangogh_local_data.OwnedProperty, rid); ok && own == "true" {
				ownedRelated = append(ownedRelated, rid)
			}
		}
	}
	return ownedRelated
}
