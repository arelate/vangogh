package reductions

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

func TagNames() error {

	tna := nod.Begin(" %s...", vangogh_integration.TagNameProperty)
	defer tna.End()

	vrAccountPage, err := vangogh_integration.NewProductReader(vangogh_integration.AccountPage)
	if err != nil {
		return tna.EndWithError(err)
	}

	const fpId = "1"
	has, err := vrAccountPage.Has(fpId)
	if err != nil {
		return tna.EndWithError(err)
	}
	if !has {
		err := fmt.Errorf("%s doesn't contain page %s", vangogh_integration.AccountPage, fpId)
		return tna.EndWithError(err)
	}

	firstPage, err := vrAccountPage.AccountPage(fpId)
	if err != nil {
		return tna.EndWithError(err)
	}

	tagNameEx, err := vangogh_integration.NewReduxWriter(vangogh_integration.TagNameProperty)
	if err != nil {
		return tna.EndWithError(err)
	}

	tagIdNames := make(map[string][]string, 0)

	for _, tag := range firstPage.Tags {
		tagIdNames[tag.Id] = []string{tag.Name}
	}

	if err := tagNameEx.BatchReplaceValues(vangogh_integration.TagNameProperty, tagIdNames); err != nil {
		return tna.EndWithError(err)
	}

	tna.EndWithResult("done")

	return nil
}
