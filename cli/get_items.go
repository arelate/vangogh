package cli

import (
	"fmt"
	"github.com/arelate/vangogh/cli/itemizations"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/nod"
	"net/url"
)

func GetItemsHandler(u *url.URL) error {
	since, err := vangogh_local_data.SinceFromUrl(u)
	if err != nil {
		return nil
	}

	ids, err := vangogh_local_data.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return GetItems(ids, since)
}

func GetItems(
	ids []string,
	since int64) error {

	gia := nod.NewProgress("getting description items...")
	defer gia.End()

	rdx, err := vangogh_local_data.NewReduxReader(
		vangogh_local_data.TitleProperty,
		vangogh_local_data.DescriptionOverviewProperty,
		vangogh_local_data.DescriptionFeaturesProperty)
	if err != nil {
		return gia.EndWithError(err)
	}

	dl := dolo.DefaultClient

	all, err := itemizations.All(ids, false, true, since, vangogh_local_data.ApiProductsV2)
	if err != nil {
		return gia.EndWithError(err)
	}

	gia.TotalInt(len(all))

	for _, id := range all {

		title, ok := rdx.GetLastVal(vangogh_local_data.TitleProperty, id)
		if !ok {
			gia.Log("%s has no title", id)
			continue
		}

		var items []string

		descOverview, ok := rdx.GetLastVal(vangogh_local_data.DescriptionOverviewProperty, id)
		if ok {
			items = vangogh_local_data.ExtractDescItems(descOverview)
		}

		descFeatures, ok := rdx.GetLastVal(vangogh_local_data.DescriptionFeaturesProperty, id)
		if ok {
			items = append(items, vangogh_local_data.ExtractDescItems(descFeatures)...)
		}

		if len(items) < 1 {
			gia.Increment()
			continue
		}

		dia := nod.NewProgress(" %s %s", id, title)
		dia.TotalInt(len(items))

		urls := make([]*url.URL, 0, len(items))
		filenames := make([]string, 0, len(items))

		for _, itemUrl := range items {
			if u, err := url.Parse(itemUrl); err == nil {
				urls = append(urls, u)
				aip, err := vangogh_local_data.AbsItemPath(u.Path)
				if err != nil {
					return gia.EndWithError(err)
				}
				filenames = append(filenames, aip)
			}
		}

		if errs := dl.GetSet(urls, dolo.NewFileIndexSetter(filenames), dia, false); len(errs) > 0 {
			for ui, e := range errs {
				dia.Error(fmt.Errorf("GetSet %s error: %s", urls[ui], e.Error()))
			}
			continue
		}

		dia.EndWithResult("done")
		gia.Increment()
	}

	gia.EndWithResult("done")

	return nil
}
