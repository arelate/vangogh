package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/redux"
	"strings"
)

func SummaryReception(id string, rdx redux.Readable) (string, color.Color) {

	if srep, ok := rdx.GetLastVal(vangogh_integration.SummaryReviewsProperty, id); ok {

		var c color.Color
		switch srep {
		case vangogh_integration.RatingPositive:
			c = color.Green
		case vangogh_integration.RatingNegative:
			c = color.Red
		case vangogh_integration.RatingMixed:
			c = color.Yellow
		default:
			c = color.Gray
		}

		ratingsReviews := srep

		if srap, sure := rdx.GetLastVal(vangogh_integration.SummaryRatingProperty, id); sure {
			ratingsReviews = fmtAggregatedRating(srap)
		}

		if ttp, sure := rdx.GetLastVal(vangogh_integration.TopTenPercentProperty, id); sure && ttp == vangogh_integration.TrueValue {
			ratingsReviews = strings.Join([]string{ratingsReviews, "<b>Top 10%</b>"}, ", ")
		}

		return ratingsReviews, c

	}

	return vangogh_integration.RatingUnknown, color.Gray
}
