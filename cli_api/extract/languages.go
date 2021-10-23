package extract

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
)

func GetLanguageCodes(exl *vangogh_extracts.ExtractsList) (gost.StrSet, error) {

	lca := nod.Begin(" %s...", vangogh_properties.LanguageCodeProperty)
	defer lca.EndWithResult("done")

	langCodeSet := gost.NewStrSet()

	if err := exl.AssertSupport(vangogh_properties.LanguageCodeProperty); err != nil {
		return langCodeSet, lca.EndWithError(err)
	}

	//digest distinct languages codes
	for _, id := range exl.All(vangogh_properties.LanguageCodeProperty) {
		idCodes, ok := exl.GetAllRaw(vangogh_properties.LanguageCodeProperty, id)
		if !ok {
			continue
		}
		for _, code := range idCodes {
			langCodeSet.Add(code)
		}
	}

	return langCodeSet, nil
}

func getMissingLanguageNames(
	langCodeSet gost.StrSet,
	exl *vangogh_extracts.ExtractsList,
	property string) (gost.StrSet, error) {
	missingLangs := gost.NewStrSetWith(langCodeSet.All()...)

	// TODO: write a comment explaining all or nothing approach
	//map all language codes to names and hide existing
	for _, lc := range missingLangs.All() {
		if _, ok := exl.Get(property, lc); ok {
			missingLangs.Hide(lc)
		}
	}

	return missingLangs, nil
}

func updateLanguageNames(languages map[string]string, missingNames gost.StrSet, names map[string][]string) {
	for langCode, langName := range languages {
		if missingNames.Has(langCode) {
			names[langCode] = []string{langName}
			missingNames.Hide(langCode)
		}
	}
}

func LanguageNames(langCodeSet gost.StrSet) error {
	property := vangogh_properties.LanguageNameProperty

	lna := nod.Begin(" %s...", property)
	defer lna.EndWithResult("done")

	langNamesEx, err := vangogh_extracts.NewList(property)
	if err != nil {
		return lna.EndWithError(err)
	}

	missingLangs, err := getMissingLanguageNames(langCodeSet, langNamesEx, property)
	if err != nil {
		return lna.EndWithError(err)
	}

	if missingLangs.Len() == 0 {
		return nil
	}

	missingLangs = gost.NewStrSetWith(langCodeSet.All()...)
	names := make(map[string][]string, 0)

	//iterate through api-products-v1 until we fill all native names
	vrApiProductsV2, err := vangogh_values.NewReader(vangogh_products.ApiProductsV2, gog_media.Game)
	if err != nil {
		return lna.EndWithError(err)
	}

	for _, id := range vrApiProductsV2.All() {
		apv2, err := vrApiProductsV2.ApiProductV2(id)
		if err != nil {
			return lna.EndWithError(err)
		}

		updateLanguageNames(apv2.GetLanguages(), missingLangs, names)

		if missingLangs.Len() == 0 {
			break
		}
	}

	if err := langNamesEx.SetMany(property, names); err != nil {
		return lna.EndWithError(err)
	}

	return nil
}

func NativeLanguageNames(langCodeSet gost.StrSet) error {
	property := vangogh_properties.NativeLanguageNameProperty

	nlna := nod.Begin(" %s...", property)
	defer nlna.End()

	langNamesEx, err := vangogh_extracts.NewList(property)
	if err != nil {
		return nlna.EndWithError(err)
	}

	missingNativeLangs, err := getMissingLanguageNames(langCodeSet, langNamesEx, property)
	if err != nil {
		return nlna.EndWithError(err)
	}

	if missingNativeLangs.Len() == 0 {
		nlna.EndWithResult("done")
		return nil
	}

	vrApiProductsV1, err := vangogh_values.NewReader(vangogh_products.ApiProductsV1, gog_media.Game)
	if err != nil {
		return nlna.EndWithError(err)
	}

	missingNativeLangs = gost.NewStrSetWith(langCodeSet.All()...)
	nativeNames := make(map[string][]string, 0)

	for _, id := range vrApiProductsV1.All() {
		apv1, err := vrApiProductsV1.ApiProductV1(id)
		if err != nil {
			return nlna.EndWithError(err)
		}

		updateLanguageNames(apv1.GetNativeLanguages(), missingNativeLangs, nativeNames)

		if missingNativeLangs.Len() == 0 {
			break
		}
	}

	if err := langNamesEx.SetMany(property, nativeNames); err != nil {
		return nlna.EndWithError(err)
	}

	nlna.EndWithResult("done")

	return nil
}
