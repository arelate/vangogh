package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/redux"
)

func SummaryReviews(id string, rdx redux.Readable) (string, color.Color) {

	if srp, ok := rdx.GetLastVal(vangogh_integration.SummaryReviewsProperty, id); ok {

		var c color.Color
		switch srp {
		case vangogh_integration.RatingPositive:
			c = color.Green
		case vangogh_integration.RatingNegative:
			c = color.Red
		case vangogh_integration.RatingMixed:
			c = color.Yellow
		default:
			c = color.Gray
		}

		return srp, c

	}

	return vangogh_integration.RatingUnknown, color.Gray
}
