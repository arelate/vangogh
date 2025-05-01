package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
)

func ReduceMisc() error {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.ReduxProperties()...)
	if err != nil {
		return err
	}

	if err = reduceOwned(rdx); err != nil {
		return err
	}

	if err = reduceTypes(rdx); err != nil {
		return err
	}

	if err = reduceSummaryRatings(rdx); err != nil {
		return err
	}

	if err = reduceDeveloperPublisher(rdx); err != nil {
		return err
	}

	if err = reduceTopPercent(rdx); err != nil {
		return err
	}

	if err = reduceDemo(rdx); err != nil {
		return err
	}

	if err = reduceCredits(rdx); err != nil {
		return err
	}

	return nil
}
