package itemizations

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
)

func Modified(
	since int64,
	pt vangogh_local_data.ProductType) ([]string, error) {

	ma := nod.Begin(" finding modified %s...", pt)
	defer ma.End()

	modSet := make(map[string]bool)

	//licence products can only update through creation, and we've already handled
	//newly created in itemizeMissing func
	if pt == vangogh_local_data.LicenceProducts {
		return nil, nil
	}

	destUrl, err := vangogh_local_data.AbsLocalProductTypeDir(pt)
	if err != nil {
		return nil, ma.EndWithError(err)
	}

	kv, err := kevlar.NewKeyValues(destUrl, kevlar.JsonExt)
	if err != nil {
		return nil, ma.EndWithError(err)
	}

	updatedAfter, err := kv.CreatedOrUpdatedAfter(since)
	if err != nil {
		return nil, ma.EndWithError(err)
	}
	for _, mid := range updatedAfter {
		modSet[mid] = true
	}

	ma.EndWithResult(itemizationResult(modSet))

	return maps.Keys(modSet), nil
}
