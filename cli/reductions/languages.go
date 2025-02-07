package reductions

//func GetLanguageCodes(rdx redux.Readable) (map[string]bool, error) {
//
//	lca := nod.Begin(" %s...", vangogh_integration.LanguageCodeProperty)
//	defer lca.EndWithResult("done")
//
//	langCodeSet := make(map[string]bool)
//
//	if err := rdx.MustHave(vangogh_integration.LanguageCodeProperty); err != nil {
//		return langCodeSet, lca.EndWithError(err)
//	}
//
//	//digest distinct languages codes
//	for id := range rdx.Keys(vangogh_integration.LanguageCodeProperty) {
//		idCodes, ok := rdx.GetAllValues(vangogh_integration.LanguageCodeProperty, id)
//		if !ok {
//			continue
//		}
//		for _, code := range idCodes {
//			langCodeSet[code] = true
//		}
//	}
//
//	return langCodeSet, nil
//}
//
//func getMissingLanguageNames(
//	langCodeSet map[string]bool,
//	rdx redux.Readable,
//	property string) (map[string]bool, error) {
//
//	missingLangs := maps.Clone(langCodeSet)
//
//	// TODO: write a comment explaining all or nothing approach
//	//map all language codes to names and hide existing
//	for lc := range missingLangs {
//		if _, ok := rdx.GetLastVal(property, lc); ok {
//			delete(missingLangs, lc)
//		}
//	}
//
//	return missingLangs, nil
//}
//
//func updateLanguageNames(languages map[string]string, missingNames map[string]bool, names map[string][]string) {
//	for langCode, langName := range languages {
//		if missingNames[langCode] {
//			names[langCode] = []string{langName}
//			delete(missingNames, langCode)
//		}
//	}
//}
//
//func LanguageNames(langCodeSet map[string]bool) error {
//	property := vangogh_integration.LanguageNameProperty
//
//	lna := nod.Begin(" %s...", property)
//	defer lna.EndWithResult("done")
//
//	langNamesEx, err := vangogh_integration.NewReduxWriter(property)
//	if err != nil {
//		return lna.EndWithError(err)
//	}
//
//	missingLangs, err := getMissingLanguageNames(langCodeSet, langNamesEx, property)
//	if err != nil {
//		return lna.EndWithError(err)
//	}
//
//	if len(missingLangs) == 0 {
//		return nil
//	}
//
//	missingLangs = maps.Clone(langCodeSet)
//	names := make(map[string][]string, 0)
//
//	//iterate through api-products-v1 until we fill all native names
//	vrApiProductsV2, err := vangogh_integration.NewProductReader(vangogh_integration.ApiProductsV2)
//	if err != nil {
//		return lna.EndWithError(err)
//	}
//
//	for id := range vrApiProductsV2.Keys() {
//		apv2, err := vrApiProductsV2.ApiProductV2(id)
//		if err != nil {
//			return lna.EndWithError(err)
//		}
//
//		updateLanguageNames(apv2.GetLanguages(), missingLangs, names)
//
//		if len(missingLangs) == 0 {
//			break
//		}
//	}
//
//	if err := langNamesEx.BatchReplaceValues(property, names); err != nil {
//		return lna.EndWithError(err)
//	}
//
//	return nil
//}
//
//func NativeLanguageNames(langCodeSet map[string]bool) error {
//	property := vangogh_integration.NativeLanguageNameProperty
//
//	nlna := nod.Begin(" %s...", property)
//	defer nlna.End()
//
//	langNamesEx, err := vangogh_integration.NewReduxWriter(property)
//	if err != nil {
//		return nlna.EndWithError(err)
//	}
//
//	missingNativeLangs, err := getMissingLanguageNames(langCodeSet, langNamesEx, property)
//	if err != nil {
//		return nlna.EndWithError(err)
//	}
//
//	if len(missingNativeLangs) == 0 {
//		nlna.EndWithResult("done")
//		return nil
//	}
//
//	vrApiProductsV1, err := vangogh_integration.NewProductReader(vangogh_integration.ApiProductsV1)
//	if err != nil {
//		return nlna.EndWithError(err)
//	}
//
//	missingNativeLangs = maps.Clone(langCodeSet)
//	nativeNames := make(map[string][]string, 0)
//
//	for id := range vrApiProductsV1.Keys() {
//		apv1, err := vrApiProductsV1.ApiProductV1(id)
//		if err != nil {
//			return nlna.EndWithError(err)
//		}
//
//		updateLanguageNames(apv1.GetNativeLanguages(), missingNativeLangs, nativeNames)
//
//		if len(missingNativeLangs) == 0 {
//			break
//		}
//	}
//
//	if err := langNamesEx.BatchReplaceValues(property, nativeNames); err != nil {
//		return nlna.EndWithError(err)
//	}
//
//	nlna.EndWithResult("done")
//
//	return nil
//}
