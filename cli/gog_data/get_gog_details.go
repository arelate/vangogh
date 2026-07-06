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

func GetGogDetails(ids []string, hc *http.Client, uat string, since int64, force bool) error {

	gda := nod.NewProgress("getting new or updated %s...", vangogh_integration.GogDetails)
	defer gda.Done()

	gogDetailsDir := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogDetails)

	kvGogDetails, err := kevlar.New(gogDetailsDir, kevlar.JsonExt)
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
		newUpdatedDetails, err = shared_data.GetGogDetailsUpdates(since)
		if err != nil {
			return err
		}
	}

	gda.TotalInt(len(newUpdatedDetails))

	if err = fetch.Items(maps.Keys(newUpdatedDetails), reqs.GogDetails(hc, uat), kvGogDetails, gda, force); err != nil {
		return err
	}

	return ReduceGogDetails(kvGogDetails, since, force)
}

func ReduceGogDetails(kvGogDetails kevlar.KeyValues, since int64, force bool) error {

	dataType := vangogh_integration.GogDetails

	rda := nod.Begin(" reducing %s...", dataType)
	defer rda.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GogDetailsProperties()...)
	if err != nil {
		return err
	}

	detailsReductions := shared_data.InitReductions(vangogh_integration.GogDetailsProperties()...)
	detailsKeyValues, err := shared_data.InitKeyValues(vangogh_integration.GogDetailsKeyValues()...)
	if err != nil {
		return err
	}

	updatedGogDetails := kvGogDetails.Since(since, kevlar.Create, kevlar.Update)

	for id := range updatedGogDetails {
		if !kvGogDetails.Has(id) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + id))
			continue
		}

		var det *gog_integration.Details
		if det, err = vangogh_integration.UnmarshalDetails(id, kvGogDetails); err != nil {
			return err
		}

		if err = reduceGogDetailsProductProperties(id, det, detailsReductions); err != nil {
			return err
		}
		if err = reduceGogDetailsKeyValues(id, det, detailsKeyValues, force); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, detailsReductions)
}

func reduceGogDetailsProductProperties(id string, det *gog_integration.Details, piv shared_data.PropertyIdValues) error {

	if det == nil {
		return nil
	}

	var err error

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.GogTitleProperty:
			values = []string{det.GetTitle()}
		case vangogh_integration.GogFeaturesProperty:
			values = det.GetFeatures()
		case vangogh_integration.GogTagIdProperty:
			values = det.GetTagIds()
		case vangogh_integration.GogReleaseDateProperty:
			values = []string{det.GetGOGRelease()}
		case vangogh_integration.GogForumUrlProperty:
			values = []string{det.GetForumUrl()}
		case vangogh_integration.GogOperatingSystemsProperty:
			values, err = det.GetOperatingSystems()
			if err != nil {
				return err
			}
		case vangogh_integration.GogBackgroundProperty:
			values = []string{gog_integration.ImageId(det.GetBackground())}
		}

		if shared_data.IsNotEmpty(values...) {
			piv[property][id] = values
		}
	}

	return nil
}

func reduceGogDetailsKeyValues(id string, det *gog_integration.Details, detailsKeyValues map[string]kevlar.KeyValues, force bool) error {

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
		case vangogh_integration.GogChangelogKeyValues:
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
