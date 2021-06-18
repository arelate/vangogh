package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"strings"
)

func stringsTrimSpace(stringsWithSpace []string) []string {
	trimmedStrings := make([]string, 0, len(stringsWithSpace))
	for _, str := range stringsWithSpace {
		tStr := strings.TrimSpace(str)
		if tStr == "" {
			continue
		}
		trimmedStrings = append(trimmedStrings, tStr)
	}
	return trimmedStrings
}

func extractTagNames(mt gog_media.Media) error {
	fmt.Println("extract tag names")
	vrAccountPage, err := vangogh_values.NewReader(vangogh_products.AccountPage, mt)
	if err != nil {
		return err
	}

	const fpId = "1"
	if !vrAccountPage.Contains(fpId) {
		return fmt.Errorf("vangogh: %s doesn't contain page %s", vangogh_products.AccountPage, fpId)
	}

	firstPage, err := vrAccountPage.AccountPage(fpId)
	if err != nil {
		return err
	}

	tagNameEx, err := vangogh_extracts.NewList(vangogh_properties.TagNameProperty)
	if err != nil {
		return err
	}

	tagIdNames := make(map[string][]string, 0)

	for _, tag := range firstPage.Tags {
		tagIdNames[tag.Id] = []string{tag.Name}
	}

	return tagNameEx.SetMany(vangogh_properties.TagNameProperty, tagIdNames)
}

func noMissingNames(codes map[string]bool) bool {
	nmn := true
	for _, ok := range codes {
		nmn = nmn && ok
	}
	return nmn
}

func extractLanguageNames(exl *vangogh_extracts.ExtractsList) error {

	if err := exl.AssertSupport(vangogh_properties.LanguageCodeProperty); err != nil {
		return err
	}

	fmt.Println("extract language names")

	langCodeSet := gost.NewStrSet()

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

	langNamesEx, err := vangogh_extracts.NewList(
		vangogh_properties.LanguageNameProperty,
		vangogh_properties.NativeLanguageNameProperty)
	if err != nil {
		return err
	}

	names := make(map[string][]string, 0)

	// TODO: write a comment explaining all or nothing approach
	//map all language codes to names and hide existing
	missingLangs := gost.StrSetWith(langCodeSet.All()...)
	for _, lc := range missingLangs.All() {
		if _, ok := langNamesEx.Get(vangogh_properties.LanguageNameProperty, lc); ok {
			missingLangs.Hide(lc)
		}
	}

	if missingLangs.Len() == 0 {
		return nil
	}

	missingLangs = gost.StrSetWith(langCodeSet.All()...)

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

		languages := apv2.GetLanguages()
		for lc, name := range languages {
			if missingLangs.Has(lc) {
				names[lc] = []string{name}
				missingLangs.Hide(lc)
			}
		}

		if missingLangs.Len() == 0 {
			break
		}
	}

	return langNamesEx.SetMany(vangogh_properties.LanguageNameProperty, names)
}

//TODO: DRY this based on extractLanguageNames
func extractNativeLanguageNames(exl *vangogh_extracts.ExtractsList) error {

	if err := exl.AssertSupport(vangogh_properties.LanguageCodeProperty); err != nil {
		return err
	}

	fmt.Println("extract native language names")

	langCodeSet := gost.NewStrSet()

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

	langNamesEx, err := vangogh_extracts.NewList(vangogh_properties.NativeLanguageNameProperty)
	if err != nil {
		return err
	}

	nativeNames := make(map[string][]string, 0)

	// TODO: write a comment explaining all or nothing approach
	//map all language codes to names and hide existing
	missingNativeLangs := gost.StrSetWith(langCodeSet.All()...)
	for _, lc := range langCodeSet.All() {
		if _, ok := langNamesEx.Get(vangogh_properties.NativeLanguageNameProperty, lc); ok {
			missingNativeLangs.Hide(lc)
		}
	}

	if missingNativeLangs.Len() == 0 {
		return nil
	}

	vrApiProductsV1, err := vangogh_values.NewReader(vangogh_products.ApiProductsV1, gog_media.Game)
	if err != nil {
		return err
	}

	missingNativeLangs = gost.StrSetWith(langCodeSet.All()...)

	for _, id := range vrApiProductsV1.All() {
		apv1, err := vrApiProductsV1.ApiProductV1(id)
		if err != nil {
			return err
		}

		nativeLanguages := apv1.GetNativeLanguages()
		for lc, name := range nativeLanguages {
			if missingNativeLangs.Has(lc) {
				nativeNames[lc] = []string{name}
				missingNativeLangs.Hide(lc)
			}
		}

		if missingNativeLangs.Len() == 0 {
			break
		}
	}

	return langNamesEx.SetMany(vangogh_properties.NativeLanguageNameProperty, nativeNames)
}

func extractTypes(mt gog_media.Media) error {

	fmt.Println("extract types")

	idsTypes := make(map[string][]string)

	for _, pt := range vangogh_products.Local() {

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}

		for _, id := range vr.All() {

			if idsTypes[id] == nil {
				idsTypes[id] = make([]string, 0)
			}

			idsTypes[id] = append(idsTypes[id], pt.String())
		}
	}

	typesEx, err := vangogh_extracts.NewList(vangogh_properties.TypesProperty)
	if err != nil {
		return err
	}

	return typesEx.SetMany(vangogh_properties.TypesProperty, idsTypes)
}

func Extract(modifiedAfter int64, mt gog_media.Media, properties []string) error {

	propSet := gost.StrSetWith(properties...)

	if len(properties) == 0 {
		propSet.Add(vangogh_properties.Extracted()...)
	}

	exl, err := vangogh_extracts.NewList(propSet.All()...)
	if err != nil {
		return err
	}

	for _, pt := range vangogh_products.Local() {

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}

		missingProps := vangogh_properties.Supported(pt, propSet.All())

		missingPropExtracts := make(map[string]map[string][]string, 0)

		var modifiedIds []string
		if modifiedAfter > 0 {
			modifiedIds = vr.ModifiedAfter(modifiedAfter, false)
		} else {
			modifiedIds = vr.All()
		}

		if len(modifiedIds) == 0 {
			continue
		}

		fmt.Printf("extract %s\n", pt)

		for _, id := range modifiedIds {

			if len(missingProps) == 0 {
				continue
			}

			propValues, err := vangogh_properties.GetProperties(id, vr, missingProps)
			if err != nil {
				return err
			}

			for prop, values := range propValues {
				if _, ok := missingPropExtracts[prop]; !ok {
					missingPropExtracts[prop] = make(map[string][]string, 0)
				}
				if trValues := stringsTrimSpace(values); len(trValues) > 0 {
					missingPropExtracts[prop][id] = trValues
				}
			}
		}

		for prop, extracts := range missingPropExtracts {
			if err := exl.SetMany(prop, extracts); err != nil {
				return err
			}
		}
	}

	//language-names are extracted separately from general pipeline,
	//given we'll be filling the blanks from api-products-v2 using
	//GetLanguages property that returns map[string]string
	if err := extractLanguageNames(exl); err != nil {
		return err
	}

	if err := extractNativeLanguageNames(exl); err != nil {
		return err
	}

	//tag-names are extracted separately from other types,
	//given it's most convenient to extract from account-pages
	if err := extractTagNames(mt); err != nil {
		return err
	}

	return extractTypes(mt)
}
