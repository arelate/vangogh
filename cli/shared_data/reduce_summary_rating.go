package shared_data

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"strconv"
)

func ReduceSummaryRatings() error {

	rsra := nod.Begin(" reducing %s...", vangogh_integration.SummaryRatingProperty)
	defer rsra.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.ReduxProperties()...)
	if err != nil {
		return err
	}

	avgSummaryRatings := make(map[string][]string)
	avgSummaryReviews := make(map[string][]string)

	for id := range rdx.Keys(vangogh_integration.TitleProperty) {

		summaryRating := 0
		summaryRatingsCount := 0

		if grs, ok := rdx.GetLastVal(vangogh_integration.RatingProperty, id); ok && grs != "" && grs != "0" {
			if grf, err := strconv.ParseFloat(grs, 32); err == nil {
				summaryRating += int(grf * 2)
				summaryRatingsCount++
			} else {
				nod.LogError(err)
			}
		}

		if srs, ok := rdx.GetLastVal(vangogh_integration.SteamReviewScoreProperty, id); ok && srs != "" && srs != "0" {
			if sri, err := strconv.ParseInt(srs, 10, 32); err == nil {
				summaryRating += int(sri) * 10
				summaryRatingsCount++
			} else {
				nod.LogError(err)
			}
		}

		if ars, ok := rdx.GetLastVal(vangogh_integration.AggregatedRatingProperty, id); ok && ars != "" && ars != "0" {
			if ari, err := strconv.ParseFloat(ars, 32); err == nil {
				summaryRating += int(ari)
				summaryRatingsCount++
			} else {
				nod.LogError(err)
			}
		}

		if mrs, ok := rdx.GetLastVal(vangogh_integration.MetacriticScoreProperty, id); ok && mrs != "" && mrs != "0" {
			if mri, err := strconv.ParseInt(mrs, 10, 32); err == nil {
				summaryRating += int(mri)
				summaryRatingsCount++
			} else {
				nod.LogError(err)
			}
		}

		if hrs, ok := rdx.GetLastVal(vangogh_integration.HltbReviewScoreProperty, id); ok && hrs != "" && hrs != "0" {
			if hri, err := strconv.ParseInt(hrs, 10, 32); err == nil {
				summaryRating += int(hri)
				summaryRatingsCount++
			} else {
				nod.LogError(err)
			}
		}

		if ocrs, ok := rdx.GetLastVal(vangogh_integration.OpenCriticMedianScoreProperty, id); ok && ocrs != "" && ocrs != "0" {
			if ocri, err := strconv.ParseFloat(ocrs, 32); err == nil {
				summaryRating += int(ocri)
				summaryRatingsCount++
			} else {
				nod.LogError(err)
			}
		}

		if summaryRatingsCount > 0 {
			avgSummaryRating := summaryRating / summaryRatingsCount
			avgSummaryRatings[id] = []string{fmt.Sprintf("%03d", avgSummaryRating)}
			avgSummaryReviews[id] = []string{vangogh_integration.RatingDesc(int64(avgSummaryRating))}
		}
	}

	if err = rdx.BatchReplaceValues(vangogh_integration.SummaryRatingProperty, avgSummaryRatings); err != nil {
		return err
	}

	if err = rdx.BatchReplaceValues(vangogh_integration.SummaryReviewsProperty, avgSummaryReviews); err != nil {
		return err
	}

	return nil
}
