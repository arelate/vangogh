package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/redux"
)

func ReduceMisc() error {

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(), vangogh_integration.ReduxProperties()...)
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
