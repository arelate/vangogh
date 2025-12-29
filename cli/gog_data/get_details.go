package gog_data

import (
	"errors"
	"maps"
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetDetails(ids []string, hc *http.Client, uat string, since int64, force bool) error {

	gda := nod.NewProgress("getting new or updated %s...", vangogh_integration.Details)
	defer gda.Done()

	detailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Details)
	if err != nil {
		return err
	}

	kvDetails, err := kevlar.New(detailsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	var newUpdatedDetails map[string]any

	if len(ids) > 0 {
		newUpdatedDetails = make(map[string]any)
		for _, id := range ids {
			newUpdatedDetails[id] = nil
		}
	} else {
		newUpdatedDetails, err = shared_data.GetDetailsUpdates(since)
		if err != nil {
			return err
		}
	}

	gda.TotalInt(len(newUpdatedDetails))

	if err = fetch.Items(maps.Keys(newUpdatedDetails), reqs.Details(hc, uat), kvDetails, gda, force); err != nil {
		return err
	}

	return ReduceDetails(kvDetails, since)
}

func ReduceDetails(kvDetails kevlar.KeyValues, since int64) error {

	dataType := vangogh_integration.Details

	rda := nod.Begin(" reducing %s...", dataType)
	defer rda.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GOGDetailsProperties()...)
	if err != nil {
		return err
	}

	detailReductions := shared_data.InitReductions(vangogh_integration.GOGDetailsProperties()...)

	updatedDetails := kvDetails.Since(since, kevlar.Create, kevlar.Update)

	for id := range updatedDetails {
		if !kvDetails.Has(id) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + id))
			continue
		}

		if err = reduceDetailsProduct(id, kvDetails, detailReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, detailReductions)
}

func reduceDetailsProduct(id string, kvDetails kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	det, err := vangogh_integration.UnmarshalDetails(id, kvDetails)
	if err != nil {
		return err
	}

	if det == nil {
		return nil
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.TitleProperty:
			values = []string{det.GetTitle()}
		case vangogh_integration.FeaturesProperty:
			values = det.GetFeatures()
		case vangogh_integration.TagIdProperty:
			values = det.GetTagIds()
		case vangogh_integration.GOGReleaseDateProperty:
			values = []string{det.GetGOGRelease()}
		case vangogh_integration.ForumUrlProperty:
			values = []string{det.GetForumUrl()}
		case vangogh_integration.ChangelogProperty:
			values = []string{det.GetChangelog()}
		case vangogh_integration.OperatingSystemsProperty:
			values, err = det.GetOperatingSystems()
			if err != nil {
				return err
			}
		}

		if shared_data.IsNotEmpty(values...) {
			piv[property][id] = values
		}
	}

	return nil
}
