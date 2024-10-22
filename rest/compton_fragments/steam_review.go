package compton_fragments

import (
	"fmt"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/font_weight"
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

	votedRow := compton.FlexItems(r, direction.Row).
		AlignItems(align.Center).
		ColumnGap(size.Small)

	fs := compton.Fspan(r, "").ForegroundColor(votedColor)
	fs.AddClass("symbol")
	fs.Append(compton.SvgUse(r, compton.Circle))

	votedHeading := compton.H3Text(votedTitle)
	//votedHeading.Append(compton.Fspan(r, votedTitle))

	votedRow.Append(fs, votedHeading)

	container.Append(votedRow)

	header := compton.FlexItems(r, direction.Row).ColumnGap(size.Small).RowGap(size.Unset).FontSize(size.Small)

	authorRow := SteamReviewHeadingRow(r, "Author")
	if review.Author.NumGamesOwned > 0 {
		AppendSteamReviewPropertyValue(r, authorRow, "Games:", strconv.Itoa(review.Author.NumGamesOwned))
	}
	if review.Author.NumReviews > 0 {
		AppendSteamReviewPropertyValue(r, authorRow, "Reviews:", strconv.Itoa(review.Author.NumReviews))
	}

	datesRow := SteamReviewHeadingRow(r, "Review")
	if review.TimestampCreated > 0 {
		AppendSteamReviewPropertyValue(r, datesRow, "Cr:", EpochDate(review.TimestampCreated))
	}
	if review.TimestampUpdated > 0 {
		AppendSteamReviewPropertyValue(r, datesRow, "Upd:", EpochDate(review.TimestampUpdated))
	}

	playtimeRow := SteamReviewHeadingRow(r, "Playtime")
	if review.Author.PlaytimeAtReview > 0 {
		AppendSteamReviewPropertyValue(r, playtimeRow, "At review:", minutesToHours(review.Author.PlaytimeAtReview))
	}
	if review.Author.PlaytimeLastTwoWeeks > 0 {
		AppendSteamReviewPropertyValue(r, playtimeRow, "Last 2w:", minutesToHours(review.Author.PlaytimeLastTwoWeeks))
	}
	if review.Author.PlaytimeForever > 0 {
		AppendSteamReviewPropertyValue(r, playtimeRow, "Total:", minutesToHours(review.Author.PlaytimeForever))
	}
	if review.Author.DeckPlaytimeAtReview > 0 {
		AppendSteamReviewPropertyValue(r, playtimeRow, "Steam Deck:", minutesToHours(review.Author.DeckPlaytimeAtReview))
	}

	noticeRow := SteamReviewHeadingRow(r, "")
	if review.PrimarilySteamDeck {
		AppendSteamReviewNotice(r, noticeRow, "Primarily Steam Deck")
	}
	if !review.SteamPurchase {
		AppendSteamReviewNotice(r, noticeRow, "Not Steam purchase")
	}
	if review.ReceivedForFree {
		AppendSteamReviewNotice(r, noticeRow, "Received for free")
	}
	if review.WrittenDuringEarlyAccess {
		AppendSteamReviewNotice(r, noticeRow, "Written during Early Access")
	}

	header.Append(authorRow, playtimeRow, datesRow)
	if noticeRow.HasChildren() {
		header.Append(noticeRow)
	}
	container.Append(header)

	var reviewContainer compton.Element
	if len(review.Review) > longReviewThreshold {
		dsTitleText := fmt.Sprintf("Show full review (%d chars)", len(review.Review))
		dsTitle := compton.Fspan(r, dsTitleText).
			ForegroundColor(color.Gray).
			FontWeight(font_weight.Bolder)
		dsReview := compton.DSSmall(r, dsTitle, false)
		container.Append(dsReview)
		reviewContainer = dsReview
	} else {
		reviewContainer = container
	}
	reviewContainer.Append(compton.Fspan(r, review.Review))

	votesRow := SteamReviewHeadingRow(r, "Votes")
	if review.VotesUp > 0 {
		AppendSteamReviewPropertyValue(r, votesRow, "Helpful:", strconv.Itoa(review.VotesUp))
	}
	if review.VotesFunny > 0 {
		AppendSteamReviewPropertyValue(r, votesRow, "Funny:", strconv.Itoa(review.VotesFunny))
	}

	if review.VotesUp > 0 || review.VotesFunny > 0 {
		container.Append(votesRow)
	}

	return container
}

func minutesToHours(m int) string {
	return strconv.Itoa(m/60) + "h"
}

func EpochDate(e int64) string {
	return time.Unix(e, 0).Format("Jan 2, '06")
}

func AppendSteamReviewPropertyValue(r compton.Registrar, c compton.Element, p, v string) {
	c.Append(compton.Fspan(r, p).ForegroundColor(color.Gray))
	c.Append(compton.Fspan(r, v))
}

func AppendSteamReviewNotice(r compton.Registrar, c compton.Element, n string) {
	notice := compton.Fspan(r, n).
		FontWeight(font_weight.Bolder).
		ForegroundColor(color.Orange)
	c.Append(notice)
}

func SteamReviewHeadingRow(r compton.Registrar, title string) compton.Element {
	row := compton.FlexItems(r, direction.Row).
		ColumnGap(size.XSmall).
		RowGap(size.Unset).
		AlignItems(align.Center).
		FontSize(size.Small)
	if title != "" {
		row.Append(compton.Fspan(r, title).FontWeight(font_weight.Bolder))
	}
	return row

}
