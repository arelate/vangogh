package itemizations

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
)

func Modified(
	since int64,
	pt vangogh_integration.ProductType) ([]string, error) {

	ma := nod.Begin(" finding modified %s...", pt)
	defer ma.End()

	modSet := make(map[string]bool)

	//licence products can only update through creation, and we've already handled
	//newly created in itemizeMissing func
	//if pt == vangogh_integration.LicenceProducts {
	//	return nil, nil
	//}

	destUrl, err := vangogh_integration.AbsProductTypeDir(pt)
	if err != nil {
		return nil, ma.EndWithError(err)
	}

	kv, err := kevlar.New(destUrl, kevlar.JsonExt)
	if err != nil {
		return nil, ma.EndWithError(err)
	}

	for mid := range kv.Since(since, kevlar.Create, kevlar.Update) {
		modSet[mid] = true
	}

	ma.EndWithResult(itemizationResult(modSet))

	return maps.Keys(modSet), nil
}
