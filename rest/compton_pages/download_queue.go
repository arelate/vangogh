package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

func DownloadsQueue(ids []string, rdx redux.Readable, permissions ...author.Permission) compton.PageElement {

	title := "Downloads queue"

	p := compton.Page(title)

	pageStack := compton.FlexItems(p, direction.Column)
	p.Append(pageStack)

	appNavLinks := compton_fragments.AppNavLinks(p, "")
	pageStack.Append(compton.FICenter(p, appNavLinks))

	dqDetails := compton.DSLarge(p, title, len(ids) > 0).
		BackgroundColor(color.Highlight).
		SummaryMarginBlockEnd(size.Normal).
		DetailsMarginBlockEnd(size.Unset).
		SummaryRowGap(size.XXSmall)

	pageStack.Append(dqDetails)

	if len(ids) > 0 {

		cf := compton.NewCountFormatter(
			compton_data.SingleItemTemplate,
			compton_data.ManyItemsSinglePageTemplate,
			compton_data.ManyItemsManyPagesTemplate)

		resultsBadge := compton.BadgeText(p, cf.Title(0, len(ids), len(ids)), color.Foreground)
		dqDetails.AppendBadges(resultsBadge)
	}

	dqDetails.Append(compton_fragments.ProductsList(p, ids, 0, len(ids), rdx, false, permissions...))

	pageStack.Append(compton.Br(), compton_fragments.SyncStatus(p, rdx))

	pageStack.Append(compton.FICenter(p, compton_fragments.GitHubLink(p), compton_fragments.LogoutLink(p)))

	return p
}
