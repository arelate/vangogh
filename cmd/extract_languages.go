package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
)

func getLanguageCodes(exl *vangogh_extracts.ExtractsList) (gost.StrSet, error) {
	langCodeSet := gost.NewStrSet()

	if err := exl.AssertSupport(vangogh_properties.LanguageCodeProperty); err != nil {
		return langCodeSet, err
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
	missingLangs := gost.StrSetWith(langCodeSet.All()...)

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

func extractLanguageNames(langCodeSet gost.StrSet) error {

	fmt.Println("extract language names")

	property := vangogh_properties.LanguageNameProperty

	langNamesEx, err := vangogh_extracts.NewList(property)
	if err != nil {
		return err
	}

	missingLangs, err := getMissingLanguageNames(langCodeSet, langNamesEx, property)
	if err != nil {
		return err
	}

	if missingLangs.Len() == 0 {
		return nil
	}

	missingLangs = gost.StrSetWith(langCodeSet.All()...)
	names := make(map[string][]string, 0)

	//iterate through api-products-v1 until we fill all native names
	vrApiProductsV2, err := vangogh_values.NewReader(vangogh_products.ApiProductsV2, gog_media.Game)
	if err != nil {
		return err
	}

	for _, id := range vrApiProductsV2.All() {
		apv2, err := vrApiProductsV2.ApiProductV2(id)
		if err != nil {
			return err
		}

		updateLanguageNames(apv2.GetLanguages(), missingLangs, names)

		if missingLangs.Len() == 0 {
			break
		}
	}

	return langNamesEx.SetMany(property, names)
}

func extractNativeLanguageNames(langCodeSet gost.StrSet) error {

	fmt.Println("extract native language names")

	property := vangogh_properties.NativeLanguageNameProperty

	langNamesEx, err := vangogh_extracts.NewList(property)
	if err != nil {
		return err
	}

	missingNativeLangs, err := getMissingLanguageNames(langCodeSet, langNamesEx, property)
	if err != nil {
		return err
	}

	if missingNativeLangs.Len() == 0 {
		return nil
	}

	vrApiProductsV1, err := vangogh_values.NewReader(vangogh_products.ApiProductsV1, gog_media.Game)
	if err != nil {
		return err
	}

	missingNativeLangs = gost.StrSetWith(langCodeSet.All()...)
	nativeNames := make(map[string][]string, 0)

	for _, id := range vrApiProductsV1.All() {
		apv1, err := vrApiProductsV1.ApiProductV1(id)
		if err != nil {
			return err
		}

		updateLanguageNames(apv1.GetNativeLanguages(), missingNativeLangs, nativeNames)

		if missingNativeLangs.Len() == 0 {
			break
		}
	}

	return langNamesEx.SetMany(property, nativeNames)
}
