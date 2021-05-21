package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"strings"
)

func stringsTrimSpace(stringsWithSpace []string) []string {
	trimmedStrings := make([]string, 0, len(stringsWithSpace))
	for _, str := range stringsWithSpace {
		trimmedStrings = append(trimmedStrings, strings.TrimSpace(str))
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

	return tagNameEx.AddMany(vangogh_properties.TagNameProperty, tagIdNames)
}

func noMissingNames(codes map[string]bool) bool {
	nmn := true
	for _, ok := range codes {
		nmn = nmn && ok
	}
	return nmn
}

func extractLanguageNames(exl *vangogh_extracts.ExtractsList) error {

	fmt.Println("extract language names")

	langNameEx, err := vangogh_extracts.NewList(vangogh_properties.LanguageNameProperty)
	if err != nil {
		return err
	}

	codes := make(map[string]bool, 0)
	names := make(map[string][]string, 0)

	//digest distinct languages codes
	for _, id := range exl.All(vangogh_properties.LanguageCodesProperty) {
		idCodes, ok := exl.GetAllRaw(vangogh_properties.LanguageCodesProperty, id)
		if !ok {
			continue
		}
		for _, code := range idCodes {
			codes[code] = true
		}
	}

	//map all language codes to names
	for lc, _ := range codes {
		_, ok := langNameEx.Get(vangogh_properties.LanguageNameProperty, lc)
		if !ok {
			codes[lc] = false
		}
	}

	if noMissingNames(codes) {
		return nil
	}

	//iterate through api-products-v2 until we fill all names
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
			if !codes[lc] {
				names[lc] = []string{name}
				codes[lc] = true
			}
		}

		if noMissingNames(codes) {
			break
		}
	}

	return langNameEx.AddMany(vangogh_properties.LanguageNameProperty, names)
}

func Extract(modifiedAfter int64, mt gog_media.Media, properties map[string]bool) error {

	if properties == nil {
		properties = make(map[string]bool, 0)
	}

	if len(properties) == 0 {
		for _, ep := range vangogh_properties.Extracted() {
			properties[ep] = true
		}
	}

	typesEx, err := vangogh_extracts.NewList(vangogh_properties.TypesProperty)
	if err != nil {
		return err
	}

	exl, err := vangogh_extracts.NewListFromMap(properties)
	if err != nil {
		return err
	}

	idsTypes := make(map[string][]string)

	for _, pt := range vangogh_products.Local() {

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}

		missingProps := vangogh_properties.Supported(pt, properties)

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

			if idsTypes[id] == nil {
				idsTypes[id] = make([]string, 0)
			}

			idsTypes[id] = append(idsTypes[id], pt.String())

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
				missingPropExtracts[prop][id] = stringsTrimSpace(values)
			}
		}

		for prop, extracts := range missingPropExtracts {
			if err := exl.AddMany(prop, extracts); err != nil {
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

	//tag-names are extracted separately from other types,
	//given it's most convenient to extract from account-pages
	if err := extractTagNames(mt); err != nil {
		return err
	}

	return typesEx.AddMany(vangogh_properties.TypesProperty, idsTypes)
}
