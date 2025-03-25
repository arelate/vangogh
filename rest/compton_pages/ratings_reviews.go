package compton_pages

import (
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

var ratingReviewsProperties = []string{
	vangogh_integration.RatingProperty,
	vangogh_integration.MetacriticScoreProperty,
	vangogh_integration.AggregatedRatingProperty,
	vangogh_integration.HltbReviewScoreProperty,
	vangogh_integration.SteamReviewScoreDescProperty,
}

func RatingsReviews(id string, sar *steam_integration.AppReviews, rdx redux.Readable) compton.PageElement {

	s := compton_fragments.ProductSection(compton_data.RatingsReviewsSection)
	s.RegisterStyles(compton_styles.Styles, "ratings.css")

	pageStack := compton.FlexItems(s, direction.Column).RowGap(size.Normal)
	s.Append(pageStack)

	for _, rrp := range compton_fragments.ProductProperties(s, id, rdx, ratingReviewsProperties...) {
		pageStack.Append(rrp)
	}

	if sar == nil {
		return s
	}

	steamReviewsRow := compton.FlexItems(s, direction.Row).
		AlignItems(align.Center).
		JustifyContent(align.Center).
		ColumnGap(size.Small).
		BackgroundColor(color.Background)
	steamReviewsRow.AddClass("steam-reviews")
	steamReviewsRow.Append(
		compton.Fspan(s, "Steam Reviews").FontSize(size.Small))
	pageStack.Append(steamReviewsRow)

	if len(sar.Reviews) == 0 {
		fs := compton.Fspan(s, "Steam reviews are not available for this product").
			ForegroundColor(color.Gray).
			TextAlign(align.Center)
		pageStack.Append(compton.FICenter(s, fs))
	}

	for ii, review := range sar.Reviews {
		if srf := compton_fragments.SteamReview(s, review); srf != nil {
			pageStack.Append(srf)
		}
		if ii < len(sar.Reviews)-1 {
			pageStack.Append(compton.Hr())
		}
	}

	return s
}
