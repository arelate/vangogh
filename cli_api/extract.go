package cli_api

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/extract"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"net/url"
	"strings"
)

func ExtractHandler(u *url.URL) error {
	mt := gog_media.Parse(url_helpers.Value(u, "media"))
	return Extract(0, mt, url_helpers.Values(u, "property"))
}

func Extract(modifiedAfter int64, mt gog_media.Media, properties []string) error {

	propSet := gost.NewStrSetWith(properties...)

	if len(properties) == 0 {
		propSet.Add(vangogh_properties.Extracted()...)
	}

	//required for language-* properties extraction below
	if !propSet.Has(vangogh_properties.LanguageCodeProperty) {
		propSet.Add(vangogh_properties.LanguageCodeProperty)
	}

	ea := nod.Begin("extracting...")
	defer ea.End()

	exl, err := vangogh_extracts.NewList(propSet.All()...)
	if err != nil {
		return ea.EndWithError(err)
	}

	for _, pt := range vangogh_products.Local() {

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return ea.EndWithError(err)
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

		pta := nod.NewProgress(" %s...", pt)
		pta.TotalInt(len(modifiedIds))

		for _, id := range modifiedIds {

			if len(missingProps) == 0 {
				pta.Increment()
				continue
			}

			propValues, err := vangogh_properties.GetProperties(id, vr, missingProps)
			if err != nil {
				return pta.EndWithError(err)
			}

			for prop, values := range propValues {
				if _, ok := missingPropExtracts[prop]; !ok {
					missingPropExtracts[prop] = make(map[string][]string, 0)
				}
				if trValues := stringsTrimSpace(values); len(trValues) > 0 {
					missingPropExtracts[prop][id] = trValues
				}
			}

			pta.Increment()
		}

		for prop, extracts := range missingPropExtracts {
			if err := exl.SetMany(prop, extracts); err != nil {
				return pta.EndWithError(err)
			}
		}

		pta.EndWithResult("done")
	}

	//language-names are extracted separately from general pipeline,
	//given we'll be filling the blanks from api-products-v2 using
	//GetLanguages property that returns map[string]string
	langCodeSet, err := extract.GetLanguageCodes(exl)
	if err != nil {
		return ea.EndWithError(err)
	}

	if err := extract.LanguageNames(langCodeSet); err != nil {
		return ea.EndWithError(err)
	}

	if err := extract.NativeLanguageNames(langCodeSet); err != nil {
		return ea.EndWithError(err)
	}

	//tag-names are extracted separately from other types,
	//given it is most convenient to extract from account-pages
	if err := extract.TagNames(mt); err != nil {
		return ea.EndWithError(err)
	}

	//orders are extracted separately from other types
	if err := extract.Orders(modifiedAfter); err != nil {
		return ea.EndWithError(err)
	}

	if err := extract.Types(mt); err != nil {
		return ea.EndWithError(err)
	}

	return nil
}

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
