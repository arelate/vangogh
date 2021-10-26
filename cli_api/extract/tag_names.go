package extract

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/nod"
)

func TagNames(mt gog_media.Media) error {

	tna := nod.Begin(" %s...", vangogh_properties.TagNameProperty)
	defer tna.End()

	vrAccountPage, err := vangogh_values.NewReader(vangogh_products.AccountPage, mt)
	if err != nil {
		return tna.EndWithError(err)
	}

	const fpId = "1"
	if !vrAccountPage.Contains(fpId) {
		err := fmt.Errorf("%s doesn't contain page %s", vangogh_products.AccountPage, fpId)
		return tna.EndWithError(err)
	}

	firstPage, err := vrAccountPage.AccountPage(fpId)
	if err != nil {
		return tna.EndWithError(err)
	}

	tagNameEx, err := vangogh_extracts.NewList(vangogh_properties.TagNameProperty)
	if err != nil {
		return tna.EndWithError(err)
	}

	tagIdNames := make(map[string][]string, 0)

	for _, tag := range firstPage.Tags {
		tagIdNames[tag.Id] = []string{tag.Name}
	}

	if err := tagNameEx.SetMany(vangogh_properties.TagNameProperty, tagIdNames); err != nil {
		return tna.EndWithError(err)
	}

	tna.EndWithResult("done")

	return nil
}
