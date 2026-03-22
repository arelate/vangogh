package gog_data

import (
	"errors"
	"io"
	"maps"
	"net/http"
	"strings"

	"github.com/arelate/southern_light/gog_integration"
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

	return ReduceDetails(kvDetails, since, force)
}

func ReduceDetails(kvDetails kevlar.KeyValues, since int64, force bool) error {

	dataType := vangogh_integration.Details

	rda := nod.Begin(" reducing %s...", dataType)
	defer rda.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GOGDetailsProperties()...)
	if err != nil {
		return err
	}

	detailsReductions := shared_data.InitReductions(vangogh_integration.GOGDetailsProperties()...)
	detailsKeyValues, err := shared_data.InitKeyValues(vangogh_integration.GOGDetailsKeyValues()...)
	if err != nil {
		return err
	}

	updatedDetails := kvDetails.Since(since, kevlar.Create, kevlar.Update)

	for id := range updatedDetails {
		if !kvDetails.Has(id) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + id))
			continue
		}

		var det *gog_integration.Details
		if det, err = vangogh_integration.UnmarshalDetails(id, kvDetails); err != nil {
			return err
		}

		if err = reduceDetailsProductProperties(id, det, detailsReductions); err != nil {
			return err
		}
		if err = reduceDetailsKeyValues(id, det, detailsKeyValues, force); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, detailsReductions)
}

func reduceDetailsProductProperties(id string, det *gog_integration.Details, piv shared_data.PropertyIdValues) error {

	if det == nil {
		return nil
	}

	var err error

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
		case vangogh_integration.OperatingSystemsProperty:
			values, err = det.GetOperatingSystems()
			if err != nil {
				return err
			}
		case vangogh_integration.BackgroundProperty:
			values = []string{gog_integration.ImageId(det.GetBackground())}
		}

		if shared_data.IsNotEmpty(values...) {
			piv[property][id] = values
		}
	}

	return nil
}

func reduceDetailsKeyValues(id string, det *gog_integration.Details, detailsKeyValues map[string]kevlar.KeyValues, force bool) error {

	if det == nil {
		return nil
	}

	var err error
	var reader io.Reader

	for kv := range detailsKeyValues {

		if detailsKeyValues[kv].Has(id) && !force {
			continue
		}

		reader = nil

		switch kv {
		case vangogh_integration.ChangelogKeyValues:
			cl := det.GetChangelog()
			if cl != "" {
				reader = strings.NewReader(cl)
			}
		}

		if reader != nil {
			if err = detailsKeyValues[kv].Set(id, reader); err != nil {
				return err
			}
		}
	}

	return nil
}
