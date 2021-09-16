package extract

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
)

func TagNames(mt gog_media.Media) error {
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
