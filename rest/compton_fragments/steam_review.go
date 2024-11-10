package compton_fragments

import (
	"fmt"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"strconv"
	"time"
)

const longReviewThreshold = 750

func SteamReview(r compton.Registrar, review steam_integration.Review) compton.Element {

	container := compton.FlexItems(r, direction.Column).RowGap(size.Normal)

	votedTitle := "Not Recommended"
	votedColor := color.Red
	if review.VotedUp {
		votedTitle = "Recommended"
		votedColor = color.Green
	}

	container.Append(compton.H3Text(votedTitle))

	topFr := compton.Frow(r).
		FontSize(size.Small).
		IconColor(compton.Circle, votedColor).Heading("Author")

	if review.Author.NumGamesOwned > 0 {
		topFr.PropVal("Games", strconv.Itoa(review.Author.NumGamesOwned))
	}
	if review.Author.NumReviews > 0 {
		topFr.PropVal("Reviews", strconv.Itoa(review.Author.NumReviews))
	}

	topFr.Heading("Review")
	if review.TimestampCreated > 0 {
		topFr.PropVal("Cr", EpochDate(review.TimestampCreated))
	}
	if review.TimestampUpdated > 0 {
		topFr.PropVal("Upd", EpochDate(review.TimestampUpdated))
	}

	topFr.Heading("Playtime")
	if review.Author.PlaytimeAtReview > 0 {
		topFr.PropVal("At review", minutesToHours(review.Author.PlaytimeAtReview))
	}
	if review.Author.PlaytimeLastTwoWeeks > 0 {
		topFr.PropVal("Last 2w", minutesToHours(review.Author.PlaytimeLastTwoWeeks))
	}
	if review.Author.PlaytimeForever > 0 {
		topFr.PropVal("Total", minutesToHours(review.Author.PlaytimeForever))
	}
	if review.Author.DeckPlaytimeAtReview > 0 {
		topFr.PropVal("Steam Deck", minutesToHours(review.Author.DeckPlaytimeAtReview))
	}

	if review.PrimarilySteamDeck {
		topFr.Highlight("Primarily Steam Deck")
	}
	if !review.SteamPurchase {
		topFr.Highlight("Not Steam purchase")
	}
	if review.ReceivedForFree {
		topFr.Highlight("Received for free")
	}
	if review.WrittenDuringEarlyAccess {
		topFr.Highlight("Written during Early Access")
	}

	container.Append(topFr)

	var reviewContainer compton.Element
	if len(review.Review) > longReviewThreshold {
		dsTitleText := fmt.Sprintf("Full review (%d chars)", len(review.Review))
		dsReview := compton.DSSmall(r, compton.Fspan(r, dsTitleText), false)
		container.Append(dsReview)
		reviewContainer = dsReview
	} else {
		reviewContainer = container
	}
	reviewContainer.Append(compton.PreText(review.Review))

	if review.VotesUp > 0 || review.VotesFunny > 0 {
		bottomFr := compton.Frow(r).
			FontSize(size.Small).
			Heading("Votes")
		if review.VotesUp > 0 {
			bottomFr.PropVal("Helpful", strconv.Itoa(review.VotesUp))
		}
		if review.VotesFunny > 0 {
			bottomFr.PropVal("Funny", strconv.Itoa(review.VotesFunny))
		}
		container.Append(bottomFr)
	}

	return container
}

func minutesToHours(m int) string {
	return strconv.Itoa(m/60) + "h"
}

func EpochDate(e int64) string {
	return time.Unix(e, 0).Format("Jan 2, '06")
}
